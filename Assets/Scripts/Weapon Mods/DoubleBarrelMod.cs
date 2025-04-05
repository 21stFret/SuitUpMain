using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleBarrelMod : WeaponMod
{
    public float reloadTime;
    float timer;
    public bool heldfire;
    public bool firing;
    public bool cooldown;
    private Shotgun gun;

    public override void Init()
    {
        base.Init();
        gun = baseWeapon as Shotgun;
        var SpreadAngle = runMod.modifiers[0].statValue;
        runUpgradeManager.ApplyMod(runMod);
        float newSpread = gun.spreadAngle * (SpreadAngle / 100);
        gun.spreadAngle += newSpread;
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
        if(firing && gun.fired)
        {
            Invoke("ShotDelay", 0.25f);
            firing = false;
            timer = reloadTime;
            baseWeapon.isFiring = false;
            baseWeapon.weaponOverride = true;
            cooldown = true;
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

    private void ShotDelay()
    {
        gun.FireShotgun();
        gun.fired = false;
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
