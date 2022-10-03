using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationManager : MonoBehaviour
{

    public static CharacterAnimationManager main;

    public bool TriggerWalk;
    public bool TriggerPlayerDead;
    public bool TriggerEnemyDead;

    private CameraRotator camRot;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        camRot = GameObject.FindGameObjectWithTag("Environment Rotator").GetComponent<CameraRotator>();
    }

    private CharacterAnimator playerAnimator;
    private CharacterAnimator enemyAnimator;

    void Update()
    {
        if (TriggerWalk)
        {
            WalkToNextEnemy();
            TriggerWalk = false;
        }
        if (TriggerPlayerDead)
        {
            PlayPlayerDead();
            TriggerPlayerDead = false;
        }
        if (TriggerEnemyDead)
        {
            PlayEnemyDead();
            TriggerEnemyDead = false;
        }
    }

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

    public void PlayAnimations(UITimelineAction playerAction, UITimelineAction enemyAction)
    {
        FetchAnimators();
        playerAnimator.Animate(playerAction.Data.Type, enemyAction.Data.Type);
        enemyAnimator.Animate(enemyAction.Data.Type, playerAction.Data.Type);
    }

    public void PlayPlayerDead()
    {
        FetchAnimators();
        playerAnimator.Die();
    }

    public void PlayEnemyDead()
    {
        FetchAnimators();
        enemyAnimator.Die();
    }

    private float walkDuration = 2.5f;

    public void WalkToNextEnemy()
    {
        FetchAnimators();
        playerAnimator.Run();
        Invoke("StopWalk", walkDuration);
        camRot.Play();
    }

    public void StopWalk()
    {
        playerAnimator.Idle();
        camRot.Stop();
    }
}
