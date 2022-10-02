using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITimelineMarker : MonoBehaviour
{


    private RectTransform rt;
    private float originalX;
    private float stepSize;
    private void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        rt = GetComponent<RectTransform>();
        originalX = transform.localPosition.x;
        stepSize = rt.sizeDelta.x;
    }

    public void Move(int timelinePosition)
    {
        transform.localPosition = new Vector2(originalX + stepSize * (timelinePosition + 1), transform.localPosition.y);
    }
}
