using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITimelineCard : MonoBehaviour
{

    [SerializeField]
    private UITimelineAction actionPrefab;
    [SerializeField]
    private Sprite testSprite;
    [SerializeField]
    private Sprite testSprite2;

    [SerializeField]
    private Transform actionContainer;
    private float height = 40;
    public void Initialize()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, height);
        CreateAction(new TimelineActionData
        {
            Icon = testSprite
        });
        CreateAction(new TimelineActionData
        {
            Icon = testSprite2
        });
        CreateAction(new TimelineActionData
        {
            Icon = testSprite
        });
    }

    private UITimelineAction CreateAction(TimelineActionData data)
    {
        UITimelineAction action = Instantiate(actionPrefab, Vector3.zero, Quaternion.identity, actionContainer);
        action.Initialize(data);
        return action;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
