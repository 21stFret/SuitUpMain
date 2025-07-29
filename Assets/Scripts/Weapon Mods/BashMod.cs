using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BashMod : WeaponMod
{
    private ShieldAlt shieldAlt;
    public float burstTimer;
    float burstTime;
    float timer;
    public bool firing;
    public bool cooldown;
    private float stunTime;
    public float stunRadius;

    public override void Init()
    {
        base.Init();
        shieldAlt = (ShieldAlt)baseWeapon;
        stunTime = runMod.modifiers[0].statValue;
    }

    // Fire Weapon
    public override void Fire()
    {
        if (!shieldAlt.isShieldActive)
        {
            return;
        }
        base.Fire();
        firing = true;

    }

    private void Update()
    {
        if (firing)
        {
            timer += Time.deltaTime;
            if (timer > burstTime)
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, stunRadius);
                foreach (var hitCollider in hitColliders)
                {
                    // Apply stun effect to enemies within the radius
                    Crawler enemy = hitCollider.GetComponent<Crawler>();
                    if (enemy != null)
                    {
                        enemy.StartCoroutine(enemy.StunCrawler(stunTime));
                    }
                }
            }
        }
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        firing = false;
        timer = 0;
    }

}
