using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public static UIManager main;
    private void Awake()
    {
        main = this;
    }
    [SerializeField]
    private UIHealthDisplay playerHealthDisplay;
    [SerializeField]
    private UIHealthDisplay enemyHealthDisplay;

    [SerializeField]
    private UIPoppingText poppingTextPrefab;
    [SerializeField]
    private UIClock uiClock;

    [SerializeField]
    private Transform poppingTextContainer;

    [SerializeField]
    private Transform playerPoppingTextPosition;
    [SerializeField]
    private Transform enemyPoppingTextPosition;
    [SerializeField]
    private RectTransform playerActionPosition;
    [SerializeField]
    private RectTransform enemyActionPosition;

    private float animateEffectTimer = 0f;
    private float animateEffectDuration = 0.01f;

    private CardEffectInContext animatedEffect;
    private bool isAnimating = false;
    private int playerHealth = 10;

    private float basicUIBreakDuration = 0.4f;

    private void Start()
    {
        playerHealthDisplay.Initialize(playerHealth, playerHealth);
        Invoke("NewEnemy", basicUIBreakDuration);
        Invoke("DrawHand", basicUIBreakDuration * 2);
    }


    public void NewEnemy()
    {
        GameManager.main.ProcessNewEnemy();
        InitializeEnemyHealth();
    }

    public void PlayCard(UICardData card)
    {
        Card playerCard = GameManager.main.PlayCard(card.Index);
        if (playerCard != null)
        {
            UICardManager.main.RemoveCard(card.Index);
            UITimelineBar.main.CreatePlayerCard(UICardManager.main.ConvertCardData(playerCard, card.Index));
        }
        PlayEnemyCard();
        PlayAction();
    }
    public void DrawHand()
    {
        GameManager.main.ProcessShuffle();
    }

    public void InitializeEnemyHealth()
    {
        enemyHealthDisplay.Initialize(playerHealth, playerHealth);
    }

    private void PlayEnemyCard()
    {
        Card enemyCard = GameManager.main.PlayEnemyCard();
        if (enemyCard != null)
        {
            UITimelineBar.main.CreateEnemyCard(UICardManager.main.ConvertCardData(enemyCard, 0));
        }
    }

    public void PlayAction()
    {
        UITimelineBar.main.AnimateCurrentStep(
            playerActionPosition,
            enemyActionPosition
        );
        UIManager.main.AnimateClockRound(delegate
        {
            // actionsLeft -= 1;
            GameManager.main.ProcessResolveAction();
            GameManager.main.ProcessResetTurnEffects();
            PlayEnemyCard();
            UITimelineBar.main.NextStep();
            Debug.Log($"Remaining actions {GameManager.main.GetPlayerRemainingActions()}");
            if (GameManager.main.GetPlayerRemainingActions() <= 0)
            {
                UICardManager.main.CanPlayCard = true;
            }
            else
            {
                PlayAction();
            }
        });
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    ShowEnemyTakeDamage(1);
                }
                else
                {
                    ShowPlayerTakeDamage(1);
                }
            }
        }
        if (isAnimating && animatedEffect != null)
        {
            animateEffectTimer += Time.deltaTime;
            if (animateEffectTimer >= animateEffectDuration)
            {
                animateEffectTimer = 0f;
                isAnimating = false;
                ProcessCardEffect(animatedEffect);
                animatedEffect = null;
            }
        }
        else if (effects.Count > 0)
        {
            isAnimating = true;
            animatedEffect = effects[0];
        }
    }

    private List<CardEffectInContext> effects = new();
    public void AddEffect(CardEffectInContext effect)
    {
        if (effect == null || effect.Effect == CardEffect.None)
        {
            return;
        }
        effects.Add(effect);
        Debug.Log($"Added effect: {effect.Effect} |{effect.Type} | {effect.Amount}");
    }


    private void ProcessCardEffect(CardEffectInContext effect)
    {
        Debug.Log($"Processing effect: {effect.Effect} |{effect.Type} | {effect.Amount}");
        if (effect.Effect == CardEffect.Damaged)
        {
            if (effect.Type == TimelineType.Enemy)
            {
                UIManager.main.ShowPlayerTakeDamage(effect.Amount);
            }
            else
            {
                UIManager.main.ShowEnemyTakeDamage(effect.Amount);
            }
        }
        effects.Remove(effect);
    }


    public void AnimateClockRound(UnityAction afterRoundAction)
    {
        uiClock.AnimateRound(afterRoundAction);
    }

    private Vector2 ScreenToWorld(Vector2 screenPos)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
    }

    public void ShowPoppingText(Vector2 worldPos, string text, AnimationDirection direction, Color color = default(Color))
    {
        UIPoppingText poppingText = Instantiate(poppingTextPrefab, Vector3.zero, Quaternion.identity, poppingTextContainer);
        poppingText.Initialize(worldPos, text, direction, color);
    }

    public void ShowEnemyTakeDamage(int damage)
    {
        Debug.Log("Enemy take damage: " + damage);
        ShowPoppingText(enemyPoppingTextPosition.position, $"-{damage}", AnimationDirection.Right, Color.red);
        enemyHealthDisplay.AnimateChange(-damage);
    }

    public void ShowPlayerTakeDamage(int damage)
    {
        ShowPoppingText(playerPoppingTextPosition.position, $"-{damage}", AnimationDirection.Left, Color.red);
        playerHealthDisplay.AnimateChange(-damage);
    }

    public void AnimatePlayerHealthChange(int change)
    {
        playerHealthDisplay.AnimateValue(change);
    }

}

public enum AnimationDirection
{
    Left,
    Right
}