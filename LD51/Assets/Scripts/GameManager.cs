using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{

    public static GameManager main;
    private void Awake()
    {
        main = this;
        playerTimeline = new(playerActionResolver);
        playerDeck = playerActionResolver.GetComponent<Deck>();
    }
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

    public Deck PlayerDeck { get { return playerDeck; } }

    private Transform enemySpawn;
    [SerializeField]
    private GameObject enemyPrefab;
    public GameObject Enemy;
    private Transform worldRotator;

    public void Start()
    {
        enemySpawn = GameObject.FindGameObjectWithTag("EnemySpawn").transform;
        worldRotator = GameObject.FindGameObjectWithTag("Environment Rotator").transform;
        UIManager.main.SetPlayerCharacter(playerActionResolver.GetComponent<Character>());
    }

    public int GetPlayerRemainingActions()
    {
        return playerTimeline.GetRemainingActions();
    }

    public int GetPlayerRemainingTime()
    {
        return playerTimeline.GetRemainingTime();
    }

    public Card PlayCard(int cardIndex)
    {
        Debug.Log($"Playing card {cardIndex}");
        if (playerTimeline.GetCurrentAction() == null)
        {
            Debug.Log("Play cards");
            Card card = playerDeck.PlayCard(cardIndex);
            Debug.Log($"Card {card.Actions.Count}");
            playerTimeline.AddCard(card);
            return card;
        }
        return null;
    }

    public void SkipRound()
    {
        if (playerTimeline.GetCurrentAction() == null)
        {
            Debug.Log("Play cards");
            int actionsLeft = GetPlayerRemainingTime();
            Card card = new Card(-1);

            for (int i = 0; i < actionsLeft; i++)
            {
                card.Actions.Add(new CardAction { ActionAmount = 1, ActionType = CardActionType.Wait });
            }

            playerTimeline.AddCard(card);
            UITimelineBar.main.CreatePlayerCard(UICardManager.main.ConvertCardData(card, -1));
        }
    }

    public Card PlayEnemyCard()
    {
        if (enemyTimeline.GetCurrentAction() == null)
        {
            int remainingTime = enemyTimeline.GetRemainingTime();
            Card card = enemyDeck.PlaySkip(remainingTime);

            if (enemyDeck.HasPlayableCard(remainingTime))
            {
                card = enemyDeck.PlayCardMaxLength(remainingTime);
            }

            enemyTimeline.AddCard(card);
            return card;
        }
        return null;
    }

    public void ResolveAction()
    {
        //currentGameState = GameState.ResolveAction;
    }
    // Update is called once per frame
    void NotUpdate()
    {
        if (currentGameState == GameState.NewEnemy)
        {
            ProcessNewEnemy();
        }
        else if (currentGameState == GameState.PlayCard)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlayCard(0);
            }
        }
        else if (currentGameState == GameState.ResolveAction)
        {
            PlayEnemyCard();
            ProcessResolveAction();
        }
        else if (currentGameState == GameState.ResetTurnEffects)
        {
            ProcessResetTurnEffects();
        }
        else if (currentGameState == GameState.EnemyDead)
        {
            Debug.Log("enemy is dead, long live enemy");
            //currentGameState = GameState.NewEnemy;
        }
        else if (currentGameState == GameState.PlayerDead)
        {
            Debug.Log("YOU DIED :::::D");
        }
        else // shuffle
        {
            ProcessShuffle();
        }
    }

    public Character ProcessNewEnemy()
    {
        currentEnemy++;
        if (currentEnemy >= enemies.Count)
        {
            Debug.Log("YOU WIN");
            return null;
        }
        enemyActionResolver = enemies[currentEnemy];
        playerTimeline.Reset();
        enemyTimeline = new(enemyActionResolver);
        enemyTimeline.Type = TimelineType.Enemy;
        enemyTimeline.SetTargetResolver(playerActionResolver);
        playerTimeline.SetTargetResolver(enemyActionResolver);
        enemyDeck = enemyActionResolver.GetComponent<Deck>();
        // heal player a bit
        //currentGameState = GameState.ShuffleHand;

        Enemy = Instantiate(enemyPrefab, worldRotator);
        Enemy.transform.position = enemySpawn.position;
        return enemyActionResolver.GetComponent<Character>();
    }

    public void ProcessShuffle()
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
        UICardManager.main.DrawHand(playerDeck.GetHand());
        enemyDeck.DrawHand();

        Debug.Log(string.Join(",", playerDeck.GetHand().Select(x => "[" + string.Join(",", x.Actions.Select(y => y.ActionType.ToString())) + "]")));
        //currentGameState = GameState.PlayCard;
        playerTimeline.Reset();
        enemyTimeline.Reset();
    }

    public void ProcessResetTurnEffects()
    {
        // TODO: Make something like GetLateActionEffects() to return a thing so something like parry can stun a character next round
        playerTimeline.ResetTurnEffects();
        enemyTimeline.ResetTurnEffects();
        /*if (playerTimeline.GetRemainingTime() == 0)
        {
            currentGameState = GameState.ShuffleHand;
        }
        else
        {
            currentGameState = GameState.PlayCard;
        }*/
    }

    public void ProcessResolveAction()
    {
        // TODO: Make time forward (and backward?) skip return boolean so it can skip the turn immediately
        // TODO: UI handle skip
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
                CardEffectInContext playerEffect = playerTimeline.ResolveActions(type);

                if (playerEffect != null)
                {
                    Debug.Log($"playerEffect  {playerEffect.Effect} {playerEffect.Amount}");
                }
                UIManager.main.AddEffect(playerEffect);

                if (playerEffect != null && playerEffect.Effect == CardEffect.Killed)
                {
                    // Do stuff
                    Debug.Log("Enemy killed");
                    UIManager.main.EnemyWasKilled();
                    //urrentGameState = GameState.EnemyDead;
                    break;
                }

                CardEffectInContext enemyEffect = enemyTimeline.ResolveActions(type);
                UIManager.main.AddEffect(enemyEffect);

                if (enemyEffect != null && enemyEffect.Effect == CardEffect.Killed)
                {
                    // Do stuff
                    Debug.Log("Player killed");
                    //currentGameState = GameState.PlayerDead;
                    break;
                }
            }

            /*if (currentGameState == GameState.ResolveAction)
            {
                currentGameState = GameState.ResetTurnEffects;
            }*/
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