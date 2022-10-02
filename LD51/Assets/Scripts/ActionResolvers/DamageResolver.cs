using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageResolver : MonoBehaviour, IActionResolver
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
        character.TakeDamage(actionAmount);
    }

    public void ResolveSelfAction(int actionAmount)
    {

    }

    public CardActionType GetActionType()
    {
        return CardActionType.Attack;
    }

    public void ResetTurnEffects()
    {
    }
}
