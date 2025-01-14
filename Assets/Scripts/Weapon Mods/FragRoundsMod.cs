using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragRoundsMod : WeaponMod
{

    public override void Init()
    {
        base.Init();
        float ExtraRounds = RunMod.modifiers[0].statValue;
        float SpreadAngle = RunMod.modifiers[1].statValue;
        Shotgun gun = baseWeapon as Shotgun;
        gun.shotsPerBurst += (int)ExtraRounds;
        float newSpread = gun.spreadAngle * (SpreadAngle / 100);
        gun.spreadAngle +=  newSpread;
    }
}

