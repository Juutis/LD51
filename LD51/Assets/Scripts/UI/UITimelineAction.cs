using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimelineAction : MonoBehaviour
{
    [SerializeField]
    private Image imgIcon;
    public void Initialize(UICardActionData data)
    {
        imgIcon.sprite = data.Icon;
    }
}


public struct UICardActionData
{
    public Sprite Icon;
}