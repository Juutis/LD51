using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthDisplay : MonoBehaviour
{
    public static UIHealthDisplay main;
    private void Awake()
    {
        main = this;
    }
    [SerializeField]
    private Text txtValue;
    [SerializeField]
    private Image imgValue;

    private int currentValue;
    private int targetValue;
    private int animateStartValue;
    private int maxValue;

    [SerializeField]
    private Animator animator;

    private bool isAnimating = false;
    private float animateTimer = 0f;
    private float animateDuration = 0.4f;

    private void Start()
    {
        Initialize(10, 10);
    }

    public void Initialize(int value, int max)
    {
        currentValue = value;
        targetValue = value;
        maxValue = max;
        SetDisplayValue();
    }

    public void AnimateValue(int value)
    {
        if (value > maxValue)
        {
            Debug.LogError($"Trying to set UI Health value to {value} which is above the max value of {maxValue}!");
            return;
        }
        animateStartValue = currentValue;
        targetValue = value;
        animator.Play("healthUpdate");
        isAnimating = true;
    }

    private void SetDisplayValue()
    {
        txtValue.text = $"{currentValue}";
        imgValue.fillAmount = (float)currentValue / (float)maxValue;
    }

    private void Update()
    {
        if (isAnimating)
        {
            animateTimer += Time.deltaTime;
            if (animateTimer > animateDuration)
            {
                currentValue = targetValue;
                isAnimating = false;
                animateTimer = 0f;
                SetDisplayValue();
                return;
            }
            currentValue = (int)Mathf.Lerp(animateStartValue, targetValue, animateTimer / animateDuration);
            SetDisplayValue();
        }
    }
}
