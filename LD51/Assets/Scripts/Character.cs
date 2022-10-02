using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private int MaxHP;
    [SerializeField]
    private int HP;
    private bool shield = false;

    public void TakeDamage(int damage)
    {
        if (!shield)
        {
            // Debug.Log($"Take damage, {gameObject.name}");
            HP = Mathf.Max(0, HP - damage);
        }
    }

    public void SetShield(bool isShield)
    {
        shield = isShield;
    }

    public void Heal(int heal)
    {
        HP = Mathf.Min(MaxHP, HP + heal);
    }
}
