using System.Collections.Generic;
using NUnit.Framework.Internal;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Core Systems")]
    public GameBoard board;
    public InfectionDeck infectionDeck;
    public PlayerDeck playerDeck;
    public List<Player> players = new List<Player>();
    public PlayerAction playerAction;
    public PlayerInfo playerInfo;

    [Header("Difficulty")]
    public Difficulty difficulty = Difficulty.Standard;

    [Header("UI")]
    public GameInfoUI gameInfoUI;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Player Pawn Settings")]
    public GameObject playerPawnPrefab;
    public RectTransform pawnContainer;

    public int currentPlayerIndex = 0;
    public int infectionRateIndex = 0;
    private int curesFound = 0;
    public int actionCount;
    private CardUIManager cardUIManager;

    private readonly int[] infectionRateTrack = { 2, 2, 2, 3, 3, 4, 4 };

    void Start()
    {
        Debug.Log("Player Count: " + GameSettings.PlayerCount);
        Debug.Log("Difficulty: " + GameSettings.Difficulty);

        ApplyDifficultyFromSettings();
        SetupGame();
        StartTurn();

        Debug.Log("Game Started!");
    }

    void SetupPlayers()
    {
        players.Clear();

        for (int i = 0; i < GameSettings.PlayerCount; i++)
        {
            GameObject playerObject = new GameObject("Player " + (i + 1));
            Player player = playerObject.AddComponent<Player>();

            player.Initialize($"Player {i + 1}", board.cityLookup["Atlanta"], board);
            player.SetPlayerName("Player " + (i + 1));

            string selectedRoleName = GameSettings.SelectedRoles[i];
            Role selectedRole = CreateRoleFromName(selectedRoleName);
            player.SetRole(selectedRole);

            players.Add(player);
        }
    }

    void SetupGame()
    {
        SetupPlayers();
        SpawnPlayerPawns();

        List<PlayerCard> allCards = CreateAllPlayerCards();
        cardUIManager = FindAnyObjectByType<CardUIManager>();

        playerDeck.Initialize(allCards);

        DealStartingCards();

        playerDeck.InsertEpidemicCards(GetEpidemicCount());

        infectionDeck.Initialize(board.cities);
        SetupInitialInfections();

        playerAction = FindAnyObjectByType<PlayerAction>();
        playerInfo = FindAnyObjectByType<PlayerInfo>();
    }

    private void SpawnPlayerPawns()
    {
        foreach (Player p in players)
        {
            GameObject pawn = Instantiate(playerPawnPrefab, pawnContainer);

            //Set pawn color based on role
            Image img = pawn.GetComponent<Image>();
            if (img != null)
                img.color = GetPawnColor(p.Role);

            //Register pawn with GameBoard
            board.playerMarkers[p] = pawn;

            //Move pawn to starting city
            board.UpdatePlayerPosition(p);
        }
    }

    private Color GetPawnColor(Role role)
    {
        if (role is MedicRole)
            return new Color32(255, 140, 0, 255);

        if (role is ScientistRole)
            return new Color32(255, 255, 255, 255);

        if (role is ResearcherRole)
            return new Color32(124, 255, 0, 255);

        if (role is DispatcherRole)
            return new Color32(170, 0, 255, 255);

        if (role is OperationsExpertRole)
            return new Color32(100, 100, 100, 255);

        if (role is QuarantineSpecialistRole)
            return new Color32(0, 200, 0, 255);

        if (role is ContingencyPlannerRole)
            return new Color32(0, 180, 255, 255);

        return Color.black;
    }


    private List<PlayerCard> CreateAllPlayerCards()
    {
        List<PlayerCard> cards = new List<PlayerCard>();

        //Create one card per city using CityDB
        foreach (CityData data in board.citiesDB.cities)
        {
            City city = board.cityLookup[data.cityName];

            PlayerCard card = new PlayerCard
            {
                City = city,
                Color = city.diseaseColor
            };

            cards.Add(card);
        }

        return cards;
    }

    private void ApplyDifficultyFromSettings()
    {
        if (GameSettings.Difficulty == "Introductory")
        {
            difficulty = Difficulty.Introductory;
        }
        else if (GameSettings.Difficulty == "Heroic")
        {
            difficulty = Difficulty.Heroic;
        }
        else
        {
            difficulty = Difficulty.Standard;
        }
    }

    private Role CreateRoleFromName(string roleName)
    {
        switch (roleName)
        {
            case "Medic":
                return new MedicRole();

            case "Scientist":
                return new ScientistRole();

            case "Dispatcher":
                return new DispatcherRole();

            case "Researcher":
                return new ResearcherRole();

            case "Quarantine Specialist":
                return new QuarantineSpecialistRole();

            case "Contingency Planner":
                return new ContingencyPlannerRole();

            case "Operations Expert":
                return new OperationsExpertRole();

            default:
                return null;
        }
    }

    private int GetEpidemicCount()
    {
        switch (difficulty)
        {
            case Difficulty.Introductory: return 4;
            case Difficulty.Standard: return 5;
            case Difficulty.Heroic: return 6;
            default: return 5;
        }
    }

    void SetupInitialInfections()
    {
        for (int i = 0; i < 3; i++)
            InfectFromDeck(3);

        for (int i = 0; i < 3; i++)
            InfectFromDeck(2);

        for (int i = 0; i < 3; i++)
            InfectFromDeck(1);
    }

    //Turn system

    void StartTurn()
    {
        if (players == null || players.Count == 0)
        {
            Debug.LogError("No players assigned to GameManager.");
            return;
        }

        if (currentPlayerIndex < 0 || currentPlayerIndex >= players.Count)
        {
            currentPlayerIndex = 0;
        }

        Player current = players[currentPlayerIndex];

        Debug.Log("Starting turn for: " + current.PlayerName);

        if(cardUIManager != null)
        {
            cardUIManager.ClearHand();
            foreach (PlayerCard card in current.Hand)
            {
                cardUIManager.ShowPlayerCard(card);
            }
        }

        if (gameInfoUI != null)
        {
            gameInfoUI.UpdateGameInfo(current, playerDeck, infectionDeck);
        }

        if (playerAction != null)
        {
            playerAction.UpdateCurrentPlayer(players[currentPlayerIndex]);
        }

        actionCount = 4;
        playerInfo.OnPlayerChange(current);
        UpdateActionDisplay();
    }

    public void EndTurn()
    {
        if (players == null || players.Count == 0)
        {
            Debug.LogError("Cannot end turn because no players exist.");
            return;
        }

        // Clear any pending actions
        if (playerAction != null)
        {
            playerAction.ClearActionState();
        }

        DrawPlayerCardsForCurrentPlayer();
        RunInfectionPhase();

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        StartTurn();
    }

    void DrawPlayerCardsForCurrentPlayer()
    {
        Player currentPlayer = players[currentPlayerIndex];

        for (int i = 0; i < 2; i++)
        {
            PlayerCard card = playerDeck.DrawPlayerCard();

            if (card is EpidemicCard)
            {
                HandleEpidemic();
            }
            else if (card != null)
            {
                currentPlayer.DrawCard(card);
            }
        }

        CheckOverSeven(currentPlayer);
    }

    void CheckOverSeven(Player player)
    {
        if (player.Hand.Count <= 7) return;

        while(player.Hand.Count > 7)
        {
            playerAction.PromptDiscard(player.Hand);
        }
    }

    void RunInfectionPhase()
    {
        int infections = infectionRateTrack[infectionRateIndex];

        for (int i = 0; i < infections; i++)
        {
            InfectionCard card = infectionDeck.DrawInfectionCard();

            if (card != null)
            {
                board.InfectCity(card.City, card.Color);
            }
        }
    }

    // Discard 
    public void DiscardPlayerCard(PlayerCard card)
    {
        if (card != null)
        {
            playerDeck.Discard(card);
            cardUIManager.DiscardCard(card);
        }
    }

    //Epidemic

    private void HandleEpidemic()
    {
        infectionRateIndex = Mathf.Min(infectionRateIndex + 1, infectionRateTrack.Length - 1);

        InfectionCard bottom = infectionDeck.DrawBottomCard();

        if (bottom != null)
        {
            board.InfectCity(bottom.City, bottom.Color);
        }

        infectionDeck.Intensify();
    }

    void InfectFromDeck(int cubeCount)
    {
        InfectionCard card = infectionDeck.DrawInfectionCard();

        if (card == null)
            return;

        for (int i = 0; i < cubeCount; i++)
        {
            board.InfectCity(card.City, card.Color);
        }
    }

    //Win/lose

    public void CureFound()
    {
        curesFound++;
        if (curesFound >= 4)
            WinGame();
    }

    private void WinGame()
    {
        Debug.Log("You win! All cures found.");
    }

    private void LoseGame(string reason)
    {
        Debug.Log("You lose: " + reason);
    }

    void DealStartingCards()
    {
        int cardsPerPlayer = GetStartingCardCount();

        foreach (Player player in players)
        {
            for (int i = 0; i < cardsPerPlayer; i++)
            {
                PlayerCard card = playerDeck.DrawPlayerCard();

                if (card != null)
                {
                    player.DrawCard(card);

                    // CardUIManager cardUIManager = FindAnyObjectByType<CardUIManager>();

                    // if (cardUIManager != null && card.City != null)
                    // {
                    //     cardUIManager.ShowPlayerCard(card);
                    // }
                }
            }
        }
    }

    int GetStartingCardCount()
    {
        if (players.Count == 2)
            return 4;

        if (players.Count == 3)
            return 3;

        if (players.Count == 4)
            return 2;

        return 4; // for 1 player testing
    }

    public void TestPlayer()
    {
        GameObject playerObject = new GameObject("Player " + (1));
        Player player = playerObject.AddComponent<Player>();
            
        player.Initialize("Player 0", board.cityLookup["Atlanta"], board);

        players.Add(player);
    }

    public void UpdateActionDisplay()
    {
        playerInfo.OnAction();
        if (actionText != null)
        {
            actionText.text = $"Actions: {actionCount}";
        }
        
        board.ShowDiseasedCities();
    }

    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            // Optional: clear after a few seconds
            CancelInvoke(nameof(ClearMessage));
            Invoke(nameof(ClearMessage), 3f);
        }
        Debug.Log(message);
    }

    private void ClearMessage()
    {
        if (messageText != null)
            messageText.text = "";
    }

    public enum Difficulty
    {
        Introductory,
        Standard,
        Heroic
    }
}