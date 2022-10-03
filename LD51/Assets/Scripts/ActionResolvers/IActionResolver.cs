using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionResolver
{
    CardActionType GetActionType();
    CardEffect ResolveSelfAction(int actionAmount);
    CardEffect ResolveTargetAction(int actionAmount);
    void ResetTurnEffects();
}

public enum CardEffect
{
    Stun,   // Caster is stunned
    Dead,   // Caster is dead
    Killed, // Target was skilled
    Damaged,
    Healed,
    None
};
