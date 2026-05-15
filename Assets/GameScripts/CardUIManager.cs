using System.Collections.Generic;
using UnityEngine;

public class CardUIManager : MonoBehaviour
{
    [Header("Card Prefabs")]
    public GameObject playerCardPrefab;
    public GameObject infectionCardPrefab;

    [Header("UI Areas")]
    public Transform currentHandArea;
    public Transform playerDiscardArea;
    public Transform infectionDiscardArea;

    public void ShowPlayerCard(PlayerCard card)
    {
        GameObject cardObject = Instantiate(playerCardPrefab, currentHandArea);

        CardUI cardUI = cardObject.GetComponent<CardUI>();
        cardUI.Setup(card);
    }

    public void ClearHand()
    {
        foreach (Transform child in currentHandArea.transform)
        {
            Destroy(child);
        }
    }

    public void ShowInfectionCard(InfectionCard card)
    {
        GameObject cardObject = Instantiate(infectionCardPrefab, infectionDiscardArea);

        CardUI cardUI = cardObject.GetComponent<CardUI>();
        cardUI.Setup(card.City.cityName, null);
    }
}