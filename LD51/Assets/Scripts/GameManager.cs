using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    private GameState currentGameState = GameState.NewEnemy;
    [SerializeField]
    private CardActionResolver playerActionResolver;

    private CardActionResolver enemyActionResolver;
    [SerializeField]
    private List<CardActionResolver> enemies;
    private int currentEnemy = -1;

    private Deck playerDeck;
    private Deck enemyDeck;

    private Timeline playerTimeline;
    private Timeline enemyTimeline;

    void Start()
    {
        playerTimeline = new(playerActionResolver);
        playerDeck = playerActionResolver.GetComponent<Deck>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.NewEnemy)
        {
            Debug.Log("New enemy");
            currentEnemy++;
            enemyActionResolver = enemies[currentEnemy];
            playerTimeline.Reset();
            enemyTimeline = new(enemyActionResolver);
            enemyTimeline.SetTargetResolver(playerActionResolver);
            playerTimeline.SetTargetResolver(enemyActionResolver);
            enemyDeck = enemyActionResolver.GetComponent<Deck>();
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
                    Card card = playerDeck.PlayCard(0);

                    playerTimeline.AddCard(card);
                }

                if (enemyTimeline.GetCurrentAction() == null)
                {
                    Card card = enemyDeck.PlayCard(0);

                    enemyTimeline.AddCard(card);
                }
                currentGameState = GameState.ResolveAction;
            }

        }
        else if (currentGameState == GameState.ResolveAction)
        {
            // TODO: Make time forward (and backward?) skip return boolean so it can skip the turn immediately
            if (playerTimeline.SkipForward())
            {
                enemyTimeline.SyncCurrentStep(playerTimeline.GetCurrentStep());
            }
            else if (enemyTimeline.SkipForward())
            {
                playerTimeline.SyncCurrentStep(enemyTimeline.GetCurrentStep());
            }
            else
            {
                List<CardActionType> actionTypePrecedence = new List<CardActionType>()
                { CardActionType.Heal, CardActionType.Defend, CardActionType.Parry, CardActionType.Attack, CardActionType.Wait };

                foreach (CardActionType type in actionTypePrecedence)
                {
                    CardEffect playerEffect = playerTimeline.ResolveActions(type);

                    if (playerEffect == CardEffect.Killed)
                    {
                        // Do stuff
                        Debug.Log("Enemy killed");
                        currentGameState = GameState.EnemyDead;
                        break;
                    }

                    CardEffect enemyEffect = enemyTimeline.ResolveActions(type);

                    if (enemyEffect == CardEffect.Killed)
                    {
                        // Do stuff
                        Debug.Log("Player killed");
                        currentGameState = GameState.PlayerDead;
                        break;
                    }
                }

                if (currentGameState == GameState.ResolveAction)
                {
                    currentGameState = GameState.ResetTurnEffects;
                }
            }
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
        else if (currentGameState == GameState.EnemyDead)
        {
            Debug.Log("enemy is dead, long live enemy");
            currentGameState = GameState.NewEnemy;
        }
        else if (currentGameState == GameState.PlayerDead)
        {
            Debug.Log("YOU DIED :::::D");
        }
        else // shuffle
        { 
            if (playerDeck.GetCurrentDeck().Count == 0)
            {
                playerDeck.ShuffleNewDeck();
            }
            if (enemyDeck.GetCurrentDeck().Count == 0)
            {
                enemyDeck.ShuffleNewDeck();
            }

            playerDeck.DrawHand();
            enemyDeck.DrawHand();

            Debug.Log(string.Join(",", playerDeck.GetHand().Select(x => "[" + string.Join(",", x.Actions.Select(y => y.ActionType.ToString())) + "]" )));
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
    PlayerDead,
    EnemyDead,
    ShuffleHand
}