using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurtFireMod : WeaponMod
{
    [Header("Overheat")]
    public float burstTimer;
    float burstTime;
    float timer;
    public bool heldfire;
    public bool firing;
    public bool cooldown;

    public override void Init()
    {
        base.Init();
        burstTime = 1.5f;
        baseWeapon.fireRate /= 1.5f; // Increase fire rate by 50% for burst fire
        runUpgradeManager.ApplyMod(runMod);
    }

    public override void RemoveMods()
    {
        base.RemoveMods();
        baseWeapon.fireRate *= 1.5f; // Reset fire rate to original value
    }

    // Fire Weapon
    public override void Fire()
    {
        if (cooldown)
        {
            return;
        }

        base.Fire();
        firing = true;
        heldfire = true;
    }

    private void Update()
    {
        if (firing)
        {
            timer += Time.deltaTime;
            if (timer > burstTime)
            {
                baseWeapon.isFiring = false;
                baseWeapon.weaponOverride = true;
                cooldown = true;
                timer = burstTimer;
                firing = false;
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                cooldown = false;
                baseWeapon.weaponOverride = false;
                if (heldfire)
                {
                    firing = true;
                    baseWeapon.isFiring = true;
                }
            }
        }
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        firing = false;
        heldfire = false;
        timer = 0;
    }

}
