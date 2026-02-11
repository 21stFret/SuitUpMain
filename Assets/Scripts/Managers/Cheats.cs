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
    public int debugAreaType = 0;
    public int debugLevelIndex = 0;

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

    public void GoToAreaAndLevelButton()
    {
        GoToAreaAndLevel(debugAreaType, debugLevelIndex);
    }

    public void SetAreaIndex(string index)
    {
        debugAreaType = int.Parse(index);
    }
    public void SetLevelIndex(string index)
    {
        debugLevelIndex = int.Parse(index);
    }

    public void GoToAreaAndLevel(int areaType, int levelIndex)
    {
        BattleManager battleManager = BattleManager.instance;
        GameManager gameManager = GameManager.instance;

        if (battleManager == null || gameManager == null)
        {
            Debug.LogError("BattleManager or GameManager instance not found!");
            return;
        }

        // Validate area type
        if (areaType < 0 || areaType > 3)
        {
            Debug.LogError($"Invalid area type: {areaType}. Must be between 0 (Grass) and 3 (Jungle)");
            return;
        }

        // Validate the level index
        if (levelIndex < 0 || levelIndex >= battleManager.Battles.Count)
        {
            Debug.LogError($"Invalid level index: {levelIndex}. Must be between 0 and {battleManager.Battles.Count - 1}");
            return;
        }

        Debug.Log($"Skipping to area {(AreaType)areaType} level {levelIndex}");

        // End current battle
        battleManager.crawlerSpawner.EndBattle();
        battleManager.crawlerSpawner.KillAllCrawlers();

        // Set the area type
        gameManager.currentAreaType = (AreaType)areaType;
        gameManager.triggeredAreaIntro = false;
        battleManager.armyGen.SetCurrentAreaType(gameManager.currentAreaType);

        // Update the battle index
        battleManager.currentBattleIndex = levelIndex;

        // Load the next room with the new battle
        gameManager.LoadNextRoom(0.5f);
    }
}