using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainerMod : WeaponMod
{

    public override void Init()
    {
        base.Init();
        LightningRodController gun = baseWeapon as LightningRodController;
        int extraChains = (int)runMod.modifiers[0].statValue;
        float extraRange = runMod.modifiers[1].statValue;
        gun.chainAmount += extraChains;
        gun.range += extraRange;
    }
}

