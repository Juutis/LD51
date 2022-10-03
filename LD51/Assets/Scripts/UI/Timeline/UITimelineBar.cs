using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UITimelineBar : MonoBehaviour
{
    public static UITimelineBar main;
    private void Awake()
    {
        main = this;
    }

    [SerializeField]
    private UITimelineCard cardPrefab;
    [SerializeField]
    private UITimelineMarker timelineMarker;
    [SerializeField]
    private UITimelineNumber numberPrefab;
    [SerializeField]
    private Transform enemyCardContainer;
    [SerializeField]
    private Transform playerCardContainer;
    [SerializeField]
    private Transform numberContainer;

    private List<UITimelineCard> enemyCards = new();
    private List<UITimelineCard> playerCards = new();

    private List<UITimelineNumber> numbers = new();
    private int timelinePosition = -1;
    private int timelineMaxIndex = 9;
    void Start()
    {
        CreateNumbers();
    }

    private void CreateNumbers()
    {
        for (int index = 0; index <= timelineMaxIndex; index += 1)
        {
            UITimelineNumber number = Instantiate(numberPrefab, Vector3.zero, Quaternion.identity, numberContainer);
            number.Initialize(index + 1);
            number.Highlight();
            numbers.Add(number);
        }
    }

    public void Clear()
    {
        ClearAllEnemyCards();
        ClearAllPlayerCards();
    }

    public void AnimateCurrentStep(RectTransform playerPosition, RectTransform enemyPosition)
    {
        Debug.Log("AnimateCurrent");
        UITimelineAction playerAction = GetCurrentAction(playerCards);
        UITimelineAction enemyAction = GetCurrentAction(enemyCards);

        if (playerAction)
        {
            playerAction.AnimatePerform(playerPosition.position, delegate
            {
                CharacterAnimationManager.main.PlayAnimations(playerAction, enemyAction);
            });
        }
        if (enemyAction)
        {
            enemyAction.AnimatePerform(enemyPosition.position);
        }

    }

    public void NextStep()
    {
        timelinePosition += 1;
        if (timelinePosition >= timelineMaxIndex)
        {
            Clear();
            UICardManager.main.PreviousRoundFinished = true;
            timelinePosition = -1;
            UIManager.main.DrawHand();
        }
        timelineMarker.Move(timelinePosition);
        UnhighlightActionsAndCards(playerCards);
        UnhighlightActionsAndCards(enemyCards);
        for (int index = 0; index < numbers.Count; index += 1)
        {
            if (index > timelinePosition)
            {
                numbers[index].Highlight();
            }
            else
            {
                numbers[index].Unhighlight();
            }
        }
    }

    private UITimelineAction GetCurrentAction(List<UITimelineCard> cards)
    {
        UITimelineAction action = null;
        int currentActionIndex = timelinePosition + 1;
        int actionPosition = 0;
        foreach (UITimelineCard card in cards)
        {
            for (int actionIndex = 0; actionIndex < card.Actions.Count; actionIndex += 1)
            {
                if (actionPosition == currentActionIndex)
                {
                    return card.Actions[actionIndex];
                }
                actionPosition += 1;
            }
        }
        return action;
    }

    private void UnhighlightActionsAndCards(List<UITimelineCard> cards)
    {
        int actionPosition = 0;
        for (int cardIndex = 0; cardIndex < cards.Count; cardIndex += 1)
        {
            if (timelinePosition >= cardIndex)
            {
                UITimelineCard card = cards[cardIndex];
                for (int actionIndex = 0; actionIndex < card.Actions.Count; actionIndex += 1)
                {
                    if (timelinePosition >= actionPosition)
                    {
                        UITimelineAction action = card.Actions[actionIndex];
                        if (action.IsHighlighted)
                        {
                            action.Unhighlight();
                        }
                    }
                    actionPosition += 1;
                }
                if (card.Actions.All(action => !action.IsHighlighted))
                {
                    card.Unhighlight();
                }
            }
        }
    }

    public UITimelineCard CreateEnemyCard(UICardData cardData)
    {
        UITimelineCard card = CreateCard(enemyCardContainer, cardData);
        enemyCards.Add(card);
        return card;
    }
    public UITimelineCard CreatePlayerCard(UICardData cardData)
    {

        UITimelineCard card = CreateCard(playerCardContainer, cardData);
        playerCards.Add(card);
        return card;
    }
    private UITimelineCard CreateCard(Transform container, UICardData cardData)
    {
        Debug.Log("Create timeline card");
        UITimelineCard card = Instantiate(cardPrefab, Vector2.zero, Quaternion.identity, container);
        card.Initialize(cardData);
        return card;
    }

    private void ClearAllEnemyCards()
    {
        enemyCards.ForEach(card => card.Remove());
        enemyCards.Clear();
    }

    private void ClearAllPlayerCards()
    {
        playerCards.ForEach(card => card.Remove());
        playerCards.Clear();
    }
}
