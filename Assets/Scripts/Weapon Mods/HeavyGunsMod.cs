using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyGunsMod : WeaponMod
{
    private MYCharacterController myCharacterController;

    public override void Init()
    {
        base.Init();
        runUpgradeManager.ApplyStatModifiers(runMod);
        myCharacterController = BattleMech.instance.myCharacterController;
    }

    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
        myCharacterController.ToggleCanMove(false);
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        myCharacterController.ToggleCanMove(true);
    }
}
