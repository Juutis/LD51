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
    public UITimelineCard CreateEnemyCard(UICardData cardData)
    {
        return CreateCard(enemyCardContainer, cardData);
    }
    public UITimelineCard CreatePlayerCard(UICardData cardData)
    {

        return CreateCard(playerCardContainer, cardData);
    }
    private UITimelineCard CreateCard(Transform container, UICardData cardData)
    {
        UITimelineCard card = Instantiate(cardPrefab, Vector2.zero, Quaternion.identity, container);
        card.Initialize(cardData);
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
