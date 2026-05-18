using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    private Stack<PlayerCard> drawPile = new Stack<PlayerCard>();
    private List<PlayerCard> discardPile = new List<PlayerCard>();

    public int Count => drawPile.Count;
    public int DiscardCount => discardPile.Count;

    public void Initialize(List<PlayerCard> allCards)
    {
        drawPile.Clear();
        discardPile.Clear();

        // Shuffle the initial deck
        Shuffle(allCards);

        // Load into stack (top of stack = end of list)
        foreach (var card in allCards)
            drawPile.Push(card);
    }

    public PlayerCard DrawPlayerCard()
    {
        if (drawPile.Count == 0)
            ReshuffleDiscardIntoDraw();

        if (drawPile.Count == 0)
            return null;

        PlayerCard card = drawPile.Pop();
        discardPile.Add(card);
        return card;
    }

    public void InsertEpidemicCards(int epidemicCount)
    {
        if (epidemicCount <= 0)
            return;

        // Convert draw pile to list (top of stack should be end of list)
        List<PlayerCard> cards = new List<PlayerCard>(drawPile);
        drawPile.Clear();

        // Create epidemic cards
        List<PlayerCard> epidemics = new List<PlayerCard>();
        for (int i = 0; i < epidemicCount; i++)
            epidemics.Add(new EpidemicCard());

        // Prepare piles
        List<List<PlayerCard>> piles = new List<List<PlayerCard>>();
        for (int i = 0; i < epidemicCount; i++)
            piles.Add(new List<PlayerCard>());

        // Distribute cards as evenly as possible (round‑robin)
        int pileIndex = 0;
        foreach (var card in cards)
        {
            piles[pileIndex].Add(card);
            pileIndex = (pileIndex + 1) % epidemicCount;
        }

        // Add one epidemic to each pile and shuffle each pile
        for (int i = 0; i < epidemicCount; i++)
        {
            piles[i].Add(epidemics[i]);
            Shuffle(piles[i]);
        }

        // Optional: shuffle pile order to randomize stack order further
        Shuffle(piles);

        // Rebuild draw pile: last pile added ends up on top
        for (int i = 0; i < piles.Count; i++)
        {
            foreach (var card in piles[i])
                drawPile.Push(card);
        }
    }

    private void ReshuffleDiscardIntoDraw()
    {
        if (discardPile.Count == 0)
            return;

        Shuffle(discardPile);
        foreach (var card in discardPile)
            drawPile.Push(card);
        discardPile.Clear();
    }

    private void Shuffle(List<PlayerCard> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    // Overload to shuffle list of lists (for piles)
    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    public void Discard(PlayerCard card)
    {
        if (card != null)
            discardPile.Add(card);
    }
}
