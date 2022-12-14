using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Timeline
{
    private const int length = 10;
    private int currentStep = 0;
    private List<CardAction> actions = new();
    private List<Card> cards = new();
    private CardActionResolver targetResolver;
    private CardActionResolver selfResolver;
    private TimelineType timelineType;

    public TimelineType Type { set { timelineType = value; } get { return timelineType; } }

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


    public CardEffectInContext ResolveActions(CardActionType type)
    {
        CardEffectInContext noneEffect = new CardEffectInContext();
        noneEffect.Effect = CardEffect.None;
        noneEffect.Type = Type;
        noneEffect.Amount = 0;
        if (currentStep >= actions.Count)
        {
            return noneEffect;
        }
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
                    Debug.Log(string.Join(", ", actions.Select(x => x.ActionType)));
                    CardAction stunAction = new() { ActionAmount = 1, ActionType = CardActionType.Stunned };
                    if (actions.Count <= currentStep + 1)
                    {
                        Card stunCard = new(-1) { Actions = new() { stunAction } };
                        actions.Add(stunAction);
                        cards.Add(stunCard);
                        if (timelineType == TimelineType.Enemy)
                        {
                            UITimelineBar.main.CreateEnemyCard(UICardManager.main.ConvertCardData(stunCard, -1));
                        }
                        else
                        {
                            UITimelineBar.main.CreatePlayerCard(UICardManager.main.ConvertCardData(stunCard, -1));
                        }
                    }
                    else
                    {
                        UITimelineCard uiCard;
                        if (timelineType == TimelineType.Enemy)
                        {
                            uiCard = UITimelineBar.main.GetLastEnemyCard();
                        }
                        else
                        {
                            uiCard = UITimelineBar.main.GetLastPlayerCard();
                        }

                        for (int i = currentStep; i < actions.Count; i++)
                        {
                            actions[i].ActionType = CardActionType.Stunned;
                            actions[i].ActionAmount = 1;
                        }

                        // TODO: Mark stun on only the future actions
                        Sprite stunIcon = UICardManager.main.GetActionIcon(CardActionType.Stunned)?.Sprite;
                        uiCard.Actions.ForEach(x => x.SetImage(stunIcon));
                        uiCard.Actions.ForEach(x => x.SetDataType(CardActionType.Stunned));
                    }

                    Debug.Log(string.Join(", ", actions.Select(x => x.ActionType)));
                }
            }
            CardEffectInContext effect = new CardEffectInContext();
            if (fromTargetEffect != CardEffect.None)
            {
                effect.Effect = fromTargetEffect;
            }
            else
            {
                effect.Effect = fromSelfEffect;
            }
            effect.Type = Type;
            effect.Amount = action.ActionAmount;
            return effect;
        }
        return noneEffect;
    }

    public bool SkipForward()
    {
        if (currentStep >= actions.Count)
        {
            return false;
        }
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

    public int GetRemainingActions()
    {
        return actions.Count - currentStep;
    }
}


public enum TimelineType
{
    Player,
    Enemy
}

public class CardEffectInContext
{
    public int Amount;
    public CardEffect Effect;
    public TimelineType Type;
}