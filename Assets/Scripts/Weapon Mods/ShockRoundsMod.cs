using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockRoundsMod : WeaponMod
{

    public override void Init()
    {
        base.Init();
        float StunTime = runMod.modifiers[0].statValue;
        float shockDamage = runMod.modifiers[1].statValue;
        Shotgun gun = baseWeapon as Shotgun;
        gun.stunTime = StunTime;
        gun.shockRounds = true;
        gun.shockDamage = shockDamage;
    }
}

