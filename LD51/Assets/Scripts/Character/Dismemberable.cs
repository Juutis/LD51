using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Dismemberable : MonoBehaviour
{
    [SerializeField]
    private bool dismember = false;

    private bool dismembered = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (dismember && !dismembered) {
            dismembered = true;
            var limb = Instantiate(gameObject, transform, transform.parent);
            limb.GetComponent<Dismemberable>().Dismember();
            gameObject.SetActive(false);
            syncSpriteRenderersInChildren(transform, limb.transform);
        }
    }

    public void Dismember() {
        dismembered = true;
        transform.parent = null;
        foreach(var rb in GetComponentsInChildren<Rigidbody2D>()) {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        var ownRb = GetComponent<Rigidbody2D>();
        ownRb.AddForce(Vector2.up * 10.0f + Vector2.left * Random.Range(-5.0f, 5.0f), ForceMode2D.Impulse);
        ownRb.AddTorque(Random.Range(-5.0f, 5.0f), ForceMode2D.Impulse);
        var joint = GetComponent<Joint2D>();
        if (joint != null) {
            joint.enabled = false;
        }
    }

    private void syncSpriteRenderersInChildren(Transform source, Transform target) {
        syncSpriteRenderers(source.gameObject, target.gameObject);
        foreach(Transform child in source) {
            var targetChild = target.Find(child.name);
            syncSpriteRenderersInChildren(child, targetChild);
        }
    }

    private void syncSpriteRenderers(GameObject source, GameObject target) {
        var sourceRand = source.GetComponent<SpriteRandomizer>();
        var targetRand = target.GetComponent<SpriteRandomizer>();
        if (sourceRand != null && targetRand != null) {
            targetRand.SetStaticSprite(sourceRand.GetSpriteIndex());
        }
    }

}
