using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHealth : MonoBehaviour
{
    public float health;
    public float healthMax;
    public GameObject flames;
    public Crawler attaker;
    public MechHealth mechHealth;
    public Cinemachine.CinemachineImpulseSource impulseSource;
    public GameObject deathEffect;
    public GameObject mainObject;
    public bool invincible;

    private void Start()
    {
        health = healthMax;
    }

    public void TakeDamage(float damage, Crawler attacker = null)
    {
        attaker = attacker;
        if (!invincible)
        {
            health -= damage;
        }

        if (mechHealth != null)
        {
            if (damage < 0)
            {
                if (health > healthMax)
                {
                    // could overheal
                    health = healthMax;
                }
                mechHealth.UpdateHealth(health, true);
                return;
            }

            mechHealth.UpdateHealth(health);

            // shakes camera
            float damagePercent = Mathf.Clamp(damage/10, 0.1f, 0.6f);
            impulseSource.GenerateImpulse(damagePercent);
        }

        AudioManager.instance.PlayHurt();

        if (health <= healthMax/2)
        {
            if(flames!= null)
            {
                flames.SetActive(true);
            }
        }


        if (health <= 0)
        {
            print(this.name + "has been killed");
            attaker.target = null;
            deathEffect.SetActive(true);
            mainObject.SetActive(false);
            if (mechHealth != null)
            {
                MechBattleController.instance.OnDie();
            }
        }
    }
}
