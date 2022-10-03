using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISkipRound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private Color originalColor;
    [SerializeField]
    private Color highlightColor;

    private void Highlight()
    {
        backgroundImage.color = highlightColor;
    }

    private void Unhighlight()
    {
        backgroundImage.color = originalColor;
    }

    private void Select()
    {
        if (UICardManager.main.CanPlayCard)
        {
            UICardManager.main.SkipRound();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Unhighlight();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }
}
