using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{

    public static CharacterAnimationManager main;
    private void Awake()
    {
        main = this;
    }

    private CharacterAnimator playerAnimator;
    private CharacterAnimator enemyAnimator;

    private void FetchAnimators()
    {
        if (playerAnimator == null)
        {
            playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterAnimator>();
        }
        if (enemyAnimator == null)
        {
            enemyAnimator = GameObject.FindGameObjectWithTag("Enemy").GetComponent<CharacterAnimator>();
        }
    }

    public void PlayAnimations(CardAction playerAction, CardAction enemyAction)
    {
        FetchAnimators();
        playerAnimator.Animate(playerAction.ActionType, enemyAction.ActionType);
        enemyAnimator.Animate(enemyAction.ActionType, playerAction.ActionType);
    }
}
