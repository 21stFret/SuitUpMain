using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyGunsMod : WeaponMod
{
    private MYCharacterController myCharacterController;

    public override void Init()
    {
        base.Init();
        runUpgradeManager.ApplyMod(runMod);
        myCharacterController = BattleMech.instance.myCharacterController;
        if (myCharacterController != null)
        {
            myCharacterController.weaponFiringSlowAmount += 0.2f;
        }
    }

    public override void RemoveMods()
    {
        base.RemoveMods();
        if (myCharacterController != null)
        {
            myCharacterController.weaponFiringSlowAmount -= 0.2f; // Reset slow down effect
        }
    }
}
