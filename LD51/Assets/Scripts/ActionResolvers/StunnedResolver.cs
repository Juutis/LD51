using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class StunnedResolver : MonoBehaviour
{
    private Character character;

    void Start()
    {
        character = GetComponent<Character>();
    }

    public void ResolveTargetAction(int actionAmount)
    {
        // set damage buff?
    }

    public void ResolveSelfAction(int actionAmount)
    {
        // play stunned animation
    }

    public CardActionType GetActionType()
    {
        return CardActionType.Stunned;
    }

    public void ResetTurnEffects()
    {
    }
}
