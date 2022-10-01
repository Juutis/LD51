using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeline
{
    private const int length = 10;
    private int currentStep = 0;
    private List<CardAction> playerActions = new();
    private List<CardAction> enemyActions = new();
    private List<Card> playerCards = new();
    private List<Card> enemyCards = new();

    public Timeline()
    {
        Reset();
    }

    public void AddCard(Card card, bool isPlayer)
    {
        if (isPlayer)
        {
            playerActions.AddRange(card.Actions);
            playerCards.Add(card);
        }
        else
        {
            enemyActions.AddRange(card.Actions);
            enemyCards.Add(card);
        }
    }

    public void Reset()
    {
        playerActions.Clear();
        enemyActions.Clear();
        currentStep = 0;
    }

    public void ResolveActions()
    {
        CardAction playerAction = playerActions[currentStep];
        CardAction enemyAction = enemyActions[currentStep];
        Debug.Log($"Resolving {playerAction.ActionType} for player");
        // Debug.Log($"Resolving {enemyAction.ActionType} for enemy");
        currentStep++;
    }

    public CardAction GetPlayerCurrentAction()
    {
        return playerActions.Count > currentStep ? playerActions[currentStep] : null;
    }

    public CardAction GetEnemyCurrentAction()
    {
        return enemyActions.Count > currentStep ? enemyActions[currentStep] : null;
    }

    public int GetRemainingTime()
    {
        return length - currentStep - 1;
    }

    public int GetPlayerRemainingTime()
    {
        return length - playerActions.Count;
    }

    public int GetEnemyRemainingTime()
    {
        return length - enemyActions.Count;
    }
}
