using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionResolver
{
    CardActionType GetActionType();
    void ResolveSelfAction(int actionAmount);
    void ResolveTargetAction(int actionAmount);
    void ResetTurnEffects();
}
