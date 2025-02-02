using UnityEngine;

public class FlameController : MechWeapon
{
    [Header("Flame gun")]
    public FlameTrigger flameTrigger;

    public override void Init()
    {
        base.Init();
        weaponType = WeaponType.Flame;
        flameTrigger.InitFlameTrigger(damage, fireRate,range);
        var effect = weaponEffects.weaponEffect.main;
        effect.startSpeed = range * 2;
    }

    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
        flameTrigger.SetCol(isFiring);
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        flameTrigger.SetCol(isFiring);
    }

}
