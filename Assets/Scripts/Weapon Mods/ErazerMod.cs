using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErazerMod : WeaponMod
{

    public override void Init()
    {
        base.Init();
        PlasmaGun gun = baseWeapon as PlasmaGun;
        var pierce = RunMod.modifiers[0].statValue;
        runUpgradeManager.ApplyStatModifiers(RunMod);
        gun.pierceCount -= (int)pierce;
    }
}

