using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //Player state
    public string PlayerName { get; private set; }
    public City CurrentCity { get; private set; }
    public Role Role { get; private set; }

    //Player hand
    public List<PlayerCard> Hand { get; private set; } = new List<PlayerCard>();
    public const int MaxHandSize = 7;

    //Initialize
    public void Initialize(string name, City startingCity, Role role)
    {
        PlayerName = name;
        CurrentCity = startingCity;
        SetRole(role);
    }

    public void SetRole(Role role)
    {
        Role = role;
        Role.Initialize(this);
    }

    //Movement

    public void MoveTo(City destination)
    {
        // GameManager should validate the move rules before calling this
        CurrentCity = destination;
    }

    //Treating Disease

    public void TreatDisease(DiseaseColor color)
    {
        int cubesToRemove = 1;

        // Allow role to modify the number of cubes removed
        Role?.OnTreatDisease(CurrentCity, color, ref cubesToRemove);

        CurrentCity.RemoveCubes(color, cubesToRemove);
    }

    //Discovering cure

    public bool TryDiscoverCure(Board board, DiseaseColor color)
    {
        int requiredCards = Role.CardsRequiredForCure;

        // Count matching cards
        int matchingCards = 0;
        foreach (var card in Hand)
        {
            if (card.Color == color)
                matchingCards++;
        }

        if (matchingCards < requiredCards)
            return false;

        // GameManager should handle removing cards and marking cure
        return true;
    }

    //Sharing knowledge

    public bool CanGiveCardTo(Player other, PlayerCard card)
    {
        // Must be in same city
        if (this.CurrentCity != other.CurrentCity)
            return false;

        // Researcher can give ANY card
        if (Role.CanGiveAnyCard)
            return true;

        // Default only give the card of the city you're in
        return card.City == this.CurrentCity;
    }

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

    //Building research station

    public bool BuildResearchStation(Board board)
    {
        // Operations Expert can build without discarding
        if (Role.CanBuildStationForFree)
        {
            board.BuildResearchStation(CurrentCity);
            return true;
        }

        // Default discard the city card
        PlayerCard cityCard = Hand.Find(c => c.City == CurrentCity);

        if (cityCard == null)
            return false;

        Hand.Remove(cityCard);
        board.BuildResearchStation(CurrentCity);
        return true;
    }

    //Drawing/discarding

    public void DrawCard(PlayerCard card)
    {
        Hand.Add(card);

        // GameManager should enforce discarding if > 7 cards
    }

    public void DiscardCard(PlayerCard card)
    {
        if (Hand.Contains(card))
            Hand.Remove(card);
    }

    //Special ability role

    public void UseRoleAbility(Board board)
    {
        Role?.UseSpecialAbility(board);
    }
}
