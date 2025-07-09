using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompressorMod : WeaponMod
{

    public override void Init()
    {
        base.Init();
        Shotgun gun = baseWeapon as Shotgun;
        var Force = runMod.modifiers[0].statValue;
        runUpgradeManager.ApplyMod(runMod);
        float newForce = gun.force * (Force / 100);
        gun.force += newForce;
    }

    public override void RemoveMods()
    {
        Shotgun gun = baseWeapon as Shotgun;
        var Force = runMod.modifiers[0].statValue;
        float newForce = gun.force * (Force / 100);
        gun.force -= newForce;
        base.RemoveMods();
    }
}

