using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockRoundsMod : WeaponMod
{

    public override void Init()
    {
        base.Init();
        float StunTime = runMod.modifiers[0].statValue;
        float shockDamage = baseWeapon.damage * (runMod.modifiers[1].statValue / 100);
        Shotgun gun = baseWeapon as Shotgun;
        gun.stunTime += StunTime;
        gun.shockRounds = true;
        gun.shockDamage = shockDamage;
    }
    
    public override void RemoveMods()
    {
        Shotgun gun = baseWeapon as Shotgun;
        gun.stunTime -= runMod.modifiers[0].statValue;
        gun.shockRounds = false;
        gun.shockDamage = 0f;
        base.RemoveMods();
    }
}

