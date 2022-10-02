using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class ParryActionResolver : MonoBehaviour, IActionResolver
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
        Debug.Log($"Parry for {gameObject.name}");
        character.SetParry(true);
        return CardEffect.None;
    }

    public CardActionType GetActionType()
    {
        return CardActionType.Parry;
    }

    public void ResetTurnEffects()
    {
        character.SetParry(false);
    }
}
