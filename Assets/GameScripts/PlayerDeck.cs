using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    private Stack<PlayerCard> drawPile = new Stack<PlayerCard>();
    private List<PlayerCard> discardPile = new List<PlayerCard>();

    public void Initialize(List<PlayerCard> allCards)
    {
        drawPile.Clear();
        discardPile.Clear();

        //Shuffle the initial deck
        Shuffle(allCards);

        //Load into stack
        foreach (var card in allCards)
            drawPile.Push(card);
    }

    //Drawing card

    public PlayerCard DrawPlayerCard()
    {
        if (drawPile.Count == 0)
            ReshuffleDiscardIntoDraw();

        PlayerCard card = drawPile.Pop();
        discardPile.Add(card);
        return card;
    }

    //Epidemic cards inserted

    public void InsertEpidemicCards(int epidemicCount)
    {
        //Convert draw pile to list
        List<PlayerCard> cards = new List<PlayerCard>(drawPile);
        drawPile.Clear();

        //Create epidemic cards
        List<PlayerCard> epidemics = new List<PlayerCard>();
        for (int i = 0; i < epidemicCount; i++)
            epidemics.Add(new EpidemicCard());

        //Split deck into equal piles
        List<List<PlayerCard>> piles = new List<List<PlayerCard>>();
        int pileSize = cards.Count / epidemicCount;

        int index = 0;
        for (int i = 0; i < epidemicCount; i++)
        {
            List<PlayerCard> pile = new List<PlayerCard>();

            //Fill pile
            for (int j = 0; j < pileSize && index < cards.Count; j++)
            {
                pile.Add(cards[index]);
                index++;
            }

            //Add one epidemic to each pile
            pile.Add(epidemics[i]);

            //Shuffle pile
            Shuffle(pile);

            piles.Add(pile);
        }

        //Combine piles back into draw pile (stack)
        for (int i = piles.Count - 1; i >= 0; i--)
        {
            foreach (var card in piles[i])
                drawPile.Push(card);
        }
    }

    //Discard and reshuffle

    private void ReshuffleDiscardIntoDraw()
    {
        Shuffle(discardPile);
        foreach (var card in discardPile)
            drawPile.Push(card);
        discardPile.Clear();
    }

    //Shuffle

    private void Shuffle(List<PlayerCard> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}
