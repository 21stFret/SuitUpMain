using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorRoundsMod : WeaponMod
{
    PlasmaGun plasmaGun;
    private int _pierceCount;

    public override void Init()
    {
        base.Init();
        plasmaGun = baseWeapon as PlasmaGun;
        _pierceCount = plasmaGun.pierceCount;
        plasmaGun.pierceCount = 0;
        plasmaGun.mirrorRounds = true;
        plasmaGun.splitCount = (int)runMod.modifiers[0].statValue;
        runUpgradeManager.ApplyMod(runMod);
    }

    public override void RemoveMods()
    {
        base.RemoveMods();
        plasmaGun.mirrorRounds = false;
        plasmaGun.splitCount = 0;
        plasmaGun.pierceCount = _pierceCount;
    }
}
