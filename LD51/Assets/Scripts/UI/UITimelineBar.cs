using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UITimelineBar : MonoBehaviour
{
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
        CreateEnemyCard();
        CreatePlayerCard();
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

    public void NextStep()
    {
        timelinePosition += 1;
        if (timelinePosition > timelineMaxIndex)
        {
            timelinePosition = -1;
        }
        timelineMarker.Move(timelinePosition);
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
    private UITimelineCard CreateEnemyCard()
    {
        return CreateCard(enemyCardContainer);
    }
    private UITimelineCard CreatePlayerCard()
    {

        return CreateCard(playerCardContainer);
    }
    private UITimelineCard CreateCard(Transform container)
    {
        UITimelineCard card = Instantiate(cardPrefab, Vector2.zero, Quaternion.identity, container);
        card.Initialize();
        return card;
    }

    private void ClearAllEnemyCards()
    {
        enemyCards.ForEach(card => card.Remove());
    }

    private void ClearAllPlayerCards()
    {
        playerCards.ForEach(card => card.Remove());
    }
}
