using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErazerMod : WeaponMod
{

    public override void Init()
    {
        base.Init();
        PlasmaGun gun = baseWeapon as PlasmaGun;
        var pierce = runMod.modifiers[0].statValue;
        runUpgradeManager.ApplyMod(runMod);
        gun.pierceCount -= (int)pierce;
    }

    public override void RemoveMods()
    {
        PlasmaGun gun = baseWeapon as PlasmaGun;
        var pierce = runMod.modifiers[0].statValue;
        gun.pierceCount += (int)pierce;
        base.RemoveMods();
    }
}

