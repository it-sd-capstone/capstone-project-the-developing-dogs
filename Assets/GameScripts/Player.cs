using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Player state info
    public string PlayerName { get; private set; }
    public City CurrentCity { get; private set; }
    public Role Role { get; private set; }
    public string RoleName { get; set; }
    public GameBoard board;

    //Player hand info
    public List<PlayerCard> Hand { get; private set; } = new List<PlayerCard>();
    public const int MaxHandSize = 7;

    public void SetPlayerName(string name)
    {
        PlayerName = name;
    }

    //Sets up the player at the start of the game
    public void Initialize(string name, City startingCity, GameBoard gameBoard)
    {
        board = gameBoard;
        PlayerName = name;
        CurrentCity = startingCity;
    }

    //Assigns a role to the player and lets the role know who owns it
    public void SetRole(Role role)
    {
        Role = role;
        Role?.Initialize(this);
    }

    //Moves the player to a new city
    public void MoveTo(City destination)
    {
        CurrentCity = destination;
    }

    //Treats disease cubes in the player's current city
    public void TreatDisease(DiseaseColor color)
    {
        int cubesToRemove = 1;

        Role?.OnTreatDisease(CurrentCity, color, ref cubesToRemove);

        for (int i = cubesToRemove; i > 0; i--)
            board.RemoveDisease(CurrentCity, color);
    }

    //Checks if the player has enough matching cards to discover a cure
    public bool TryDiscoverCure(GameBoard board, DiseaseColor color)
    {
        int requiredCards = Role?.CardsRequiredForCure ?? 5;

        int matchingCards = 0;
        foreach (var card in Hand)
        {
            if (card.Color == color)
                matchingCards++;
        }

        if (matchingCards < requiredCards)
            return false;

        return true;
    }

    //Checks if this player can give a specific card to another player
    public bool CanGiveCardTo(Player other, PlayerCard card)
    {
        if (CurrentCity != other.CurrentCity)
            return false;

        if (Role?.CanGiveAnyCard == true)
            return true;

        return card.City == CurrentCity;
    }

    //Gives a card to another player if the rules allow it
    public bool GiveCardTo(Player other, PlayerCard card)
    {
        if (!Hand.Contains(card))
            return false;

        if (!CanGiveCardTo(other, card))
            return false;

        Hand.Remove(card);
        other.Hand.Add(card);
        return true;
    }

    //Builds a research station at the player's current city
    public bool BuildResearchStation(GameBoard board)
    {
        if (Role?.CanBuildStationForFree == true)
        {
            board.BuildResearchStation(CurrentCity);
            return true;
        }

        PlayerCard cityCard = Hand.Find(c => c.City == CurrentCity);

        if (cityCard == null)
            return false;

        Hand.Remove(cityCard);
        board.BuildResearchStation(CurrentCity);
        return true;
    }

    //Adds a card to the player's hand
    public void DrawCard(PlayerCard card)
    {
        Hand.Add(card);
    }

    //Removes a card from the player's hand if they have it
    public void DiscardCard(PlayerCard card)
    {
        if (Hand.Contains(card))
            Hand.Remove(card);
    }

    //Uses the player's role ability
    public void UseRoleAbility(GameBoard board)
    {
        Role?.UseSpecialAbility(board);
    }

    public bool CanFly(Player p, City city)
    {
        foreach (PlayerCard card in p.Hand)
        {
            if (card.City == p.CurrentCity) return true;
            if (card.City == city) return true;
        }
        return false;
    }
}
