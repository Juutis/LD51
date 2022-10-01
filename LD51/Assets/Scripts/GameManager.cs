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

    private Queue<Card> playerCurrentDeck = new();
    private Queue<Card> enemyCurrentDeck = new();

    private List<Card> playerHand = new();
    private List<Card> enemyHand = new();

    private const int handSize = 5;

    private Timeline timeline;

    void Start()
    {
        timeline = new();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.PlayCard)
        {
            if (timeline.GetRemainingTime() == 0)
            {
                currentGameState = GameState.ShuffleHand;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                if (timeline.GetPlayerCurrentAction() == null)
                {
                    Debug.Log("Play cards");
                    Card card = playerHand[0];
                    playerHand.RemoveAt(0);

                    timeline.AddCard(card, true);
                }

                if (timeline.GetEnemyCurrentAction() == null)
                {
                    Card eCard = enemyHand[0];
                    enemyHand.RemoveAt(0);

                    timeline.AddCard(eCard, false);
                }
                currentGameState = GameState.ResolveAction;
            }

        }
        else if (currentGameState == GameState.ResolveAction)
        {
            timeline.ResolveActions();
            currentGameState = GameState.PlayCard;
        }
        else
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
            timeline.Reset();
        }
    }
}

public enum GameState
{
    PlayCard,
    ResolveAction,
    ShuffleHand
}