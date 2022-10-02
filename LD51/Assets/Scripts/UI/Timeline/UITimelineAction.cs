using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UITimelineAction : MonoBehaviour
{
    [SerializeField]
    private Image imgIcon;

    [SerializeField]
    private Material grayScaleMaterial;
    private bool isHighlighted = true;
    public bool IsHighlighted { get { return isHighlighted; } }

    public void Initialize(UICardActionData data)
    {
        imgIcon.sprite = data.Icon;
    }

    public void Unhighlight()
    {
        GetComponentsInChildren<Text>().ToList().ForEach(text => text.material = grayScaleMaterial);
        GetComponentsInChildren<Image>().ToList().ForEach(text => text.material = grayScaleMaterial);
        isHighlighted = false;
    }
}


public struct UICardActionData
{
    public Sprite Icon;
    public int Count;
}