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
                .Select((x, index) => new Card(x.Card, -1))
        );

        return currentDeck;
    }

    public void DrawHand()
    {
        hand.Clear();
        for (int i = 0; i < handSize; i++)
        {
            Card card = currentDeck.Dequeue();
            card.Index = i;
            hand.Add(card);
        }
    }

    public Card PlayCard(int cardId)
    {
        //int i = Mathf.Clamp(index, 0, hand.Count - 1);
        //Card card = hand[i];
        //hand.RemoveAt(i);
        Card foundCard = hand.ToList().FirstOrDefault(card => card.Index == cardId);
        return foundCard;
    }

    public Card PlayCardMaxLength(int maxLength)
    {
        Card card = hand.ToList().FirstOrDefault(card => card.Actions.Count <= maxLength);
        return card;
    }


    public Card PlaySkip(int skipAmount)
    {
        Card skipCard = new Card(-1);
        for (int i = 0; i < skipAmount; i++)
        {
            skipCard.Actions.Add(new CardAction { ActionType = CardActionType.Wait, ActionAmount = 1 });
        }

        return skipCard;
    }

    public bool HasPlayableCard(int remainingTime)
    {
        bool hasPlayable = false;
        foreach (Card c in hand)
        {
            hasPlayable |= c.Actions.Count <= remainingTime;
        }

        return hasPlayable;
    }
}
