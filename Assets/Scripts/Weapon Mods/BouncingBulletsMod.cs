using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBulletsMod : WeaponMod
{

    public override void Init()
    {
        base.Init();
        runUpgradeManager.ApplyMod(runMod);
        baseWeapon.bounces = (int)runMod.modifiers[0].statValue;
    }

    public override void RemoveMods()
    {
        base.RemoveMods();
        runUpgradeManager.RemoveMod(runMod);
        baseWeapon.bounces = 0;
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
