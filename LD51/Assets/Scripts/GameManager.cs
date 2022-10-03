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

    public int GetPlayerRemainingActions ()
    {
        return playerTimeline.GetRemainingActions();
    }

    public void PlayCard(int cardIndex)
    {
        if (playerTimeline.GetCurrentAction() == null)
        {
            Debug.Log("Play cards");
            Card card = playerDeck.PlayCard(cardIndex);

            playerTimeline.AddCard(card);
            UICardManager.main.RemoveCard(cardIndex);
            UITimelineBar.main.CreatePlayerCard(UICardManager.main.ConvertCardData(card, cardIndex));
        }
    }

    public void PlayEnemyCard()
    {
        if (enemyTimeline.GetCurrentAction() == null)
        {
            // ai code needed
            int index = 0;

            Card card = enemyDeck.PlaySkip(enemyTimeline.GetRemainingTime());

            if (enemyDeck.HasPlayableCard(enemyTimeline.GetRemainingTime()))
            {
                card = enemyDeck.PlayCard(index);
            }

            enemyTimeline.AddCard(card);
            UITimelineBar.main.CreateEnemyCard(UICardManager.main.ConvertCardData(card, index));
        }
    }

    public void ResolveAction()
    {
        currentGameState = GameState.ResolveAction;
    }
    // Update is called once per frame
    void Update()
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
            currentGameState = GameState.NewEnemy;
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

    private void ProcessNewEnemy()
    {
        Debug.Log("New enemy");
        currentEnemy++;
        enemyActionResolver = enemies[currentEnemy];
        playerTimeline.Reset();
        enemyTimeline = new(enemyActionResolver);
        enemyTimeline.Type = TimelineType.Enemy;
        enemyTimeline.SetTargetResolver(playerActionResolver);
        playerTimeline.SetTargetResolver(enemyActionResolver);
        enemyDeck = enemyActionResolver.GetComponent<Deck>();
        // heal player a bit
        currentGameState = GameState.ShuffleHand;
    }

    private void ProcessShuffle()
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
        currentGameState = GameState.PlayCard;
        playerTimeline.Reset();
        enemyTimeline.Reset();
    }

    private void ProcessResetTurnEffects()
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

    private void ProcessResolveAction()
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

                UIManager.main.AddEffect(playerEffect);

                if (playerEffect.Effect == CardEffect.Killed)
                {
                    // Do stuff
                    Debug.Log("Enemy killed");
                    currentGameState = GameState.EnemyDead;
                    break;
                }

                CardEffectInContext enemyEffect = enemyTimeline.ResolveActions(type);
                UIManager.main.AddEffect(enemyEffect);

                if (enemyEffect.Effect == CardEffect.Killed)
                {
                    // Do stuff
                    Debug.Log("Player killed");
                    currentGameState = GameState.PlayerDead;
                    break;
                }
            }

            CardAction playerAction = playerTimeline.GetCurrentAction();
            CardAction enemyAction = enemyTimeline.GetCurrentAction();
            CharacterAnimationManager.main.PlayAnimations(playerAction, enemyAction);

            if (currentGameState == GameState.ResolveAction)
            {
                currentGameState = GameState.ResetTurnEffects;
            }
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