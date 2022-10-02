using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private UIHealthDisplay playerHealthDisplay;
    [SerializeField]
    private UIHealthDisplay enemyHealthDisplay;

    [SerializeField]
    private UIPoppingText poppingTextPrefab;

    [SerializeField]
    private Transform poppingTextContainer;

    [SerializeField]
    private Transform playerPoppingTextPosition;
    [SerializeField]
    private Transform enemyPoppingTextPosition;

    private void Update()
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    ShowEnemyTakeDamage(1);
                }
                else
                {
                    ShowPlayerTakeDamage(1);
                }
            }
            if (Input.GetMouseButton(0))
            {
                ShowPoppingText(ScreenToWorld(Input.mousePosition), "asd", AnimationDirection.Left, Color.red);
            }
        }
    }

    private Vector2 ScreenToWorld(Vector2 screenPos)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
    }

    public void ShowPoppingText(Vector2 worldPos, string text, AnimationDirection direction, Color color = default(Color))
    {
        UIPoppingText poppingText = Instantiate(poppingTextPrefab, Vector3.zero, Quaternion.identity, poppingTextContainer);
        poppingText.Initialize(worldPos, text, direction, color);
    }

    public void ShowEnemyTakeDamage(int damage)
    {
        ShowPoppingText(enemyPoppingTextPosition.position, $"-{damage}", AnimationDirection.Right, Color.red);
        enemyHealthDisplay.AnimateChange(-damage);
    }

    public void ShowPlayerTakeDamage(int damage)
    {
        ShowPoppingText(playerPoppingTextPosition.position, $"-{damage}", AnimationDirection.Left, Color.red);
        playerHealthDisplay.AnimateChange(-damage);
    }

    private void Start()
    {
        playerHealthDisplay.Initialize(10, 10);
        enemyHealthDisplay.Initialize(10, 10);
    }

    public void AnimatePlayerHealthChange(int change)
    {
        playerHealthDisplay.AnimateValue(change);
    }

}

public enum AnimationDirection
{
    Left,
    Right
}