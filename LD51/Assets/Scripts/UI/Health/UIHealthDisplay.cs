using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthDisplay : MonoBehaviour
{
    [SerializeField]
    private Text txtValue;
    [SerializeField]
    private Image imgValue;
    [SerializeField]
    private GameObject container;

    private int currentValue;
    private int targetValue;
    private int animateStartValue;
    private int maxValue;

    [SerializeField]
    private Animator animator;

    private bool isAnimating = false;
    private float animateTimer = 0f;
    private float animateDuration = 0.4f;

    [SerializeField]
    private bool showMaxValue = false;

    public bool StartHidden = true;

    void Start()
    {
        if (StartHidden) {
            container.SetActive(false);
        }
    }

    public void Hide()
    {
        container.SetActive(false);
    }

    public void Initialize(int value, int max)
    {
        container.SetActive(true);
        currentValue = value;
        targetValue = value;
        maxValue = max;
        SetDisplayValue();
    }

    public void AnimateChange(int change)
    {
        AnimateValue(currentValue + change);
    }

    public void AnimateValue(int value)
    {
        value = Mathf.Clamp(value, 0, maxValue);
        animateStartValue = currentValue;
        targetValue = value;
        animator.Play("healthUpdate");
        isAnimating = true;
    }

    private void SetDisplayValue()
    {
        if (showMaxValue)
        {
            txtValue.text = $"{currentValue} / {maxValue}";
        }
        else
        {
            txtValue.text = $"{currentValue}";
        }
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
