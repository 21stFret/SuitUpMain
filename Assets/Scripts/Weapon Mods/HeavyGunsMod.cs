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
    }

    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
        myCharacterController.slowedAmount += myCharacterController.maxSpeed * 0.5f; // Slow down the character by 50%
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        myCharacterController.slowedAmount -= myCharacterController.maxSpeed * 0.5f;
    }
}
