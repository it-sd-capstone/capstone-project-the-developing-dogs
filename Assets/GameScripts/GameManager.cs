using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Core Systems")]
    public GameBoard board;
    public InfectionDeck infectionDeck;
    public PlayerDeck playerDeck;
    public List<Player> players = new List<Player>();

    [Header("Difficulty")]
    public Difficulty difficulty = Difficulty.Standard;

    public int currentPlayerIndex = 0;
    public int infectionRateIndex = 0;
    private int curesFound = 0;
    public int actionCount;

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
            
            player.Initialize($"Player {i}", board.cityLookup["Atlanta"], board);

            players.Add(player);
        }
    }

    void SetupGame()
    {
        SetupPlayers();

        List<PlayerCard> allCards = CreateAllPlayerCards();

        playerDeck.Initialize(allCards);
        playerDeck.InsertEpidemicCards(GetEpidemicCount());

        DealStartingCards();

        infectionDeck.Initialize(board.cities);
        SetupInitialInfections();
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

        actionCount = 4;
    }

    public void EndTurn()
    {
        if (actionCount > 0) return;
        if (players == null || players.Count == 0)
        {
            Debug.LogError("Cannot end turn because no players exist.");
            return;
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

    public enum Difficulty
    {
        Introductory,
        Standard,
        Heroic
    }
}