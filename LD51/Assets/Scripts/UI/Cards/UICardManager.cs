using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICardManager : MonoBehaviour
{

    public static UICardManager main;
    private void Awake()
    {
        main = this;
    }

    [SerializeField]
    private UICard uiCardPrefab;

    [SerializeField]
    private Transform uiCardHandContainer;

    [SerializeField]
    private Sprite testSprite1;
    [SerializeField]
    private Sprite testSprite2;

    [SerializeField]
    private Color defaultCostColor;

    [SerializeField]
    private List<Color> costColors = new();

    private List<UICardData> cardDatas = new();
    private List<UICard> cards = new();

    private void Start()
    {
        CreateTestData();
        cardDatas.ForEach(card => CreateUICard(card));
    }

    private void CreateTestData()
    {
        UICardActionData testAction1 = new UICardActionData
        {
            Icon = testSprite1
        };
        UICardActionData testAction2 = new UICardActionData
        {
            Icon = testSprite2
        };
        UICardData testCard1 = CreateCardData(
            new() {
                testAction1
            }
        );
        UICardData testCard2 = CreateCardData(
            new() {
                testAction1,
                testAction2,
                testAction1,
            }
        );
        UICardData testCard3 = CreateCardData(
            new() {
                testAction1,
                testAction2,
                testAction2,
                testAction1,
            }
        );
        cardDatas.Add(testCard1);
        cardDatas.Add(testCard2);
        cardDatas.Add(testCard3);
    }

    public UICardData CreateCardData(List<UICardActionData> actions)
    {
        return new UICardData
        {
            Actions = actions,
            CostColor = actions.Count < costColors.Count ? costColors[actions.Count] : defaultCostColor
        };
    }

    public void CreateUICard(UICardData data)
    {
        UICard card = Instantiate(uiCardPrefab, Vector2.zero, Quaternion.identity, uiCardHandContainer);
        card.Initialize(data);
        cards.Add(card);
    }

    public void RemoveCard(UICard card)
    {
        cards.Remove(card);
    }
}
