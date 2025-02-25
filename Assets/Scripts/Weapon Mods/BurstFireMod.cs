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
        
        burstTime = 0.1f +(baseWeapon.fireRate * 3);
        runUpgradeManager.ApplyStatModifiers(runMod);
    }

    // Fire Weapon
    public override void Fire()
    {
        if(cooldown)
        {
            return;
        }

        base.Fire();
        firing = true;
        heldfire = true;
    }

    private void Update()
    {
        if(firing)
        {
            timer += Time.deltaTime;
            if(timer > burstTime)
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
            if(timer <= 0)
            {
                timer = 0;
                cooldown = false;
                baseWeapon.weaponOverride = false;
                if(heldfire)
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
