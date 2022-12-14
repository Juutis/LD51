using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class UITimelineAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image imgIcon;

    [SerializeField]
    private Material grayScaleMaterial;

    [SerializeField]
    private GameObject container;

    [SerializeField]
    private Text amountText;

    private bool isHighlighted = true;
    public bool IsHighlighted { get { return isHighlighted; } }

    private float animateTimer = 0f;
    private float animateDelay = 0.1f;
    private float animateDuration = 0.4f;
    private float stayDuration = 0.5f;
    private bool isAnimating = false;
    private Vector3 originalPosition;
    private Vector3 targetPosition;

    private RectTransform rt;

    private UICardActionData data;
    public UICardActionData Data { get { return data; } }

    public void SetDataType(CardActionType type)
    {
        data.Type = type;
    }

    private UnityAction callback;
    private void Update()
    {
        if (isAnimating)
        {
            animateTimer += Time.deltaTime;
            rt.position = Vector2.Lerp(originalPosition, targetPosition, animateTimer / animateDuration);
            if (animateTimer >= animateDuration)
            {
                animateTimer = 0f;
                isAnimating = false;
                rt.position = targetPosition;
                if (callback != null)
                {
                    callback();
                }
            }
        }
    }

    public void Reset()
    {
        rt.position = originalPosition;
    }

    public void Initialize(UICardActionData data)
    {
        this.data = data;
        rt = container.GetComponent<RectTransform>();
        imgIcon.sprite = data.Icon;
        if (data.Count > 1)
        {
            amountText.text = data.Count.ToString();
        }
        else
        {
            amountText.gameObject.SetActive(false);
        }
    }

    public void AnimatePerform(Vector3 target, UnityAction callback = null)
    {
        if (!isAnimating)
        {
            this.callback = callback;
            targetPosition = target;
            Invoke("AnimateWithDelay", animateDelay);
        }
    }

    private void AnimateWithDelay()
    {
        isAnimating = true;
        originalPosition = rt.position;
    }

    public void Unhighlight()
    {
        container.GetComponentsInChildren<Text>().ToList().ForEach(text => text.material = grayScaleMaterial);
        container.GetComponentsInChildren<Image>().ToList().ForEach(text => text.material = grayScaleMaterial);
        isHighlighted = false;
    }

    public void SetImage(Sprite img)
    {
        imgIcon.sprite = img;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UITooltip.main.Show($"{data.Type}", transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UITooltip.main.Hide();
    }
}


public struct UICardActionData
{
    public CardActionType Type;
    public Sprite Icon;
    public int Count;
}