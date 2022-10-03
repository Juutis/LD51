using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour
{
    public static UITooltip main;
    private void Awake()
    {
        main = this;
    }
    [SerializeField]
    private Text txtTooltip;
    private RectTransform rt;
    private float characterWidth = 10;
    private float characterPaddingWidth = 10;
    [SerializeField]
    private GameObject container;
    [SerializeField]
    private RectTransform imageContainer;
    private bool isShown = false;

    public void Start()
    {
        Hide();
    }

    public void Hide()
    {
        container.SetActive(false);
        isShown = false;
    }

    public void Show(string text, Vector3 position)
    {
        if (rt == null)
        {
            rt = GetComponent<RectTransform>();
        }
        txtTooltip.text = text;
        imageContainer.sizeDelta = new Vector2(text.Length * characterWidth + 2 * characterPaddingWidth, 20f);
        //container.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
        container.SetActive(true);
        isShown = true;
    }

    private void Update()
    {
        if (isShown)
        {
            container.transform.position = Input.mousePosition;
        }
    }
}
