using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Card
{
    public List<CardAction> Actions;
    public Sprite CardArt;

    public int Index;

    public Card(int index)
    {
        Actions = new List<CardAction>();
        Index = index;
    }
    public Card(Card card, int index)
    {
        Index = index;
        Actions = new(card.Actions.Select(x => x.Clone()));
        CardArt = card.CardArt;
    }
}

[Serializable]
public class CardAction
{
    public CardActionType ActionType;
    public int ActionAmount; // heal, damage or rewind steps

    public CardAction Clone()
    {
        return new CardAction { ActionAmount = ActionAmount, ActionType = ActionType };
    }
}

public enum CardActionType
{
    Attack,
    Defend,
    Wait,
    Heal,
    Parry,
    SkipForward,
    Rewind,
    Stunned,
    None // No action, shouldn't be in cards
}
