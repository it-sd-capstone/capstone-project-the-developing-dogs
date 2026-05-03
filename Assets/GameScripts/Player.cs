using System.Collections.Generic;
using UnityEngine;

// Represents a player in the game.
// This class handles the player's city, role, cards, and main player actions.
public class Player : MonoBehaviour
{
    // Player state information.
    public string PlayerName { get; private set; }
    public City CurrentCity { get; private set; }
    public Role Role { get; private set; }
    public GameBoard board;

    // Player hand information.
    public List<PlayerCard> Hand { get; private set; } = new List<PlayerCard>();
    public const int MaxHandSize = 7;

    // Sets up the player at the start of the game.
    public void Initialize(string name, City startingCity, Role role, GameBoard gameBoard)
    {
        board = gameBoard;
        PlayerName = name;
        CurrentCity = startingCity;
        SetRole(role);
    }

    // Assigns a role to the player and lets the role know who owns it.
    public void SetRole(Role role)
    {
        Role = role;
        Role?.Initialize(this);
    }

    // Moves the player to a new city.
    public void MoveTo(City destination)
    {
        // GameManager should validate the move rules before calling this.
        CurrentCity = destination;
    }

    // Treats disease cubes in the player's current city.
    public void TreatDisease(DiseaseColor color)
    {
        int cubesToRemove = 1;

        // Allow the role to change how many cubes are removed.
        Role?.OnTreatDisease(CurrentCity, color, ref cubesToRemove);

        for(int i = cubesToRemove; i >= 0; i--)
        board.RemoveDisease(CurrentCity, color);
    }

    // Checks if the player has enough matching cards to discover a cure.
    public bool TryDiscoverCure(GameBoard board, DiseaseColor color)
    {
        int requiredCards = Role?.CardsRequiredForCure ?? 5;

        // Count matching cards in the player's hand.
        int matchingCards = 0;
        foreach (var card in Hand)
        {
            if (card.Color == color)
                matchingCards++;
        }

        if (matchingCards < requiredCards)
            return false;

        // GameManager should handle removing cards and marking the cure.
        return true;
    }

    // Checks if this player can give a specific card to another player.
    public bool CanGiveCardTo(Player other, PlayerCard card)
    {
        // Players must be in the same city to share knowledge.
        if (this.CurrentCity != other.CurrentCity)
            return false;

        // Researcher can give any card.
        if (Role?.CanGiveAnyCard == true)
            return true;

        // Default rule: only give the card that matches the city you are in.
        return card.City == this.CurrentCity;
    }

    // Gives a card to another player if the rules allow it.
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

    // Builds a research station at the player's current city.
    public bool BuildResearchStation(GameBoard board)
    {
        // Operations Expert can build without discarding a city card.
        if (Role?.CanBuildStationForFree == true)
        {
            board.BuildResearchStation(CurrentCity);
            return true;
        }

        // Default rule: discard the matching city card first.
        PlayerCard cityCard = Hand.Find(c => c.City == CurrentCity);

        if (cityCard == null)
            return false;

        Hand.Remove(cityCard);
        board.BuildResearchStation(CurrentCity);
        return true;
    }

    // Adds a card to the player's hand.
    public void DrawCard(PlayerCard card)
    {
        Hand.Add(card);

        // GameManager should enforce discarding if the hand goes over 7 cards.
    }

    // Removes a card from the player's hand if they have it.
    public void DiscardCard(PlayerCard card)
    {
        if (Hand.Contains(card))
            Hand.Remove(card);
    }

    // Uses the player's role ability.
    public void UseRoleAbility(GameBoard board)
    {
        Role?.UseSpecialAbility(board);
    }
}
