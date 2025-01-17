﻿using UnityEngine;

public class FlameController : MechWeapon
{
    [Header("Flame gun")]
    public FlameTrigger flameTrigger;

    public override void Init()
    {
        base.Init();
        weaponType = WeaponType.Flame;
        flameTrigger.InitFlameTrigger(damage, fireRate,range);
    }

    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
        flameTrigger.SetCol(true);
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        flameTrigger.SetCol(false);
    }

}
