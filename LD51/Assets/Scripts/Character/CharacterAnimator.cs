using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterAnimator : MonoBehaviour
{
    private Animator anim;

    public CardActionType action = CardActionType.Wait;
    private CardActionType prevAction = CardActionType.Wait;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (action != prevAction)
        {
            Animate(action, CardActionType.Attack);
            prevAction = action;
        }
    }

    public void Animate(CardActionType selfAction, CardActionType otherAction)
    {
        var animation = "idle";
        if (selfAction == CardActionType.Attack)
        {
            if (otherAction == CardActionType.Defend)
            {
                animation = "slash_block";
            }
            else if (otherAction == CardActionType.Parry)
            {
                animation = "slash_parry";
            }
            else
            {
                animation = "slash";
            }
        }
        else if (selfAction == CardActionType.Heal)
        {
            animation = "swig";
        }
        else if (selfAction == CardActionType.Defend)
        {
            animation = "block";
        }
        else if (selfAction == CardActionType.Parry)
        {
            animation = "parry";
        }
        else if (selfAction == CardActionType.Stunned)
        {
            if (isAnimationPlaying("stunned") || isAnimationPlaying("slash_parry"))
            {
                return;
            }
            animation = "stunned";
        }
        anim.Play(animation);
    }

    public void Pause()
    {
        anim.speed = 0.0f;
    }

    public void Resume()
    {
        anim.speed = 1.0f;
    }

    private string[] dieAnimations = new string[] { "die_1", "die_2" };

    public void Die()
    {
        var animation = dieAnimations[Random.Range(0, dieAnimations.Length)];
        anim.Play(animation);
    }

    public void Run()
    {
        anim.Play("run");
    }

    public void Idle(bool force = false)
    {
        if (!force && (isAnimationPlaying("idle") || isAnimationPlaying("run")))
        {
            return;
        }
        anim.Play("idle");
    }

    private bool isAnimationPlaying(string name)
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
        var isPlaying = anim.GetCurrentAnimatorClipInfo(0).Where(it => it.clip.name == name).Count() > 0;
        return isPlaying;
    }
}
