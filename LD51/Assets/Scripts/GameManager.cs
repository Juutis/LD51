using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    private GameState currentGameState = GameState.ShuffleHand;
    [SerializeField]
    private List<CardConfig> playerDeck = new();
    [SerializeField]
    private List<CardConfig> enemyDeck = new();
    [SerializeField]
    private CardActionResolver playerActionResolver;
    [SerializeField]
    private CardActionResolver enemyActionResolver;

    private Queue<Card> playerCurrentDeck = new();
    private Queue<Card> enemyCurrentDeck = new();

    private List<Card> playerHand = new();
    private List<Card> enemyHand = new();

    private const int handSize = 5;

    private Timeline playerTimeline;
    private Timeline enemyTimeline;

    void Start()
    {
        enemyTimeline = new(enemyActionResolver);
        playerTimeline = new(playerActionResolver);
        playerTimeline.SetTargetResolver(enemyActionResolver);
        enemyTimeline.SetTargetResolver(playerActionResolver);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.NewEnemy)
        {
            // TODO: enemyTimeline = new();
            playerTimeline.Reset();
            // heal player a bit
            currentGameState = GameState.ShuffleHand;
        }
        else if (currentGameState == GameState.PlayCard)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (playerTimeline.GetCurrentAction() == null)
                {
                    Debug.Log("Play cards");
                    Card card = playerHand[0];
                    playerHand.RemoveAt(0);

                    playerTimeline.AddCard(card);
                }

                if (enemyTimeline.GetCurrentAction() == null)
                {
                    Card card = enemyHand[0];
                    enemyHand.RemoveAt(0);

                    enemyTimeline.AddCard(card);
                }
                currentGameState = GameState.ResolveAction;
            }

        }
        else if (currentGameState == GameState.ResolveAction)
        {
            // TODO: Make time forward (and backward?) skip return boolean so it can skip the turn immediately
            playerTimeline.ResolveActions(CardActionType.Heal);
            enemyTimeline.ResolveActions(CardActionType.Heal);
            playerTimeline.ResolveActions(CardActionType.Defend);
            enemyTimeline.ResolveActions(CardActionType.Defend);
            playerTimeline.ResolveActions(CardActionType.Attack);
            enemyTimeline.ResolveActions(CardActionType.Attack);
            currentGameState = GameState.ResetTurnEffects;
        }
        else if (currentGameState == GameState.ResetTurnEffects)
        {
            // TODO: Make something like GetLateActionEffects() to return a thing so something like parry can stun a character next round
            playerTimeline.ResetTurnEffects();
            enemyTimeline.ResetTurnEffects();
            if (playerTimeline.GetRemainingTime() == 0)
            {
                currentGameState = GameState.ShuffleHand;
            }
            else
            {
                currentGameState = GameState.PlayCard;
            }
        }
        else // shuffle
        { 
            if (playerCurrentDeck.Count == 0)
            {
                playerCurrentDeck = new Queue<Card>(
                    playerDeck
                        .OrderBy(x => Random.Range(0, playerDeck.Count))
                        .Select(x => new Card(x.Card))
                );
            }
            if (enemyCurrentDeck.Count == 0)
            {
                enemyCurrentDeck = new Queue<Card>(
                    enemyDeck
                        .OrderBy(x => Random.Range(0, enemyDeck.Count))
                        .Select(x => new Card(x.Card))
                );
            }

            for (int i = 0; i < handSize; i++)
            {
                playerHand.Add(playerCurrentDeck.Dequeue());
                enemyHand.Add(enemyCurrentDeck.Dequeue());
            }

            Debug.Log(string.Join(",", playerHand.Select(x => "[" + string.Join(",", x.Actions.Select(y => y.ActionType.ToString())) + "]" )));
            currentGameState = GameState.PlayCard;
            playerTimeline.Reset();
            enemyTimeline.Reset();
        }
    }
}

public enum GameState
{
    NewEnemy,
    PlayCard,
    ResolveAction,
    ResetTurnEffects,
    ShuffleHand
}