using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoppingText : MonoBehaviour
{
    [SerializeField]
    private Text txtValue;
    [SerializeField]
    private Animator animator;
    public void Initialize(Vector2 pos, string value, AnimationDirection direction, Color color = default(Color))
    {
        transform.position = pos;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
        txtValue.text = value;
        txtValue.color = color;
        if (direction == AnimationDirection.Right)
        {
            animator.Play("poppingTextPopRight");
        }
        else
        {
            animator.Play("poppingTextPopLeft");
        }
    }

    public void AnimationEnded()
    {
        Destroy(gameObject);
    }
}
