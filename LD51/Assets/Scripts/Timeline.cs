using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class Timeline
{
    private const int length = 10;
    private int currentStep = 0;
    private List<CardAction> actions = new();
    private List<Card> cards = new();
    private CardActionResolver targetResolver;
    private CardActionResolver selfResolver;

    public Timeline(CardActionResolver self)
    {
        Reset();
        selfResolver = self;
    }

    public void AddCard(Card card)
    {
        actions.AddRange(card.Actions);
        cards.Add(card);
    }

    public void Reset()
    {
        actions.Clear();
        currentStep = 0;
    }

    public void ResolveActions(CardActionType type)
    {
        CardAction action = actions[currentStep];
        if (action.ActionType == type)
        {
            Debug.Log($"Resolving {action.ActionType} for {selfResolver.gameObject.name}");
            selfResolver.ResolveSelfAction(action);
            targetResolver.ResolveTargetAction(action);
        }
    }

    public void ResetTurnEffects()
    {
        currentStep++;
        selfResolver.ResetTurnEffects();
    }

    public CardAction GetCurrentAction()
    {
        return actions.Count > currentStep ? actions[currentStep] : null;
    }

    public int GetRemainingTime()
    {
        return length - currentStep;
    }

    public void SetTargetResolver(CardActionResolver target)
    {
        targetResolver = target;
    }

    public void SetSelfResolver(CardActionResolver self)
    {
        selfResolver = self;
    }
}
