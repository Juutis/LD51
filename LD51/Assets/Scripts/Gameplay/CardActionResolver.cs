using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CardActionResolver : MonoBehaviour
{
    private Dictionary<CardActionType, IActionResolver> resolvers;

    public void Start()
    {
        resolvers = new();
        foreach (IActionResolver resolver in GetComponents<IActionResolver>())
        {
            resolvers[resolver.GetActionType()] = resolver;
        }
    }

    public CardEffect ResolveSelfAction(CardAction action)
    {
        if (resolvers.ContainsKey(action.ActionType))
        {
            return resolvers[action.ActionType].ResolveSelfAction(action.ActionAmount);
        }

        return CardEffect.None;
    }

    public CardEffect ResolveTargetAction(CardAction action)
    {
        if (resolvers.ContainsKey(action.ActionType))
        {
            return resolvers[action.ActionType].ResolveTargetAction(action.ActionAmount);
        }

        return CardEffect.None;
    }

    public void ResetTurnEffects()
    {
        foreach(var resolver in resolvers.Values)
        {
            resolver.ResetTurnEffects();
        }
    }
}