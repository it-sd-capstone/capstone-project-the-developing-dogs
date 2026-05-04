using System.Collections.Generic;
using UnityEngine;

public class InfectionDeck : MonoBehaviour
{
    private Stack<InfectionCard> drawPile = new Stack<InfectionCard>();
    private List<InfectionCard> discardPile = new List<InfectionCard>();

    public void Initialize(List<City> cities)
    {
        drawPile.Clear();
        discardPile.Clear();

        //Create one infection card per city
        List<InfectionCard> temp = new List<InfectionCard>();
        foreach (City c in cities)
            temp.Add(new InfectionCard(c, c.diseaseColor));


        Shuffle(temp);

        //Load into stack
        foreach (var card in temp)
            drawPile.Push(card);
    }

    public InfectionCard DrawInfectionCard()
    {
        if (drawPile.Count == 0)
            ReshuffleDiscardIntoDraw();

        InfectionCard card = drawPile.Pop();
        discardPile.Add(card);
        return card;
    }

    public InfectionCard DrawBottomCard()
    {
        if (drawPile.Count == 0)
            ReshuffleDiscardIntoDraw();

        //Convert to list to access bottom
        List<InfectionCard> list = new List<InfectionCard>(drawPile);
        InfectionCard bottom = list[0];

        //Remove bottom
        list.RemoveAt(0);

        //Rebuild stack
        drawPile = new Stack<InfectionCard>(list);

        discardPile.Add(bottom);
        return bottom;
    }

    public void Intensify()
    {
        //Shuffle discard pile and place on top of draw pile
        Shuffle(discardPile);

        foreach (var card in discardPile)
            drawPile.Push(card);

        discardPile.Clear();
    }

    private void ReshuffleDiscardIntoDraw()
    {
        Shuffle(discardPile);
        foreach (var card in discardPile)
            drawPile.Push(card);
        discardPile.Clear();
    }

    private void Shuffle(List<InfectionCard> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
}