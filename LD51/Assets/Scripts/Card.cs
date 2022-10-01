using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

[Serializable]
public class Card
{
    public List<CardAction> Actions;
    public Sprite CardArt;

    public Card() { }
    public Card(Card card)
    {
        Actions = new (card.Actions);
        CardArt = card.CardArt;
    }
}

[Serializable]
public class CardAction
{
    public CardActionType ActionType;
    public int ActionAmount; // heal, damage or rewind steps
}

public enum CardActionType
{
    Attack,
    Defend,
    Wait,
    Heal,
    None // No action, shouldn't be in cards
}
