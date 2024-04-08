using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public TargetHealth targetHealth;
    private bool isDead;

    public void TakeDamage(float damage)
    {
        // Destroy the prop
        if (isDead)
        {
            return;
        }
        targetHealth.health -= damage;
        if (targetHealth.health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        isDead = true;
        print(targetHealth.gameObject.name + " has died");
    }
}
