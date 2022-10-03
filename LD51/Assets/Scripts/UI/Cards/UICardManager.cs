using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

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
    private Transform currentCardContainer;

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


    private Vector3 originalScale;
    private Vector3 targetScale = new Vector3(1f, 1f, 1);
    private float animateTimer = 0f;
    private float animateDuration = 1f;
    private bool isAnimating = false;
    [SerializeField]
    private Transform playerCardTarget;
    [SerializeField]
    private Transform enemyCardTarget;
    [SerializeField]
    private Transform enemyCardOriginalPos;
    private Vector3 originalPosition;
    private Vector3 targetPosition;

    private bool playCardNext = false;


    public void SkipRound()
    {
        if (canPlayCard)
        {
            GameManager.main.SkipRound();
            GameManager.main.ResolveAction();
            UIManager.main.PlayAction();
        }
    }

    private UICardData nextCard;
    private UICardData nextCardEnemy;
    private UICard copyCard;
    private UICard copyCardEnemy;
    private UnityAction animationFinishedCallback;
    public void PlayCard(UICardData card)
    {
        if (canPlayCard)
        {
            nextCard = card;
            canPlayCard = false;
            AnimateCardSelection();
        }
    }

    public void DestroyCopyCard()
    {
        if (copyCard != null)
        {
            Destroy(copyCard.gameObject);
            copyCard = null;
        }
    }

    public void AnimateEnemyCard(Card enemyCard, UnityAction callback)
    {
        animationFinishedCallback = callback;
        playCardNext = false;
        isAnimating = true;
        animateTimer = 0f;
        copyCard = Instantiate(uiCardPrefab, Vector2.zero, Quaternion.identity, currentCardContainer);
        copyCard.transform.position = enemyCardOriginalPos.position;
        copyCard.Initialize(ConvertCardData(enemyCard, -1));
        targetPosition = enemyCardTarget.position;
        originalPosition = copyCard.transform.position;
        originalScale = copyCard.transform.localScale;
    }

    public void AnimateCardSelection()
    {
        playCardNext = true;
        isAnimating = true;
        animateTimer = 0f;
        UICard card = FindCard(nextCard.Index);
        copyCard = Instantiate(card, card.transform.position, Quaternion.identity, currentCardContainer);
        copyCard.SetInactiveButNotGrayscale();
        targetPosition = playerCardTarget.position;
        originalPosition = copyCard.transform.position;
        originalScale = copyCard.transform.localScale;
        RemoveCard(nextCard.Index);
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
        if (isAnimating)
        {
            animateTimer += Time.deltaTime;
            copyCard.transform.position = Vector3.Lerp(originalPosition, targetPosition, animateTimer / animateDuration);
            copyCard.transform.localScale = Vector3.Lerp(originalScale, targetScale, animateTimer / animateDuration);
            if (animateTimer >= animateDuration)
            {
                animateTimer = 0f;
                isAnimating = false;
                copyCard.transform.position = targetPosition;
                copyCard.transform.localScale = targetScale;
                DestroyCopyCard();
                if (playCardNext)
                {
                    Invoke("PlayCardWithDelay", 0.2f);
                    playCardNext = false;
                }
                if (animationFinishedCallback != null)
                {
                    animationFinishedCallback();
                    animationFinishedCallback = null;
                }
            }
        }
    }

    public void PlayCardWithDelay()
    {
        UIManager.main.PlayCard(nextCard);
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

    private UICard FindCard(int index)
    {
        return cards.FirstOrDefault(card => card.Index == index);
    }

    public void RemoveCard(int index)
    {
        UICard card = FindCard(index);
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