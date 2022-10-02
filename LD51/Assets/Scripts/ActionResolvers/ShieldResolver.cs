using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldResolver : MonoBehaviour, IActionResolver
{
    [SerializeField]
    private Character character;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResolveTargetAction(int actionAmount)
    {
    }

    public void ResolveSelfAction(int actionAmount)
    {
        // Debug.Log($"Shield up for {gameObject.name}");
        character.SetShield(true);
    }

    public CardActionType GetActionType()
    {
        return CardActionType.Defend;
    }

    public void ResetTurnEffects()
    {
        character.SetShield(false);
    }
}
