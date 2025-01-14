using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompressorMod : WeaponMod
{

    public override void Init()
    {
        base.Init();
        Shotgun gun = baseWeapon as Shotgun;
        var Force = RunMod.modifiers[0].statValue;
        runUpgradeManager.ApplyStatModifiers(RunMod);
        float newForce = gun.force * (Force / 100);
        gun.force += newForce;
    }
}

