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
        audioSource = GetComponent<AudioSource>();
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

    public AudioClip[] slash;
    public AudioClip[] block;
    public AudioClip[] swing;
    public AudioClip[] drink;
    public AudioClip[] footstep;
    public AudioClip[] parry;
    private AudioSource audioSource;

    public void PlaySlash() {
        audioSource.PlayOneShot(slash[Random.Range(0, slash.Length)]);
    } 
    public void PlayBlock() {
        audioSource.PlayOneShot(block[Random.Range(0, block.Length)]);
    }
    public void PlaySwing() {
        audioSource.PlayOneShot(swing[Random.Range(0, swing.Length)]);
    }
    public void PlayDrink() {
        audioSource.PlayOneShot(drink[Random.Range(0, drink.Length)]);
    }
    public void PlayFootStep() {
        audioSource.PlayOneShot(footstep[Random.Range(0, footstep.Length)]);
    }
    public void PlayParry() {
        audioSource.PlayOneShot(parry[Random.Range(0, parry.Length)]);
    }
}
