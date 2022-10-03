using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private int MaxHP;
    [SerializeField]
    private int HP;

    public int MaxHealth { get { return MaxHP; } }
    public int Health { get { return HP; } }
    private bool shield = false;
    private bool parry = false;

    public CardEffect TakeDamage(int damage)
    {
        if (parry)
        {
            return CardEffect.Stun;
        }
        if (!shield)
        {
            // Debug.Log($"Take damage, {gameObject.name}");
            HP = Mathf.Max(0, HP - damage);
            if (HP <= 0)
            {
                return CardEffect.Killed;
            }
            return CardEffect.Damaged;
        }

        return CardEffect.None;
    }

    public void ClampHP()
    {
        HP = Mathf.Min(MaxHealth, HP);
    }

    public void SetShield(bool isShield)
    {
        shield = isShield;
    }

    public void SetParry(bool isParry)
    {
        parry = isParry;
    }

    public void Heal(int heal)
    {
        HP = HP + heal; // Mathf.Min(MaxHP, HP + heal);
    }
}
