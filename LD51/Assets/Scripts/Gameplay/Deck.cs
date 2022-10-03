using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    private List<CardCount> deck = new();
    private Queue<Card> currentDeck = new();
    private List<Card> hand = new();
    private const int handSize = 5;

    void Start() {
        var sum = 0;
        foreach(var cardAmount in deck) {
            sum += cardAmount.count;
        }
        if (sum % 5 != 0) {
            Debug.LogError("Deck size not divisible by 5: " + sum, gameObject);
        }
    }

    public List<Card> GetHand()
    {
        return hand;
    }

    public Queue<Card> GetCurrentDeck()
    {
        return currentDeck;
    }

    public Queue<Card> ShuffleNewDeck()
    {
        var cards = new List<Card>();
        foreach(var cardAmount in deck) {
            for(var i = 0; i < cardAmount.count; i++) {
                cards.Add(new Card(cardAmount.card.Card, -1));
            }
        }

        currentDeck = new Queue<Card>(
            cards.OrderBy(x => Random.Range(0, deck.Count))
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
        hand.Remove(foundCard);
        return foundCard;
    }

    public Card PlayCardMaxLength(int maxLength)
    {
        Card card = hand.ToList().FirstOrDefault(card => card.Actions.Count <= maxLength);
        hand.Remove(card);
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

[System.Serializable]
public struct CardCount{
    public CardConfig card;
    public int count;
}
