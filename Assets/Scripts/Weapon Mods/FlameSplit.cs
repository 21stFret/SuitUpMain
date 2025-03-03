using UnityEngine;

public class FlameSplit : WeaponMod
{
    [Header("Flame gun")]
    public FlameTrigger[] flameTrigger;
    public ParticleSystem[] flameEffects;

    public override void Init()
    {
        base.Init();
        for (int i = 0; i < flameTrigger.Length; i++)
        {
            flameTrigger[i].InitFlameTrigger(baseWeapon.damage, baseWeapon.fireRate, baseWeapon.range);
            var effect = flameEffects[i].main;
            effect.startSpeed = range * 2;
        }
        runUpgradeManager.ApplyStatModifiers(runMod);
    }

    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
        for (int i = 0; i < flameTrigger.Length; i++)
        {
            flameEffects[i].Play();
            flameTrigger[i].SetCol(true);
        }
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        for (int i = 0; i < flameTrigger.Length; i++)
        {
            flameEffects[i].Stop();
            flameTrigger[i].SetCol(false);
        }
    }

}
