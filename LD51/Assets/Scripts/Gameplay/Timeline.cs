using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public CardEffect ResolveActions(CardActionType type)
    {
        CardAction action = actions[currentStep];
        if (action.ActionType == type)
        {
            Debug.Log($"Resolving {action.ActionType} for {selfResolver.gameObject.name}");
            CardEffect fromSelfEffect = selfResolver.ResolveSelfAction(action); // TODO: fromSelfEffect not used
            CardEffect fromTargetEffect = targetResolver.ResolveTargetAction(action);

            if (fromTargetEffect == CardEffect.Stun)
            {
                if (currentStep < length)
                {
                    CardAction stunAction = new() { ActionAmount = 1, ActionType = CardActionType.Stunned };
                    actions[currentStep+1] = stunAction;
                }
            }

            if (fromTargetEffect == CardEffect.Killed)
            {
                return CardEffect.Killed;
            }
        }

        return CardEffect.None;
    }

    public bool SkipForward()
    {
        CardAction action = actions[currentStep];
        if (action.ActionType == CardActionType.SkipForward)
        {
            Debug.Log($"Resolving {action.ActionType} for {selfResolver.gameObject.name}");
            Mathf.Min(currentStep + action.ActionAmount, length);
            // TODO: Fill timeline slots with none action
            return true;
        }

        return false;
    }

    public void SyncCurrentStep(int newStep)
    {
        // TODO: Fill timeline slots with none action
        currentStep = newStep;
    }

    public void ResetTurnEffects()
    {
        Debug.Log("Current timeline: " + string.Join(",", actions.Select(x => x.ActionType)));
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

    public int GetCurrentStep()
    {
        return currentStep;
    }
}
