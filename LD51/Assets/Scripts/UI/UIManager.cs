using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private UISkipRound skipRoundButton;

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
    private float animateEffectDuration = 0f;

    private CardEffectInContext animatedEffect;
    private bool isAnimating = false;

    private float basicUIBreakDuration = 0.4f;
    private bool enemyKilled = false;
    private List<CardEffectInContext> effects = new();

    private Character currentEnemy;
    private Character player;

    bool playerKilled = false;
    public GameObject YouDied, YouWin;
    bool playerWasKilled = false;

    private void Start()
    {
        playerHealthDisplay.Initialize(player.Health, player.MaxHealth);
        Invoke("NewEnemy", basicUIBreakDuration);
        Invoke("DrawHand", basicUIBreakDuration * 2);
        skipRoundButton.SetInactive();
    }

    public void NewEnemy()
    {
        currentEnemy = GameManager.main.ProcessNewEnemy();
        CharacterAnimationManager.main.WalkToNextEnemy();

        UITimelineBar.main.Clear();
        Invoke("NextRoundDelayed", 3f);
        Invoke("ShowEnemyHealth", 3f);
    }
    
    public void SetPlayerCharacter(Character p)
    {
        player = p;
    }

    public void NextRoundDelayed()
    {
        UITimelineBar.main.NextRound();
        enemyKilled = false;
    }

    public void ShowEnemyHealth()
    {
        if (currentEnemy != null)
        {
            InitializeEnemyHealth(currentEnemy.MaxHealth);
            UICardManager.main.PreviousRoundFinished = true;
        }
    }

    public void PlayCard(UICardData card)
    {
        if (playerWasKilled)
        {
            return;
        }
        Debug.Log($"Playing card: {card.Index} -> {card.Actions.Count}");
        Card playerCard = GameManager.main.PlayCard(card.Index);
        if (playerCard != null)
        {
            UITimelineBar.main.CreatePlayerCard(UICardManager.main.ConvertCardData(playerCard, card.Index));
        }
        if (!enemyKilled && !playerWasKilled)
        {
            PlayEnemyCard(delegate
            {
                Invoke("PlayActions", 0.5f);
            });
        }
        UICardManager.main.SetHandInactive();
        skipRoundButton.SetInactive();
    }

    public void PlayActions()
    {
        PlayAction();
    }

    public void DrawHand()
    {
        UITimelineBar.main.ResetMarker();
        GameManager.main.ProcessShuffle();
        UICardManager.main.DestroyCopyCard();
        //skipRoundButton.SetActiveAgain();
    }

    public void InitializeEnemyHealth(int hp)
    {
        enemyHealthDisplay.Initialize(hp, hp);
    }

    public void HideEnemyHp()
    {
        enemyHealthDisplay.Hide();
    }

    private void PlayEnemyCard(UnityAction callback)
    {
        if (GameManager.main.GetPlayerRemainingActions() <= 0)
        {
            callback();
            return;
        }
        Card enemyCard = GameManager.main.PlayEnemyCard();
        if (enemyCard != null)
        {
            AnimateEnemyCard(enemyCard, delegate
            {
                UITimelineBar.main.CreateEnemyCard(UICardManager.main.ConvertCardData(enemyCard, 0));
                callback();
            });
        }
        else
        {
            callback();
        }
    }

    public void AnimateEnemyCard(Card enemyCard, UnityAction callback)
    {
        UICardManager.main.AnimateEnemyCard(enemyCard, callback);
    }
    
    public void EnemyWasKilled()
    {
        Debug.Log("Enemy was killed!");
        enemyKilled = true;
    }

    public void PlayerWasKilled()
    {
        UICardManager.main.SetHandInactive();
        UICardManager.main.CanPlayCard = false;
        Debug.Log("Player was killed!");
        playerWasKilled = true;
        YouDied.SetActive(true);
    }


    public void Win()
    {
        YouWin.SetActive(true);
    }
    public void PlayAction()
    {
        if (playerWasKilled)
        {
            return;
        }
        UITimelineBar.main.AnimateCurrentStep(
            playerActionPosition,
            enemyActionPosition,
            delegate
            {
                // actionsLeft -= 1;
                Debug.Log("Playing enemy card");
                if (!enemyKilled && GameManager.main.GetPlayerRemainingActions() > 0 && !playerKilled)
                {
                    PlayEnemyCard(ContinueAction);
                }
                else
                {
                    ContinueAction();
                }

            }
        );
    }

    private void ContinueAction()
    {
        UITimelineBar.main.NextStep();
        Debug.Log($"Remaining actions {GameManager.main.GetPlayerRemainingActions()}");
        if (GameManager.main.GetPlayerRemainingActions() <= 0)
        {
            UICardManager.main.DestroyCopyCard();
            UICardManager.main.CanPlayCard = true;
            UICardManager.main.SetPlayableCardsActive();
            skipRoundButton.SetActiveAgain();
        }
        else
        {
            if (!enemyKilled && !playerWasKilled)
            {
                PlayAction();
            }
        }
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
            HandleHPEffects(TimelineType.Player);
            HandleHPEffects(TimelineType.Enemy);
            if (effects.Count > 0)
            {
                animatedEffect = effects[0];
            }
        }

        player.ClampHP();
        currentEnemy?.ClampHP();
    }

    private void HandleHPEffects(TimelineType type)
    {
        int totalHPChange = 0;
        List<CardEffectInContext> handledCards = new();
        TimelineType damageType = type == TimelineType.Player ? TimelineType.Enemy : TimelineType.Player; 

        foreach (CardEffectInContext effectContext in effects)
        {
            if (effectContext.Effect == CardEffect.Healed && effectContext.Type == type)
            {
                totalHPChange += effectContext.Amount;
                handledCards.Add(effectContext);
            }
            else if (effectContext.Effect == CardEffect.Damaged && effectContext.Type == damageType)
            {
                totalHPChange -= effectContext.Amount;
                handledCards.Add(effectContext);
            }
        }

        handledCards.ForEach(x => effects.Remove(x));

        if (totalHPChange <= 0 && handledCards.Count > 0)
        {
            effects.Add(new() { Amount = -totalHPChange, Effect = CardEffect.Damaged, Type = damageType });
        }
        else if (totalHPChange > 0 && handledCards.Count > 0)
        {
            effects.Add(new() { Amount = totalHPChange, Effect = CardEffect.Healed, Type = type });
        }
    }

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
        if (effect.Effect == CardEffect.Healed)
        {
            if (effect.Type == TimelineType.Enemy)
            {
                UIManager.main.ShowEnemyTakeDamage(-effect.Amount);
            }
            else
            {
                UIManager.main.ShowPlayerTakeDamage(-effect.Amount);
            }
        }
        if (effect.Effect == CardEffect.Damaged || effect.Effect == CardEffect.Killed)
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
        if (effect.Effect == CardEffect.Killed)
        {
            if (effect.Type == TimelineType.Enemy)
            {
                CharacterAnimationManager.main.PlayPlayerDead();
            }
            else
            {
                CharacterAnimationManager.main.PlayEnemyDead();
                Invoke("NewEnemy", 1.0f);
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
        // Debug.Log("Enemy take damage: " + damage);
        Color textColor = damage > 0 ? Color.red : Color.green;
        textColor = damage == 0 ? Color.white : textColor;
        string damageText = damage > 0 ? $"-{damage}" : $"+{-damage}";
        damageText = damage == 0 ? "±0" : damageText;
        ShowPoppingText(enemyPoppingTextPosition.position, damageText, AnimationDirection.Right, textColor);
        enemyHealthDisplay.AnimateChange(-damage);
    }

    public void ShowPlayerTakeDamage(int damage)
    {
        // Debug.Log("Player take damage: " + damage);
        Color textColor = damage > 0 ? Color.red : Color.green;
        textColor = damage == 0 ? Color.white : textColor;
        string damageText = damage > 0 ? $"-{damage}" : $"+{-damage}";
        damageText = damage == 0 ? "±0" : damageText;
        ShowPoppingText(playerPoppingTextPosition.position, damageText, AnimationDirection.Left, textColor);
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