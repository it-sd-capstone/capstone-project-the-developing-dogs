using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class PlayerAction : MonoBehaviour
{
    [Header("managers")]
    [SerializeField] public GameBoard board;
    [SerializeField] public GameManager gm;

    [Header("action buttons")]
    [SerializeField] GameObject drive;
    [SerializeField] GameObject fly;
    [SerializeField] GameObject station;
    [SerializeField] GameObject treat;
    [SerializeField] GameObject share;
    [SerializeField] GameObject cure;

    [Header("YNPrompt")]
    [SerializeField] private GameObject ynPanel;
    [SerializeField] private TextMeshProUGUI ynExplanation;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [Header("NoticePanel")]
    [SerializeField] private GameObject noticePanel;
    [SerializeField] private TextMeshProUGUI noticeExplainer;
    [SerializeField] private Button okButton;

    [Header("Disease Selection")]
    [SerializeField] private GameObject diseaseSelectionPanel;
    [SerializeField] private Button redDiseaseButton;
    [SerializeField] private Button blueDiseaseButton;
    [SerializeField] private Button yellowDiseaseButton;
    [SerializeField] private Button blackDiseaseButton;
    [SerializeField] private TextMeshProUGUI diseasePromptText;

    [Header("Player Select")]
    [SerializeField] private GameObject playerSelectPanel;
    [SerializeField] private TextMeshProUGUI pSelectText;
    [SerializeField] private GameObject buttonRect;
    [SerializeField] private GameObject pSelectButtonPrefab;

    // Action states
    private bool isAwaitingMove = false;
    private bool isAwaitingDiseaseSelection = false;
    private string pendingActionType = "";
    private List<City> currentValidDestinations = new List<City>();
    private List<Player> shareable = new List<Player>();
    private int neededForCure;
    private bool taking = false;
    public bool dispatching = false;
    private Player dispatched;
    
    // Callbacks for confirmation
    private Action<bool> currentConfirmationCallback;

    private Player currentP;
    private City currentPendingCity;
    private City pendingTreatCity;
    private string pendingTreatContext;
    private DiseaseColor pendingCure;
    private bool shuttleFlight;
    private bool attemptShare;
    private PlayerCard cardToShare;
    public bool discarding = false;

    public void Start()
    {
        noticePanel.SetActive(false);
        ynPanel.SetActive(false);
        playerSelectPanel.SetActive(false);
        if (diseaseSelectionPanel != null)
            diseaseSelectionPanel.SetActive(false);
        
        // Setup button listeners
        if (yesButton != null)
            yesButton.onClick.AddListener(() => OnConfirmationResponse(true));
        if (noButton != null)
            noButton.onClick.AddListener(() => OnConfirmationResponse(false));
        
        // Setup disease buttons
        if (redDiseaseButton != null)
            redDiseaseButton.onClick.AddListener(() => OnDiseaseSelected(DiseaseColor.Red));
        if (blueDiseaseButton != null)
            blueDiseaseButton.onClick.AddListener(() => OnDiseaseSelected(DiseaseColor.Blue));
        if (yellowDiseaseButton != null)
            yellowDiseaseButton.onClick.AddListener(() => OnDiseaseSelected(DiseaseColor.Yellow));
        if (blackDiseaseButton != null)
            blackDiseaseButton.onClick.AddListener(() => OnDiseaseSelected(DiseaseColor.Black));;
    }

    public void UpdateCurrentPlayer(Player p) 
    { 
        currentP = p; 
        ClearActionState();
    }
    
    // Call when a city is clicked on the board
    public void OnCitySelected(City selectedCity)
    {
        if (!isAwaitingMove)
            return;

        switch (pendingActionType)
        {
            case "Drive":
                // Check if selected city is a valid destination
                if (currentValidDestinations.Contains(selectedCity))
                {
                    // Execute the move
                    ExecuteDrive(selectedCity);
                }
                else
                {
                    ShowMessage("Cannot drive to that city! Must be connected by a line.");
                }
                break; 
            case "Fly":
                if (currentValidDestinations.Contains(selectedCity))
                {
                    ExecuteFly(selectedCity);
                }
                else
                {
                    ShowMessage("Cannot fly to that city! Must be a card in your hand.");
                }
                break;
            case "Build":
                BuildStation(selectedCity);
                break;           
        }
        
    }

    // Driving (move between connected cities)
    public void OnDriveClick()
    {
        if (currentP == null)
        {
            Debug.LogError("No current player selected!");
            return;
        }

        if (isAwaitingMove)
        {
            board.ClearAllHighlights();
            currentValidDestinations.Clear();
            ClearActionState();
            isAwaitingMove = false;
            pendingActionType = "";
            drive.GetComponent<Image>().color = Color.white;
            return;
        }
        
        // Check if player has actions remaining
        if (gm.actionCount <= 0)
        {
            ShowMessage("No actions remaining! End your turn.");
            return;
        }
        
        drive.GetComponent<Image>().color = Color.grey;
        

        // Clear any existing action state
        ClearActionState();

         // Get valid neighboring cities
        currentValidDestinations.Clear();
        foreach (City neighbor in currentP.CurrentCity.neighbors)
        {
            currentValidDestinations.Add(neighbor);
        }
        
        if (currentValidDestinations.Count == 0)
        {
            ShowMessage("No neighboring cities to drive to!");
            return;
        }
        
        // Enter drive mode
        pendingActionType = "Drive";
        isAwaitingMove = true;

        // Highlight all valid destinations
        foreach (City city in currentValidDestinations)
        {
            board.Highlight(city);
        }

        ShowMessage($"Select a city to drive to. ({currentValidDestinations.Count} options)");
    }

    private void ExecuteDrive(City destination)
    {
        // Move the player
        if (!dispatching)
        {
            currentP.MoveTo(destination);
        } else
        {
            dispatched.MoveTo(destination);
        }
        

        if(currentP.RoleName == "Medic" || dispatched.RoleName == "Medic")
        {
            if (board.curePool[DiseaseColor.Red])
            {
                ExecuteTreat(currentP.CurrentCity, DiseaseColor.Red);
            }
            if (board.curePool[DiseaseColor.Blue])
            {
                ExecuteTreat(currentP.CurrentCity, DiseaseColor.Blue);
            }
            if (board.curePool[DiseaseColor.Yellow])
            {
                ExecuteTreat(currentP.CurrentCity, DiseaseColor.Yellow);
            }
            if (board.curePool[DiseaseColor.Black])
            {
                ExecuteTreat(currentP.CurrentCity, DiseaseColor.Black);
            }
        }
        
        // Consume one action
        gm.actionCount--;
        gm.UpdateActionDisplay();
        
        // Update visuals
        if (!dispatching) board.UpdatePlayerPosition(currentP);
        else board.UpdatePlayerPosition(dispatched);
        
        Debug.Log($"{currentP.PlayerName} drove from {currentP.CurrentCity?.cityName} to {destination.cityName}.");
        
        if (!dispatching) ShowMessage($"Drove to {destination.cityName}! {gm.actionCount} actions remaining.");
        else ShowMessage($"You sent {dispatched.PlayerName} to {destination.cityName}");

        drive.GetComponent<Image>().color = Color.white;
        
        // Clear action state
        ClearActionState();
        
        // Check if player has no actions left
        if (gm.actionCount <= 0)
        {
            ShowMessage("No actions remaining.");
        }
    }

    //Flight (movement between non-connected cities)
    public void OnFlyClick()
    {
        if (currentP == null || gm.actionCount <= 0)
        {
            ShowMessage(gm.actionCount <= 0 ? "No actions remaining!" : "No current player selected!");
            return;
        }
        
        if (isAwaitingMove)
        {
            board.ClearAllHighlights();
            currentValidDestinations.Clear();
            ClearActionState();
            isAwaitingMove = false;
            pendingActionType = "";
            fly.GetComponent<Image>().color = Color.white;
            return;
        }

        fly.GetComponent<Image>().color = Color.grey;

        ClearActionState();
        currentValidDestinations.Clear();

        foreach (City city in board.cities)
        {
            if (city == currentP.CurrentCity) break;
            if (city.hasResearchStation) currentValidDestinations.Add(city);
        }
        
        // Get all cities that the player can fly to using cards
        foreach (PlayerCard card in currentP.Hand)
        {
            if (card.City == null) break;

            // sacrifice current city card to go anywhere
            if (card.City == currentP.CurrentCity)
            {
                currentValidDestinations = board.cities.ToList();
                ShowMessage("You have your current city's card so you may fly anywhere!");
            }

            // sacrifice a card to travel to that city
            if (!currentValidDestinations.Contains(card.City))
            {
                if (card.City == currentP.CurrentCity)
                {
                    currentValidDestinations = board.cities;
                }
                currentValidDestinations.Add(card.City);
                
                // Also highlight the city
                board.Highlight(card.City);
            }
        }
        
        if (currentValidDestinations.Count == 0)
        {
            ShowMessage("No city cards in hand to fly with!");
            return;
        }
        
        pendingActionType = "Fly";
        isAwaitingMove = true;
        ShowMessage($"Select a city to fly to using a city card. ({currentValidDestinations.Count} options)");
    }
    
    private void ExecuteFly(City destination)
    {
        // Find the card that matches either current city or destination
        PlayerCard cardToDiscard = null;

        fly.GetComponent<Image>().color = Color.white;

        if (currentP.CurrentCity.hasResearchStation && destination.hasResearchStation)
            shuttleFlight = true;

        // ignore discard procedure if flight between
        if (shuttleFlight)
        {
            ShowConfirmation($"Fly from {currentP.CurrentCity.cityName} to {destination.cityName}?",
            (confirmed) =>
            {
                if (confirmed)
                {
                // Move the player
                currentP.MoveTo(destination);
                    
                // Consume action
                gm.actionCount--;
                gm.UpdateActionDisplay();
                board.UpdatePlayerPosition(currentP);
                    
                Debug.Log($"{currentP.PlayerName} flew to {destination.cityName}. Actions: {gm.actionCount}");
                ShowMessage($"Flew to {destination.cityName}! {gm.actionCount} actions remaining.");
                    
                shuttleFlight = false;
                ClearActionState();
                return;
                }
            });
            return;
        }
        
        foreach (PlayerCard card in currentP.Hand)
        {
            if (card.City == currentP.CurrentCity || card.City == destination)
            {
                cardToDiscard = card;
                break;
            }
        }
        
        if (cardToDiscard == null)
        {
            ShowMessage("No valid card to discard for flight!");
            ClearActionState();
            return;
        }

        // Show confirmation for discarding card
        ShowConfirmation($"Discard {cardToDiscard.City.cityName} card to fly to {destination.cityName}?", 
            (confirmed) => {
                if (confirmed)
                {
                    // Discard the card
                    currentP.DiscardCard(cardToDiscard);
                    gm.DiscardPlayerCard(cardToDiscard);
                    
                    // Move the player
                    if (dispatching) dispatched.MoveTo(destination);
                    else currentP.MoveTo(destination);
                    
                    // Consume action
                    gm.actionCount--;
                    gm.UpdateActionDisplay();
                    board.UpdatePlayerPosition(currentP);
                    
                    Debug.Log($"{currentP.PlayerName} flew to {destination.cityName} discarding {cardToDiscard.City.cityName}. Actions: {gm.actionCount}");
                    if (!dispatching) ShowMessage($"Flew to {destination.cityName}! {gm.actionCount} actions remaining.");
                    else ShowMessage($"Flew {dispatched.PlayerName} to {destination.cityName}!");
                    
                    ClearActionState();
                }
                else
                {
                    // Cancel flight
                    ClearActionState();
                    ShowMessage("Flight cancelled.");
                }
            });
    }
    
    // research station building
    private void ExecuteBuild(City location)
    {
        // Check if building a station is valid
        if (location.hasResearchStation)
        {
            ShowMessage("A research station already exists here!");
            ClearActionState();
            return;
        }
        
        // Check if Operations Expert can build for free
        bool canBuildFree = currentP.Role?.CanBuildStationForFree ?? false;
        
        if (!canBuildFree)
        {
            // Need to discard the city card
            PlayerCard cityCard = currentP.Hand.Find(c => c.City == location);
            CityCard cc = FindAnyObjectByType<CityCard>();
            
            if (cityCard == null)
            {
                ShowMessage($"Need the {location.cityName} city card to build a research station here!");
                ClearActionState();
                return;
            }
            
            // Show confirmation for discarding card
            ShowConfirmation($"Discard {location.cityName} card to build a research station?", 
                (confirmed) => {
                    if (confirmed)
                    {
                        BuildStation(location);
                        currentP.DiscardCard(cityCard);
                        gm.DiscardPlayerCard(cityCard);
                        cc.UpdateCC(location);
                    }
                    else
                    {
                        ClearActionState();
                        ShowMessage("Construction cancelled.");
                    }
                });
        }
        else
        {
            // Operations Expert builds for free
            BuildStation(location);
        }
    }
    
    private void BuildStation(City location)
    {
        if (currentP.BuildResearchStation(gm.board))
        {
            gm.actionCount--;
            gm.UpdateActionDisplay();
            board.UpdateResearchStationVisual(location);
            ShowMessage($"Built research station in {location.cityName}!");
            ClearActionState();
        }
        else
        {
            ShowMessage("Failed to build research station!");
            ClearActionState();
        }
    }
    
    public void OnStationClick()
    {
        if (currentP == null || gm.actionCount <= 0)
        {
            ShowMessage(gm.actionCount <= 0 ? "No actions remaining!" : "No current player selected!");
            return;
        }
        
        ClearActionState();
        pendingActionType = "Build";
        isAwaitingMove = true;
        
        // Highlight only the current city for building
        currentValidDestinations.Clear();
        currentValidDestinations.Add(currentP.CurrentCity);
        ExecuteBuild(currentP.CurrentCity);
    }
    
    // TREAT DISEASE ACTION
    public void OnTreatClick()
    {
        if (currentP == null)
        {
            ShowMessage("No current player selected!");
            return;
        }
        
        // Check if player has actions remaining
        if (gm.actionCount <= 0)
        {
            ShowMessage("No actions remaining! End your turn.");
            return;
        }
        
        // Check if current city has any disease cubes
        bool hasDisease = false;
        foreach (DiseaseColor color in Enum.GetValues(typeof(DiseaseColor)))
        {
            if (currentP.CurrentCity.GetDiseaseCount(color) > 0)
            {
                hasDisease = true;
                break;
            }
        }
        
        if (!hasDisease)
        {
            ShowMessage($"No disease cubes to treat in {currentP.CurrentCity.cityName}!");
            return;
        }
        
        // Clear any existing action state
        ClearActionState();

                // Set up for disease selection
        pendingActionType = "Treat";
        pendingTreatCity = currentP.CurrentCity;
        pendingTreatContext = "current";
        isAwaitingDiseaseSelection = true;
        
        // Show disease selection panel
        ShowDiseaseSelection();
        pendingActionType = "Treat";
    }

    private void ShowDiseaseSelection()
    {
        if (diseaseSelectionPanel != null)
        {
            // Update prompt text
            if (diseasePromptText != null)
            {
                string roleBonus = "";
                if (currentP.Role is MedicRole)
                    roleBonus = "\n(Medic: Will remove ALL cubes of this color)";
                else if (currentP.Role is OperationsExpertRole)
                    roleBonus = "\n(Operations Expert: Can treat from adjacent cities)";
                    
                diseasePromptText.text = $"Select a disease to treat in {currentP.CurrentCity.cityName}.{roleBonus}";
            }
            
            diseaseSelectionPanel.SetActive(true);
        }
    }

    private void OnDiseaseSelected(DiseaseColor color)
    {
        // Close the selection panel
        if (diseaseSelectionPanel != null)
            diseaseSelectionPanel.SetActive(false);
        
        if (pendingActionType == "Treat")
        {
            ExecuteTreat(currentP.CurrentCity, color);
        }
        else if (pendingActionType == "TreatAdjacent")
        {
            // For Operations Expert treating adjacent cities
            ExecuteTreat(currentPendingCity, color);
            currentPendingCity = null;
        }
        
        isAwaitingDiseaseSelection = false;
        pendingActionType = "";
        pendingTreatCity = null;
    }

    private void ExecuteTreat(City city, DiseaseColor color)
    {
        // Check if there are cubes to treat
        int currentCubes = city.GetDiseaseCount(color);
        if (currentCubes <= 0)
        {
            ShowMessage($"No {color} disease cubes in {city.cityName} to treat!");
            return;
        }
        
        // Determine how many cubes to remove (role can modify this)
        int cubesToRemove = 1;
        
        // Check for Medic role - removes all cubes
        if (currentP.Role is MedicRole)
        {
            cubesToRemove = currentCubes;
            ShowMessage($"Medic will remove all {currentCubes} {color} cube(s)!");
        }
        
        // Check if disease is eradicated (cured and no cubes on board)
        bool isEradicated = board.IsEradicated(color);
        if (isEradicated)
        {
            cubesToRemove = currentCubes;
            ShowMessage($"Disease is eradicated! Removing all {color} cubes automatically!");
        }
        
        // Check if disease is cured (but not eradicated)
        bool isCured = board.IsCured(color);
        if (isCured && !isEradicated)
        {
            ShowMessage($"Disease is cured! Treatment is more effective.");
            cubesToRemove = currentCubes;
        }
        
        // Let the role modify how many cubes to remove
        currentP.Role?.OnTreatDisease(city, color, ref cubesToRemove);
        
        // Remove the cubes
        for (int i = 0; i < cubesToRemove; i++)
        {
            if (city.GetDiseaseCount(color) > 0)
            {
                board.RemoveDisease(city, color);
            }
        }
        
        // Consume one action
        gm.actionCount--;
        gm.UpdateActionDisplay();

        // Log and show message
        string roleText = "";
        if (currentP.Role is MedicRole && cubesToRemove > 1)
            roleText = " (Medic removed all cubes!)";
        else if (currentP.Role is OperationsExpertRole && city != currentP.CurrentCity)
            roleText = " (Operations Expert treated adjacent city!)";
            
        ShowMessage($"Treated {cubesToRemove} {color} disease cube(s) in {city.cityName}!{roleText} ");
        Debug.Log($"{currentP.PlayerName} treated {cubesToRemove} {color} cube(s) in {city.cityName}");
        
        // Check if player has no actions left
        if (gm.actionCount <= 0)
        {
            ShowMessage("No actions remaining. Click End Turn when ready.");
        }
        
        // Clear action state
        ClearActionState();
    }

    // share cards between players in city
    public void OnShareClick()
    {
        shareable.Clear();
        foreach (Player player in gm.players)
        {
            if (player.CurrentCity == currentP.CurrentCity && player != currentP)
            {
                
                foreach (PlayerCard card in player.Hand)
                {
                    if(card.City == currentP.CurrentCity)
                    {
                        shareable.Add(player);
                        cardToShare = card;
                        taking = true;
                        break;
                    }
                }
                if (player.RoleName == "Researcher")
                {
                    shareable.Add(currentP);
                    if (shareable.Count != 0)
                    {
                        ShowMessage("Select a card to take from the researcher.");
                        CardUIManager CUIM = FindAnyObjectByType<CardUIManager>();
                        CUIM.ShowPlayerCards(player);
                        attemptShare = true;
                        taking = true;
                        return;
                    }
                }
                
            }
        }
        if (currentP.RoleName == "Researcher")
        {
            ShowMessage("Select a card to give.");
            attemptShare = true;
            taking = false;
            return;
        }
        
        foreach (PlayerCard card in currentP.Hand)
        {
            if (card.City == currentP.CurrentCity)
            {
                cardToShare = card;
                taking = false;
            }
        }
        
        attemptShare = true;
        CardShare(cardToShare);
    }

    public void CardShare(PlayerCard card)
    {
        if (!attemptShare) return;

        cardToShare = card;

        foreach(Player player in shareable)
        {
            if (player.Hand.Count < 7)
            {
                GameObject button = Instantiate(pSelectButtonPrefab, playerSelectPanel.transform);
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = player.PlayerName;
                button.GetComponent<Button>().onClick.AddListener(() => ExecuteShare(player));
            }
        }
        playerSelectPanel.SetActive(true);
    }

    public void ExecuteShare(Player player)
    {
        CardUIManager CUIM = FindAnyObjectByType<CardUIManager>();
        if (!taking)
        {
            ShowMessage($"{cardToShare.City.cityName} card shared with {player.PlayerName}!");
        } else
        {
            ShowMessage($"{cardToShare.City.cityName} taken from {player.PlayerName}");
        }

        if (!taking)
        {
            currentP.Hand.Remove(cardToShare);
            
            player.Hand.Add(cardToShare);
        } else
        {
            player.Hand.Remove(cardToShare);

            currentP.Hand.Add(cardToShare);
        }
        CUIM.ClearHand();
        foreach (PlayerCard card in currentP.Hand)
        {
            CUIM.ShowPlayerCard(card);
        }
        
        playerSelectPanel.SetActive(false);
        
        
        cardToShare = null;
        attemptShare = false;
        gm.actionCount--;
        gm.UpdateActionDisplay();
    }

    public void OnShareCancel()
    {
        shareable.Clear();
        attemptShare = false;
        playerSelectPanel.SetActive(false);
    }

    // cure disease
    public void OnCureClick()
    {
        if (currentP.RoleName == "Scientist") neededForCure = 4;
        else neededForCure = 5;

        if (currentP.Hand.Count < neededForCure)
        {
            ShowMessage("Not enough cards to cure a disease.");
            return;
        }
        int red = 0;
        int blue = 0;
        int yellow = 0;
        int black = 0;
        foreach (PlayerCard card in currentP.Hand)
        {
            switch(card.City.diseaseColor){
                case DiseaseColor.Red:
                    red++;
                    break;
                case DiseaseColor.Blue:
                    blue++;
                    break;
                case DiseaseColor.Yellow:
                    yellow++;
                    break;
                default:
                    black++;
                    break;    
            }
        }
        if(red >= neededForCure)
        {
            pendingCure = DiseaseColor.Red;
            ShowConfirmation($"Cure the red disease for {neededForCure} red cards?", (confirmed)=> {
                if (confirmed)
                {
                    board.CureDisease(pendingCure);
                    int i = 0;
                    List<PlayerCard> ToDiscard = new List<PlayerCard>();
                    foreach (PlayerCard card in currentP.Hand)
                    {
                        if(card.City.diseaseColor == pendingCure && i < neededForCure)
                        {
                            ToDiscard.Add(card);
                            i++;
                        }
                    }
                    foreach (PlayerCard card in ToDiscard)
                    {
                        currentP.DiscardCard(card);
                    }
                    ToDiscard.Clear();
                    ShowMessage($"You cured the {pendingCure} disease!");
                }
                else ShowMessage("Cure Cancelled.");
                });
            
            board.CheckWin();
            return;
        }
        if(blue >= neededForCure)
        {
            pendingCure = DiseaseColor.Blue;
            ShowConfirmation($"Cure the blue disease for {neededForCure} blue cards?", (confirmed)=> {
                if (confirmed)
                {
                    board.CureDisease(pendingCure);
                    int i = 0;
                    List<PlayerCard> ToDiscard = new List<PlayerCard>();
                    foreach (PlayerCard card in currentP.Hand)
                    {
                        if(card.City.diseaseColor == pendingCure && i < neededForCure)
                        {
                            ToDiscard.Add(card);
                            i++;
                        }
                    }
                    foreach (PlayerCard card in ToDiscard)
                    {
                        currentP.DiscardCard(card);
                    }
                    ToDiscard.Clear();
                    ShowMessage($"You cured the {pendingCure} disease!");
                }
                else ShowMessage("Cure Cancelled.");
                });

            board.CheckWin();
            return;
        }
        if(yellow >= neededForCure)
        {
            pendingCure = DiseaseColor.Yellow;
            ShowConfirmation($"Cure the yellow disease for {neededForCure} yellow cards?", (confirmed)=> {
                if (confirmed)
                {
                    board.CureDisease(pendingCure);
                    int i = 0;
                    List<PlayerCard> ToDiscard = new List<PlayerCard>();
                    foreach (PlayerCard card in currentP.Hand)
                    {
                        if(card.City.diseaseColor == pendingCure && i < neededForCure)
                        {
                            ToDiscard.Add(card);
                            i++;
                        }
                    }
                    foreach (PlayerCard card in ToDiscard)
                    {
                        currentP.DiscardCard(card);
                    }
                    ToDiscard.Clear();
                    ShowMessage($"You cured the {pendingCure} disease!");
                }
                else ShowMessage("Cure Cancelled.");
                });
                
            board.CheckWin();
            return;
        }
        if(black >= neededForCure)
        {
            pendingCure = DiseaseColor.Black;
            ShowConfirmation($"Cure the black disease for {neededForCure} black cards?", (confirmed)=> {
                if (confirmed)
                {
                    board.CureDisease(pendingCure);
                    int i = 0;
                    List<PlayerCard> ToDiscard = new List<PlayerCard>();
                    foreach (PlayerCard card in currentP.Hand)
                    {
                        if(card.City.diseaseColor == pendingCure && i < neededForCure)
                        {
                            ToDiscard.Add(card);
                            i++;
                        }
                    }
                    foreach (PlayerCard card in ToDiscard)
                    {
                        currentP.DiscardCard(card);
                    }
                    ToDiscard.Clear();
                    ShowMessage($"You cured the {pendingCure} disease!");
                }
                else ShowMessage("Cure Cancelled.");
                });
                
            board.CheckWin();
            return;
        }
    }
    
    // Helper methods
    public void ClearActionState()
    {
        // Clear all highlights
        if (board != null && currentValidDestinations != null)
        {
            foreach (City city in currentValidDestinations)
            {
                board.ClearHighlight(city);
            }
        }
        
        isAwaitingMove = false;
        pendingActionType = "";
        currentValidDestinations.Clear();
    }
    
    private void ShowMessage(string message)
    {
        Debug.Log(message);

        noticeExplainer.text = message;
        noticePanel.SetActive(true);
        // You can add a UI text element to show messages to the player
        // For example: messageText.text = message;
    }
    
    private void ShowConfirmation(string message, Action<bool> callback)
    {
        currentConfirmationCallback = callback;
        ynExplanation.text = message;
        ynPanel.SetActive(true);
    }
    
    private void OnConfirmationResponse(bool response)
    {
        ynPanel.SetActive(false);
        currentConfirmationCallback?.Invoke(response);
        currentConfirmationCallback = null;
    }

    public void OnOperationsExpertTreatClick()
    {
        if (currentP.Role == null || !(currentP.Role is OperationsExpertRole))
        {
            ShowMessage("Only Operations Expert can treat adjacent cities!");
            return;
        }
        
        if (gm.actionCount <= 0)
        {
            ShowMessage("No actions remaining!");
            return;
        }
        
        ClearActionState();
        
        // Find all adjacent cities with disease
        List<City> adjacentWithDisease = new List<City>();
        foreach (City neighbor in currentP.CurrentCity.neighbors)
        {
            bool hasDisease = false;
            foreach (DiseaseColor color in System.Enum.GetValues(typeof(DiseaseColor)))
            {
                if (neighbor.GetDiseaseCount(color) > 0)
                {
                    hasDisease = true;
                    break;
                }
            }
            if (hasDisease)
            {
                adjacentWithDisease.Add(neighbor);
            }
        }
        
        if (adjacentWithDisease.Count == 0)
        {
            ShowMessage("No adjacent cities have disease cubes to treat!");
            return;
        }
        
        // Highlight adjacent cities with disease
        pendingActionType = "TreatAdjacentSelection";
        isAwaitingMove = true;
        currentValidDestinations = adjacentWithDisease;
        
        foreach (City city in adjacentWithDisease)
        {
            board.Highlight(city);
        }
        
        ShowMessage($"Select an adjacent city to treat (Operations Expert ability).");
    }

    public void PromptDiscard()
    {
        ShowMessage($"Too many cards ({currentP.Hand.Count}/7)! Select a card to discard.");
        CardUIManager CUIM = FindAnyObjectByType<CardUIManager>();
        CUIM.ShowPlayerCards(currentP);
        discarding = true;
    }

    public void Discard(PlayerCard card)
    {
    if (!discarding) return;  // Only process if we're in discard mode
    
    // Check if player actually has this card
    if (!currentP.Hand.Contains(card))
    {
        ShowMessage("Cannot discard: You don't have this card!");
        return;
    }
    
    // Remove the card
    currentP.Hand.Remove(card);
    gm.DiscardPlayerCard(card);
    
    // Reset discard state
    discarding = false;
    
    // Re-check if still over limit
    if (currentP.Hand.Count > 7)
    {
        ShowMessage($"Still have {currentP.Hand.Count} cards. Discard another.");
        discarding = true;
        return;
    }
    
    // Proceed to next phase
    gm.DrawDone();
}

    public void CloseNotice()
    {
        noticePanel.SetActive(false);
    }
}