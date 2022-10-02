using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealResolver : MonoBehaviour
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
