using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public class UITimelineAction : MonoBehaviour
{
    [SerializeField]
    private Image imgIcon;

    [SerializeField]
    private Material grayScaleMaterial;

    [SerializeField]
    private GameObject container;
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
                Invoke("Reset", stayDuration);
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
    }

    public void AnimatePerform(Vector3 target, UnityAction callback = null)
    {
        if (!isAnimating)
        {
            this.callback = callback;
            targetPosition = target;
            Debug.Log("AnimatePerform");
            Invoke("AnimateWithDelay", animateDelay);
        }
    }

    private void AnimateWithDelay()
    {
        Debug.Log("AnimateWithDelay");
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
}


public struct UICardActionData
{
    public CardActionType Type;
    public Sprite Icon;
    public int Count;
}