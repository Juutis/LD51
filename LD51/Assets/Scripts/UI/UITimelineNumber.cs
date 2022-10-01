using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimelineNumber : MonoBehaviour
{
    [SerializeField]
    private Image imgBackground;
    [SerializeField]
    private Text txtNumber;
    [SerializeField]
    private Color highlightColor;

    private Color originalColor;

    public void Initialize(int number)
    {
        txtNumber.text = number.ToString();
        originalColor = imgBackground.color;
    }


    public void Highlight()
    {
        imgBackground.color = highlightColor;
    }

    public void Unhighlight()
    {
        imgBackground.color = originalColor;
    }
}
