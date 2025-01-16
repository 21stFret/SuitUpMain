using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBulletsMod : WeaponMod
{

    public override void Init()
    {
        base.Init();
        runUpgradeManager.ApplyStatModifiers(runMod);
        baseWeapon.bounces = (int)runMod.modifiers[0].statValue;
    }

    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
    }
}
