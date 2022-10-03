using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class HealResolver : MonoBehaviour, IActionResolver
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
        character.Heal(actionAmount);
        return CardEffect.Healed;
    }

    public CardActionType GetActionType()
    {
        return CardActionType.Heal;
    }

    public void ResetTurnEffects()
    {
    }
}
