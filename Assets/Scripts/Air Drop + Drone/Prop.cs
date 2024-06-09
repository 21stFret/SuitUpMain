using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public float health;
    public float healthMax;
    private bool isDead;

    public virtual void Init()
    {
        health = healthMax;
    }

    public void TakeDamage(float damage)
    {
        // Destroy the prop
        if (isDead)
        {
            return;
        }
        health -= damage;
        if (health  <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        isDead = true;
        print(gameObject.name + " has died");
    }
}
