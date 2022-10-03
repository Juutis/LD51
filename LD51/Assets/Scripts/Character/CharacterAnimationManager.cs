using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAnimationManager : MonoBehaviour
{

    public static CharacterAnimationManager main;

    public bool TriggerWalk;
    public bool TriggerPlayerDead;
    public bool TriggerEnemyDead;

    private CameraRotator camRot;

    private Transform enemyArea;
    private float enemyTriggerDistance = 0.5f;
    private UnityAction callback;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        camRot = GameObject.FindGameObjectWithTag("Environment Rotator").GetComponent<CameraRotator>();
        enemyArea = GameObject.FindGameObjectWithTag("EnemyArea").transform;
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

        if (walking)
        {
            var targetPos = enemyArea.position;
            targetPos.y = 0.0f;
            var enemyPos = enemyAnimator.transform.position;
            enemyPos.y = 0.0f;
            if (Vector3.Distance(targetPos, enemyPos) < enemyTriggerDistance)
            {
                StopWalk();
            }
        }
    }

    private void FetchAnimators()
    {
        if (playerAnimator == null)
        {
            playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterAnimator>();
        }
        enemyAnimator = GameManager.main.Enemy.GetComponent<CharacterAnimator>();
    }

    private float animationLength = 1f;
    public void PlayAnimations(UITimelineAction playerAction, UITimelineAction enemyAction, UnityAction callback)
    {
        Debug.Log("Playing animations " + playerAction.Data.Type + " and " + enemyAction.Data.Type);
        this.callback = callback;
        FetchAnimators();
        playerAnimator.Animate(playerAction.Data.Type, enemyAction.Data.Type);
        enemyAnimator.Animate(enemyAction.Data.Type, playerAction.Data.Type);
        Invoke("RunCallback", animationLength);
    }

    public void RunCallback()
    {
        callback();
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
        Invoke("HideEnemyHP", 0.2f);
    }

    public void HideEnemyHP()
    {
        UIManager.main.HideEnemyHp();
    }

    private bool walking = false;

    public void WalkToNextEnemy()
    {
        FetchAnimators();
        playerAnimator.Run();
        walking = true;
        camRot.Play();
    }

    public void StopWalk()
    {
        playerAnimator.Idle();
        camRot.Stop();
        walking = false;
    }
}
