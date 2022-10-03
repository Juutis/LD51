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

    private List<Card> pendingHand;
    private bool canPlayCard = true;
    public bool CanPlayCard { get { return canPlayCard; } set { canPlayCard = value; } }

    private bool previousRoundFinished = false;
    public bool PreviousRoundFinished { set { previousRoundFinished = value; } }

    public void SkipRound()
    {
        if (canPlayCard)
        {
            GameManager.main.SkipRound();
            GameManager.main.ResolveAction();
            UIManager.main.PlayAction();
        }
    }

    public void PlayCard(UICardData card)
    {
        if (canPlayCard)
        {
            canPlayCard = false;
            UIManager.main.PlayCard(card);
        }
    }

    public void DrawHand(List<Card> hand)
    {
        pendingHand = hand;
    }

    private void Update()
    {
        if (pendingHand != null)
        {
            if (previousRoundFinished)
            {
                cards.ForEach(x => Destroy(x.gameObject));
                cards.Clear();
                previousRoundFinished = false;
                int index = 0;
                pendingHand.ForEach(card => CreateUICard(ConvertCardData(card, index++)));
                pendingHand = null;
            }
        }
    }

    public void SetUnplayableCardsInactive()
    {
        int timeLeft = GameManager.main.GetPlayerRemainingTime();
        cards.Where(card => card.Cost > timeLeft).ToList().ForEach(x => x.SetInactive());
    }

    public void SetHandInactive()
    {
        cards.ForEach(card => card.SetInactive());
    }

    public void SetPlayableCardsActive()
    {
        int timeLeft = GameManager.main.GetPlayerRemainingTime();
        cards.Where(card => card.Cost <= timeLeft).ToList().ForEach(x => x.SetActiveAgain());
    }

    public UICardData ConvertCardData(Card card, int index)
    {
        return new UICardData
        {
            Actions = card.Actions.Select((action) => new UICardActionData
            {
                Type = action.ActionType,
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

    public ActionIcon GetActionIcon(CardActionType type)
    {
        return actionIcons.FirstOrDefault(x => x.Type == type);
    }
}

[System.Serializable]
public class ActionIcon
{
    public Sprite Sprite;
    public CardActionType Type;
}