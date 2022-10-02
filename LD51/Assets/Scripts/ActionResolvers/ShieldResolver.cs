using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class ShieldResolver : MonoBehaviour, IActionResolver
{
    private Character character;

    void Start()
    {
        character = GetComponent<Character>();
    }

    public CardEffect ResolveTargetAction(int actionAmount)
    {
        return CardEffect.None;
    }

    public CardEffect ResolveSelfAction(int actionAmount)
    {
        // Debug.Log($"Shield up for {gameObject.name}");
        character.SetShield(true);
        return CardEffect.None;
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
