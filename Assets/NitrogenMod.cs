using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitrogenMod : WeaponMod
{
    [Header("Liquid Nitrogen")]
    public FlameTrigger flameTrigger;
    public ParticleSystem flameEffects;

    public override void Init()
    {
        base.Init();
        baseWeapon.weaponOverride = true;
        flameTrigger.InitFlameTrigger(baseWeapon.damage, baseWeapon.fireRate, 13, WeaponType.Cryo);
        runUpgradeManager.ApplyStatModifiers(runMod);
    }

    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
        flameEffects.Play();
        flameTrigger.SetCol(true);
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        flameEffects.Stop();
        flameTrigger.SetCol(false);
    }
}
