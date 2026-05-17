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

    private Dictionary<PlayerCard, GameObject> cardGameObjects = new Dictionary<PlayerCard, GameObject>();

    public void ShowPlayerCards(Player player)
    {
        ClearHand();
        foreach (PlayerCard card in player.Hand)
        {
            ShowPlayerCard(card);
        }
    }

    public void ShowPlayerCard(PlayerCard card)
    {
        GameObject cardObject = Instantiate(playerCardPrefab, currentHandArea);

        CardUI cardUI = cardObject.GetComponent<CardUI>();
        cardUI.Setup(card);

        cardGameObjects[card] = cardObject;
    }

    public void DiscardCard(PlayerCard card)
    {
        if (currentHandArea == null) return;

        if(cardGameObjects.TryGetValue(card, out GameObject cardObj))
        {
            Destroy(cardObj);
        }
    }

    public void ClearHand()
    {
        if (currentHandArea == null) return;

        while (currentHandArea.childCount > 0)
        {
            Transform child = currentHandArea.GetChild(0);
            child.SetParent(null); // Detach first
            Destroy(child.gameObject);
        }
    }

    public void ShowInfectionCard(InfectionCard card)
    {
        GameObject cardObject = Instantiate(infectionCardPrefab, infectionDiscardArea);

        CardUI cardUI = cardObject.GetComponent<CardUI>();
        cardUI.Setup(card.City.cityName, null);
    }
}