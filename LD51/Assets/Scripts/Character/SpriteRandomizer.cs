using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRandomizer : MonoBehaviour
{
    private SpriteRenderer sprite;
    [SerializeField]
    private Sprite[] sprites;

    private int staticSprite = -1;
    private int spriteIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (staticSprite < 0) {
            spriteIndex = Random.Range(0, sprites.Length);
        } else {
            spriteIndex = staticSprite;
        }
        sprite.sprite = sprites[spriteIndex];
    }

    public void SetStaticSprite(int index) {
        staticSprite = index;
    }

    public int GetSpriteIndex() {
        return spriteIndex;
    }
}
