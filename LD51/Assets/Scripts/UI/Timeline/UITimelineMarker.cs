using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITimelineMarker : MonoBehaviour
{


    private RectTransform rt;
    private float initialX;
    private float stepSize;


    private float animateTimer = 0f;
    private float animateDelay = 0.1f;
    private float animateDuration = 0.4f;
    private float stayDuration = 0.5f;
    private bool isAnimating = false;
    private float targetX;
    private float originalX;

    private void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        rt = GetComponent<RectTransform>();
        initialX = transform.localPosition.x;
        stepSize = rt.sizeDelta.x;
    }

    private void Update()
    {
        if (isAnimating)
        {
            animateTimer += Time.deltaTime;
            transform.localPosition = new Vector2(Mathf.Lerp(originalX, targetX, animateTimer / animateDuration), transform.localPosition.y);
            if (animateTimer > animateDuration)
            {
                isAnimating = false;
                animateTimer = 0f;
                transform.localPosition = new Vector2(targetX, transform.localPosition.y);
            }
        }
    }

    public void Move(int timelinePosition)
    {
        //transform.localPosition = new Vector2(initialX + stepSize * (timelinePosition + 1), transform.localPosition.y);
        isAnimating = true;
        originalX = transform.localPosition.x;
        targetX = initialX + stepSize * (timelinePosition + 1);
    }

}
