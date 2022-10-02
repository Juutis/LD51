using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class HealResolver : MonoBehaviour
{
    private Character character;

    void Start()
    {
        character = GetComponent<Character>();
    }

    public void ResolveTargetAction(int actionAmount)
    {
        character.Heal(actionAmount);
    }

    public void ResolveSelfAction(int actionAmount)
    {

    }

    public CardActionType GetActionType()
    {
        return CardActionType.Heal;
    }

    public void ResetTurnEffects()
    {
    }
}
