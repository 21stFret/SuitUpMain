using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyConverterMod : WeaponMod
{
    public override void Init()
    {
        base.Init();
        runUpgradeManager.ApplyStatModifiers(runMod);
    }
}
