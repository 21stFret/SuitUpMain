using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    public TargetHealth targetHealth;
    public GameObject UI;
    public WeaponController altWeaponController;
    public GameObject Chips;

    public void ToggleInvincible(bool toggle)
    {
        targetHealth.invincible = toggle;
    }

    public void ToggleUI(bool toggle)
    {
        UI.SetActive(!toggle);
    }

    public void ToggleAltWeaponAmmo(bool toggle)
    {
        if (toggle)
        {
            altWeaponController.altWeaponEquiped.weaponFuelManager.cheatlocked = true;
        }
        else
        {
            altWeaponController.altWeaponEquiped.weaponFuelManager.cheatlocked = false;
        }
    }

    public void ToggleFreeDroneUse(bool toggle)
    {
        BattleMech.instance.droneController.testingFreeUse = toggle;
    }

    public void ToggleFreeReRoll(bool toggle)
    {
        GameManager.instance.runUpgradeManager.freeReroll = toggle;
    }

    private bool richcheck = false;
    public void GetRichQuick()
    {
        if (richcheck) return;
        richcheck = true;
        CashCollector.instance.AddCash(1000000);
        CashCollector.instance.AddArtifact(100);
        PlayerSavedData.instance.topDif = 3;
        PlayerSavedData.instance.SavePlayerData();
        LogManager.instance.DiscoverAllLogs();
        print("Gave you 1 million cash and 100 artifacts!");
    }

    public void SkipLevel()
    {
        BattleManager.instance.ObjectiveComplete();
    }

    public void ToggleChips(bool toggle)
    {
        Chips.SetActive(toggle);
    }
}
