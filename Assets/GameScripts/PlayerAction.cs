using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAction : MonoBehaviour
{
    [Header("Managers")]
    public GameBoard board;
    public GameManager gm;

    [Header("Action Buttons")]
    public GameObject drive;
    public GameObject fly;
    public GameObject station;
    public GameObject treat;
    public GameObject share;
    public GameObject cure;

    [Header("YN Prompt")]
    public GameObject ynPanel;
    public TextMeshProUGUI ynExplanation;
    public Button yesButton;
    public Button noButton;

    [Header("Notice Panel")]
    public GameObject noticePanel;
    public TextMeshProUGUI noticeExplainer;
    public Button okButton;

    [Header("Disease Selection")]
    public GameObject diseaseSelectionPanel;
    public Button redDiseaseButton;
    public Button blueDiseaseButton;
    public Button yellowDiseaseButton;
    public Button blackDiseaseButton;
    public TextMeshProUGUI diseasePromptText;

    [Header("Player Select")]
    public GameObject playerSelectPanel;
    public TextMeshProUGUI pSelectText;
    public GameObject pSelectButtonPrefab;

    //State
    private Player currentP;
    private bool isAwaitingMove = false;
    private bool isAwaitingDiseaseSelection = false;
    private string pendingActionType = "";
    private List<City> currentValidDestinations = new List<City>();

    //Sharing
    private List<Player> shareable = new List<Player>();
    private PlayerCard cardToShare;
    private bool attemptShare = false;

    // Discard
    public bool discarding = false;

    //Confirmation
    private Action<bool> currentConfirmationCallback;

    //Cure
    private DiseaseColor pendingCure;

    private void Start()
    {
        ynPanel.SetActive(false);
        noticePanel.SetActive(false);
        playerSelectPanel.SetActive(false);
        diseaseSelectionPanel.SetActive(false);

        yesButton.onClick.AddListener(() => OnConfirmationResponse(true));
        noButton.onClick.AddListener(() => OnConfirmationResponse(false));
        okButton.onClick.AddListener(() => noticePanel.SetActive(false));

        redDiseaseButton.onClick.AddListener(() => OnDiseaseSelected(DiseaseColor.Red));
        blueDiseaseButton.onClick.AddListener(() => OnDiseaseSelected(DiseaseColor.Blue));
        yellowDiseaseButton.onClick.AddListener(() => OnDiseaseSelected(DiseaseColor.Yellow));
        blackDiseaseButton.onClick.AddListener(() => OnDiseaseSelected(DiseaseColor.Black));
    }

    public void UpdateCurrentPlayer(Player p)
    {
        currentP = p;
        ClearActionState();
    }

    //UI
    private void ShowMessage(string msg)
    {
        gm.ShowMessage(msg);
    }

    private void ShowNotice(string msg)
    {
        noticePanel.SetActive(true);
        noticeExplainer.text = msg;
    }

    public void PromptDiscard()
    {
        discarding = true;
        ShowNotice("You have more than 7 cards. Select a card to discard.");
    }

    private void ShowConfirmation(string msg, Action<bool> callback)
    {
        ynPanel.SetActive(true);
        ynExplanation.text = msg;
        currentConfirmationCallback = callback;
    }

    private void OnConfirmationResponse(bool confirmed)
    {
        ynPanel.SetActive(false);
        currentConfirmationCallback?.Invoke(confirmed);
        currentConfirmationCallback = null;
    }

    //City click
    public void OnCitySelected(City city)
    {
        if (!isAwaitingMove)
            return;

        if (!currentValidDestinations.Contains(city))
        {
            ShowNotice("That city is not a valid destination.");
            return;
        }

        switch (pendingActionType)
        {
            case "Drive":
                ExecuteDrive(city);
                break;

            case "Fly":
                ExecuteFly(city);
                break;

            case "Build":
                ExecuteBuild(city);
                break;
        }
    }

    //Drive
    public void OnDriveClick()
    {
        if (gm.actionCount <= 0)
        {
            ShowNotice("No actions remaining.");
            return;
        }

        if (isAwaitingMove)
        {
            ClearActionState();
            drive.GetComponent<Image>().color = Color.white;
            return;
        }

        drive.GetComponent<Image>().color = Color.grey;

        currentValidDestinations = currentP.CurrentCity.neighbors.ToList();

        if (currentValidDestinations.Count == 0)
        {
            ShowNotice("No neighboring cities to drive to.");
            return;
        }

        pendingActionType = "Drive";
        isAwaitingMove = true;

        foreach (City c in currentValidDestinations)
            board.Highlight(c);

        ShowNotice("Select a city to drive to.");
    }

    private void ExecuteDrive(City destination)
    {
        currentP.MoveTo(destination);

        gm.actionCount--;
        gm.UpdateActionDisplay();
        board.UpdatePlayerPosition(currentP);

        ShowNotice($"Moved to {destination.cityName}.");

        ClearActionState();
        drive.GetComponent<Image>().color = Color.white;
    }

    //Fly
    public void OnFlyClick()
    {
        if (gm.actionCount <= 0)
        {
            ShowNotice("No actions remaining.");
            return;
        }

        if (isAwaitingMove)
        {
            ClearActionState();
            fly.GetComponent<Image>().color = Color.white;
            return;
        }

        fly.GetComponent<Image>().color = Color.grey;

        currentValidDestinations.Clear();

        //Shuttle flight
        foreach (City c in board.cities)
            if (c.hasResearchStation && c != currentP.CurrentCity)
                currentValidDestinations.Add(c);

        //Direct/charter flight
        foreach (PlayerCard card in currentP.Hand)
        {
            if (card.City == currentP.CurrentCity)
            {
                currentValidDestinations = board.cities.ToList();
                break;
            }

            if (!currentValidDestinations.Contains(card.City))
                currentValidDestinations.Add(card.City);
        }

        if (currentValidDestinations.Count == 0)
        {
            ShowNotice("You cannot fly anywhere.");
            return;
        }

        foreach (City c in currentValidDestinations)
            board.Highlight(c);

        pendingActionType = "Fly";
        isAwaitingMove = true;

        ShowNotice("Select a city to fly to.");
    }

    private void ExecuteFly(City destination)
    {
        PlayerCard discardCard = null;

        foreach (PlayerCard card in currentP.Hand)
        {
            if (card.City == currentP.CurrentCity || card.City == destination)
            {
                discardCard = card;
                break;
            }
        }

        if (discardCard == null)
        {
            ShowNotice("You do not have a valid card to fly.");
            ClearActionState();
            return;
        }

        ShowConfirmation($"Discard {discardCard.City.cityName} to fly to {destination.cityName}?",
            confirmed =>
            {
                if (!confirmed)
                {
                    ClearActionState();
                    return;
                }

                currentP.DiscardCard(discardCard);
                gm.DiscardPlayerCard(discardCard);

                currentP.MoveTo(destination);
                gm.actionCount--;
                gm.UpdateActionDisplay();
                board.UpdatePlayerPosition(currentP);

                ShowNotice($"Flew to {destination.cityName}.");

                ClearActionState();
            });
    }

    //Build
    public void OnStationClick()
    {
        if (gm.actionCount <= 0)
        {
            ShowNotice("No actions remaining.");
            return;
        }

        pendingActionType = "Build";
        isAwaitingMove = true;

        currentValidDestinations.Clear();
        currentValidDestinations.Add(currentP.CurrentCity);

        ExecuteBuild(currentP.CurrentCity);
    }

    private void ExecuteBuild(City city)
    {
        if (city.hasResearchStation)
        {
            ShowNotice("A research station already exists here.");
            ClearActionState();
            return;
        }

        bool free = currentP.Role.CanBuildStationForFree;

        if (!free)
        {
            PlayerCard cityCard = currentP.Hand.Find(c => c.City == city);

            if (cityCard == null)
            {
                ShowNotice("You need this city's card to build a station.");
                ClearActionState();
                return;
            }

            ShowConfirmation($"Discard {city.cityName} to build a station?",
                confirmed =>
                {
                    if (!confirmed)
                    {
                        ClearActionState();
                        return;
                    }

                    currentP.DiscardCard(cityCard);
                    gm.DiscardPlayerCard(cityCard);

                    BuildStation(city);
                });
        }
        else
        {
            BuildStation(city);
        }
    }

    private void BuildStation(City city)
    {
        board.BuildResearchStation(city);
        board.UpdateResearchStationVisual(city);

        gm.actionCount--;
        gm.UpdateActionDisplay();

        ShowNotice($"Built a research station in {city.cityName}.");

        ClearActionState();
    }

    //Treat
    public void OnTreatClick()
    {
        bool hasDisease = Enum.GetValues(typeof(DiseaseColor))
            .Cast<DiseaseColor>()
            .Any(c => currentP.CurrentCity.GetDiseaseCount(c) > 0);

        if (!hasDisease)
        {
            ShowNotice("There are no cubes to treat here.");
            return;
        }

        pendingActionType = "Treat";
        isAwaitingDiseaseSelection = true;

        ShowDiseaseSelection();
    }

    private void ShowDiseaseSelection()
    {
        diseaseSelectionPanel.SetActive(true);

        string bonus = currentP.Role is MedicRole ? "\n(Medic removes ALL cubes)" : "";
        diseasePromptText.text = $"Select a disease to treat.{bonus}";
    }

    private void OnDiseaseSelected(DiseaseColor color)
    {
        diseaseSelectionPanel.SetActive(false);
        ExecuteTreat(currentP.CurrentCity, color);
    }

    private void ExecuteTreat(City city, DiseaseColor color)
    {
        int cubes = city.GetDiseaseCount(color);

        if (cubes == 0)
        {
            ShowNotice("No cubes of that color here.");
            return;
        }

        int remove = 1;

        if (currentP.Role is MedicRole)
            remove = cubes;

        if (board.IsEradicated(color))
            remove = cubes;

        if (board.IsCured(color))
            remove = cubes;

        for (int i = 0; i < remove; i++)
            board.RemoveDisease(city, color);

        gm.actionCount--;
        gm.UpdateActionDisplay();

        ShowNotice($"Removed {remove} {color} cube(s).");

        ClearActionState();
    }

    //Share knowledge
    public void OnShareClick()
    {
        shareable.Clear();
        cardToShare = null;
        attemptShare = false;

        //Find players in same city
        foreach (Player p in gm.players)
            if (p != currentP && p.CurrentCity == currentP.CurrentCity)
                shareable.Add(p);

        if (shareable.Count == 0)
        {
            ShowNotice("No other players in this city.");
            return;
        }

        //Researcher
        if (currentP.Role is ResearcherRole)
        {
            List<Player> validTargets = shareable
                .Where(p => p.Hand.Count < Player.MaxHandSize)
                .ToList();

            if (validTargets.Count == 0)
            {
                ShowNotice("No players here can receive a card.");
                return;
            }

            shareable = validTargets;

            ShowNotice("Select a card to give.");

            CardUIManager cuim = FindAnyObjectByType<CardUIManager>();
            cuim.ClearHand();
            cuim.ShowPlayerCards(currentP);

            attemptShare = true;
            return;
        }

        //Must give matching city card
        cardToShare = currentP.Hand.FirstOrDefault(c => c.City == currentP.CurrentCity);

        if (cardToShare == null)
        {
            ShowNotice("You do not have this city's card, so you cannot share.");
            return;
        }

        List<Player> receivers = shareable
            .Where(p => p.Hand.Count < Player.MaxHandSize)
            .ToList();

        if (receivers.Count == 0)
        {
            ShowNotice("No players here can receive a card.");
            return;
        }

        shareable = receivers;

        attemptShare = true;
        CardShare(cardToShare);
    }

    public void CardShare(PlayerCard card)
    {
        if (!attemptShare)
            return;

        cardToShare = card;

        foreach (Transform child in playerSelectPanel.transform)
            Destroy(child.gameObject);

        foreach (Player p in shareable)
        {
            GameObject btn = Instantiate(pSelectButtonPrefab, playerSelectPanel.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = p.PlayerName;
            btn.GetComponent<Button>().onClick.AddListener(() => ExecuteShare(p));
        }

        pSelectText.text = "Select a player to give the card to:";
        playerSelectPanel.SetActive(true);
    }

    public void ExecuteShare(Player target)
    {
        if (cardToShare == null)
        {
            ShowNotice("No card selected.");
            return;
        }

        //Give card
        currentP.Hand.Remove(cardToShare);
        target.Hand.Add(cardToShare);

        ShowNotice($"{currentP.PlayerName} gave {cardToShare.City.cityName} to {target.PlayerName}.");

        //Refresh UI
        CardUIManager cuim = FindAnyObjectByType<CardUIManager>();
        cuim.ClearHand();
        cuim.ShowPlayerCards(currentP);

        playerSelectPanel.SetActive(false);

        gm.actionCount--;
        gm.UpdateActionDisplay();

        attemptShare = false;
        cardToShare = null;
    }

    //Cure
    public void OnCureClick()
    {
        int needed = currentP.Role.CardsRequiredForCure;

        var groups = currentP.Hand.GroupBy(c => c.City.diseaseColor)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var kv in groups)
        {
            if (kv.Value >= needed)
            {
                pendingCure = kv.Key;

                ShowConfirmation($"Cure {pendingCure} using {needed} cards?", confirmed =>
                {
                    if (!confirmed)
                        return;

                    List<PlayerCard> toDiscard = currentP.Hand
                        .Where(c => c.City.diseaseColor == pendingCure)
                        .Take(needed)
                        .ToList();

                    foreach (PlayerCard c in toDiscard)
                    {
                        currentP.DiscardCard(c);
                        gm.DiscardPlayerCard(c);
                    }

                    board.CureDisease(pendingCure);
                    ShowNotice($"Cured {pendingCure}!");

                    gm.actionCount--;
                    gm.UpdateActionDisplay();
                });

                return;
            }
        }

        ShowNotice("You do not have enough matching cards to cure a disease.");
    }

    //Discard
    public void Discard(PlayerCard card)
    {
        if (!discarding)
            return;

        currentP.DiscardCard(card);
        gm.DiscardPlayerCard(card);

        CardUIManager cuim = FindAnyObjectByType<CardUIManager>();
        cuim.ClearHand();
        cuim.ShowPlayerCards(currentP);

        discarding = false;
        noticePanel.SetActive(false);

        gm.DrawDone();
    }

    //Clear state
    public void ClearActionState()
    {
        isAwaitingMove = false;
        isAwaitingDiseaseSelection = false;
        pendingActionType = "";
        currentValidDestinations.Clear();
        shareable.Clear();
        attemptShare = false;
        cardToShare = null;

        board.ClearAllHighlights();
        playerSelectPanel.SetActive(false);
        diseaseSelectionPanel.SetActive(false);
        ynPanel.SetActive(false);
        noticePanel.SetActive(false);
    }
}
