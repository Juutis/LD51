using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    [SerializeField]
    private List<ActionIcon> actionIcons = new();

    private List<UICardData> cardDatas = new();
    private List<UICard> cards = new();

    private bool canPlayCard = true;
    public bool CanPlayCard { get { return canPlayCard; } }

    public void PlayCard(UICardData card)
    {
        if (canPlayCard)
        {
            canPlayCard = false;
            GameManager.main.PlayCard(card.Index);
            GameManager.main.ResolveAction();
            PlayAction(card.Actions.Count);
        }
    }

    public void PlayAction(int actionsLeft)
    {
        UIManager.main.AnimateClockRound(delegate
        {
            actionsLeft -= 1;
            UITimelineBar.main.NextStep();
            if (actionsLeft <= 0)
            {
                canPlayCard = true;
            }
            else
            {
                GameManager.main.ResolveAction();
                PlayAction(actionsLeft);
            }
        });
    }

    public void DrawHand(List<Card> hand)
    {
        int index = 0;
        hand.ForEach(card => CreateUICard(ConvertCardData(card, index++)));
    }

    public UICardData ConvertCardData(Card card, int index)
    {
        return new UICardData
        {
            Actions = card.Actions.Select((action) => new UICardActionData
            {
                Icon = actionIcons.FirstOrDefault(icon => icon.Type == action.ActionType).Sprite,
                Count = action.ActionAmount
            }).ToList(),
            CostColor = card.Actions.Count < costColors.Count ? costColors[card.Actions.Count] : defaultCostColor,
            Index = index
        };

    }

    public void CreateUICard(UICardData data)
    {
        UICard card = Instantiate(uiCardPrefab, Vector2.zero, Quaternion.identity, uiCardHandContainer);
        card.Initialize(data);
        cards.Add(card);
    }

    public void RemoveCard(int index)
    {
        UICard card = cards.FirstOrDefault(card => card.Index == index);
        if (card != null)
        {
            cards.Remove(card);
            Destroy(card.gameObject);
        }
    }
}

[System.Serializable]
public class ActionIcon
{
    public Sprite Sprite;
    public CardActionType Type;
}