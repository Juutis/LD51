using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICard : MonoBehaviour
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

    [SerializeField]
    private Image cardArt;

    [SerializeField]
    private Material grayscaleMaterial;

    private List<UITimelineAction> actions = new();

    [SerializeField]
    private List<Image> grayScaledImages;

    private bool inactive = false;

    [SerializeField]
    private UICardHoverHelper hoverHelper;

    public int Index { get { return data.Index; } }
    public int Cost { get { return data.Actions.Count; } }

    public void Initialize(UICardData data)
    {
        this.data = data;
        txtCardCost.text = $"{data.Actions.Count}";
        imgCardCostBg.color = data.CostColor;
        data.Actions.ForEach(action => CreateAction(action));
        hoverHelper.Initialize(this);
        cardArt.sprite = data.CardArt;
    }
    private void CreateAction(UICardActionData data)
    {
        UITimelineAction action = Instantiate(actionPrefab, Vector3.zero, Quaternion.identity, actionContainer);
        action.Initialize(data);
        actions.Add(action);
    }

    public void Highlight()
    {
        if (!isHovered && UICardManager.main.CanPlayCard && !inactive)
        {
            isHovered = true;
            canvas.sortingOrder = highlightSortingOrder;
            animator.Play("cardHighlight");
        }
    }

    public void Unhighlight()
    {
        if (isHovered && !inactive)
        {
            isHovered = false;
            canvas.sortingOrder = originalSortingOrder;
            animator.Play("cardUnhighlight");
        }
    }

    public void Select()
    {
        if (UICardManager.main.CanPlayCard && !inactive)
        {
            UICardManager.main.PlayCard(data);
            UITooltip.main.Hide();
        }
    }

    public void SetInactiveButNotGrayscale()
    {
        inactive = true;
        hoverHelper.enabled = false;
    }

    public void SetInactive()
    {
        inactive = true;
        grayScaledImages.ForEach(x => x.material = grayscaleMaterial);
        hoverHelper.enabled = false;
    }

    public void SetActiveAgain()
    {
        inactive = false;
        grayScaledImages.ForEach(x => x.material = null);
        hoverHelper.enabled = true;
    }
}


public struct UICardData
{
    public List<UICardActionData> Actions;
    public Color CostColor;
    public Sprite CardArt;
    public int Index;
}