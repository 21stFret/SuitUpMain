using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainsawSlasher : WeaponMod
{
    private float rotateSpeedBonus;

    public override void Init()
    {
        base.Init();
        // take the first modifier from the runMod and apply it to the runUpgradeManager
        RunMod runmodedit = new RunMod();
        runmodedit.modifiers = new List<Modifier>();
        Modifier modifier = runMod.modifiers[0];
        runmodedit.modifiers.Add(modifier);
        runUpgradeManager.ApplyMod(runmodedit);

        // apply the second modifier from the runMod to character controller rotation speed
        if (runMod.modifiers.Count > 1)
        {
            Modifier secondModifier = runMod.modifiers[1];
            rotateSpeedBonus = secondModifier.statValue / 100;
            BattleMech.instance.myCharacterController.rotateSpeed *= rotateSpeedBonus;
        }
    }

    public override void RemoveMods()
    {
        base.RemoveMods();
        BattleMech.instance.myCharacterController.rotateSpeed /= rotateSpeedBonus;
    }
}
