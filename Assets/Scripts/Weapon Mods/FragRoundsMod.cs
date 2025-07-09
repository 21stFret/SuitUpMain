using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragRoundsMod : WeaponMod
{
    public override void Init()
    {
        base.Init();
        float ExtraRounds = runMod.modifiers[0].statValue;
        float SpreadAngle = runMod.modifiers[1].statValue;
        Shotgun gun = baseWeapon as Shotgun;
        gun.shotsPerBurst += (int)ExtraRounds;
        float newSpread = gun.spreadAngle * (SpreadAngle / 100);
        gun.spreadAngle += newSpread;
    }

    public override void RemoveMods()
    {
        Shotgun gun = baseWeapon as Shotgun;
        float ExtraRounds = runMod.modifiers[0].statValue;
        float SpreadAngle = runMod.modifiers[1].statValue;
        gun.shotsPerBurst -= (int)ExtraRounds;
        float newSpread = gun.spreadAngle * (SpreadAngle / 100);
        gun.spreadAngle -= newSpread;
        base.RemoveMods();
    }
}

