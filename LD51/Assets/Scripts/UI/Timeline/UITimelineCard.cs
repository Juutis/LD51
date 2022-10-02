using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimelineCard : MonoBehaviour
{

    [SerializeField]
    private UITimelineAction actionPrefab;

    [SerializeField]
    private Transform actionContainer;

    [SerializeField]
    private Image imgCostBg;

    [SerializeField]
    private Material grayscaleMaterial;
    private float height = 40;
    private float singleActionWidth = 40;
    private List<UITimelineAction> actions = new();

    public List<UITimelineAction> Actions { get { return actions; } }

    public void Initialize(UICardData data)
    {
        Debug.Log(data.Actions);
        RectTransform rect = GetComponent<RectTransform>();
        imgCostBg.color = data.CostColor;
        rect.sizeDelta = new Vector2(data.Actions.Count * singleActionWidth, height);
        data.Actions.ForEach(action => CreateAction(action));
    }

    public void Unhighlight()
    {
        imgCostBg.material = grayscaleMaterial;
    }

    private UITimelineAction CreateAction(UICardActionData data)
    {
        UITimelineAction action = Instantiate(actionPrefab, Vector3.zero, Quaternion.identity, actionContainer);
        action.Initialize(data);
        actions.Add(action);
        return action;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}