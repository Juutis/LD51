using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    private List<CardConfig> deck = new();
    private Queue<Card> currentDeck = new();
    private List<Card> hand = new();
    private const int handSize = 5;

    public List<Card> GetHand()
    {
        return hand;
    }

    public List<CardConfig> GetDeck()
    {
        return deck;
    }

    public Queue<Card> GetCurrentDeck()
    {
        return currentDeck;
    }

    public Queue<Card> ShuffleNewDeck()
    {
        currentDeck = new Queue<Card>(
            deck
                .OrderBy(x => Random.Range(0, deck.Count))
                .Select(x => new Card(x.Card))
        );

        return currentDeck;
    }

    public void DrawHand()
    {
        for (int i = 0; i < handSize; i++)
        {
            hand.Add(currentDeck.Dequeue());
        }
    }

    public Card PlayCard(int index)
    {
        int i = Mathf.Clamp(index, 0, hand.Count - 1);
        Card card = hand[i];
        hand.RemoveAt(i);
        return card;
    }
}
