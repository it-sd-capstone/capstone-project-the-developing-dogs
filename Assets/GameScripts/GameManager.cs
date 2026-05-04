using System.Collections.Generic;
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

    private int currentPlayerIndex = 0;
    private int infectionRateIndex = 0;
    private int curesFound = 0;

    private readonly int[] infectionRateTrack = { 2, 2, 2, 3, 3, 4, 4 };

    void Start()
    {
        SetupGame();
        StartTurn();
    }

    void SetupGame()
    {
        
        List<PlayerCard> allCards = CreateAllPlayerCards();
        playerDeck.Initialize(allCards);
        playerDeck.InsertEpidemicCards(GetEpidemicCount());
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

    private void SetupInitialInfections()
    {
        //3 cities with 3 cubes
        for (int i = 0; i < 3; i++)
        {
            var card = infectionDeck.DrawInfectionCard();
            board.InfectCity(card.City, card.Color);
        }

        //3 cities with 2 cubes
        for (int i = 0; i < 3; i++)
        {
            var card = infectionDeck.DrawInfectionCard();
            board.InfectCity(card.City, card.Color);
        }

        //3 cities with 1 cube
        for (int i = 0; i < 3; i++)
        {
            var card = infectionDeck.DrawInfectionCard();
            board.InfectCity(card.City, card.Color);
        }
    }

    //Turn system

    void StartTurn()
    {
        Player current = players[currentPlayerIndex];
        Debug.Log("Starting turn for: " + current.PlayerName);
    }

    public void EndTurn()
    {
        DrawPlayerCards();
        InfectionPhase();

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        StartTurn();
    }

    //Player card draw

    private void DrawPlayerCards()
    {
        Player current = players[currentPlayerIndex];

        for (int i = 0; i < 2; i++)
        {
            PlayerCard card = playerDeck.DrawPlayerCard();

            if (card is EpidemicCard)
            {
                HandleEpidemic();
            }
            else
            {
                current.DrawCard(card);
            }
        }
    }

    //Epidemic

    private void HandleEpidemic()
    {
        infectionRateIndex = Mathf.Min(infectionRateIndex + 1, infectionRateTrack.Length - 1);

        InfectionCard bottom = infectionDeck.DrawBottomCard();
        board.InfectCity(bottom.City, bottom.Color);

        infectionDeck.Intensify();
    }

    //Infection

    private void InfectionPhase()
    {
        int infections = infectionRateTrack[infectionRateIndex];

        for (int i = 0; i < infections; i++)
        {
            InfectionCard card = infectionDeck.DrawInfectionCard();
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
}

public enum Difficulty
{
    Introductory,
    Standard,
    Heroic
}
