using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class DamageResolver : MonoBehaviour, IActionResolver
{
    private Character character;

    void Start()
    {
        character = GetComponent<Character>();
    }

    public CardEffect ResolveTargetAction(int actionAmount)
    {
        return character.TakeDamage(actionAmount);
    }

    public CardEffect ResolveSelfAction(int actionAmount)
    {
        return CardEffect.None;
    }

    public CardActionType GetActionType()
    {
        return CardActionType.Attack;
    }

    public void ResetTurnEffects()
    {
    }
}
