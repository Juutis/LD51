using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private bool isHovered = false;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Canvas canvas;
    private int originalSortingOrder = 0;
    private int highlightSortingOrder = 5;

    [SerializeField]
    private UICardData data;

    [SerializeField]
    private Transform actionContainer;

    [SerializeField]
    private UITimelineAction actionPrefab;

    [SerializeField]
    private Text txtCardCost;


    [SerializeField]
    private Image imgCardCostBg;

    private List<UITimelineAction> actions = new();

    public void Initialize(UICardData data)
    {
        this.data = data;
        txtCardCost.text = $"{data.Actions.Count}";
        imgCardCostBg.color = data.CostColor;
        data.Actions.ForEach(action => CreateAction(action));
    }

    private void CreateAction(UICardActionData data)
    {
        UITimelineAction action = Instantiate(actionPrefab, Vector3.zero, Quaternion.identity, actionContainer);
        action.Initialize(data);
        actions.Add(action);
    }

    public void Highlight()
    {
        if (!isHovered)
        {
            isHovered = true;
            canvas.sortingOrder = highlightSortingOrder;
            animator.Play("cardHighlight");
        }
    }

    public void Unhighlight()
    {
        if (isHovered)
        {
            isHovered = false;
            canvas.sortingOrder = originalSortingOrder;
            animator.Play("cardUnhighlight");
        }
    }

    public void Select()
    {
        UITimelineBar.main.CreatePlayerCard(data);
        UICardManager.main.RemoveCard(this);
        Destroy(gameObject);
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


public struct UICardData
{
    public List<UICardActionData> Actions;
    public Color CostColor;
}