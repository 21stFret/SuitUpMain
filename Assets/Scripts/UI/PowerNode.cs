using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerNode : MonoBehaviour
{
    public ModCategory modCategory;
    public List<RunMod> runMods = new List<RunMod>();
    public List<ChipSlotUI> chipSlots = new List<ChipSlotUI>();
    public List<ComboLock> comboLocks = new List<ComboLock>();
    public bool empowered = false;
    public RunMod currentRunMod;
    private RunUpgradeManager _runUpgradeManager;
    private bool _isInitialized = false;
    public ModEntry modEntry;
    public LockUIPopup lockUIPopup;

    public void Start()
    {
        _runUpgradeManager = GameManager.instance.runUpgradeManager;
        if (!_isInitialized)
        {
            Initialize();
            _isInitialized = true;
        }
        //modEntry = GetComponentInChildren<ModEntry>();
        modEntry._mod.modCategory = modCategory;
        modEntry._init = true;
    }

    public void Initialize()
    {
        SelectRandomMods();
        CreateRandomLocks();
    }

    private void CreateRandomLocks()
    {
        if (comboLocks.Count == 0)
        {
            Debug.LogWarning("No combo locks available to create random locks.");
            return; // No combo locks to process
        }

        foreach (var lockItem in comboLocks)
        {
            lockItem.CreateRandomLock();
        }
    }

    private void SelectRandomMods()
    {
        runMods.Clear();
        runMods.AddRange(_runUpgradeManager.GetWeaponModsByCategory(modCategory));
        if(modCategory == ModCategory.MAIN)
        {
            runMods = _runUpgradeManager.FilterModsbyWeapon(runMods, BattleMech.instance.weaponController.mainWeaponEquiped.weaponType);
        }
        if(modCategory == ModCategory.ALT)
        {
            runMods = _runUpgradeManager.FilterModsbyWeapon(runMods, BattleMech.instance.weaponController.altWeaponEquiped.weaponType);
        }

    }

    public void TryLocks()
    {
        List<ModBuildType> modTypes = new List<ModBuildType>();
        int rarity = -1;

        if (chipSlots.Count == 0)
        {
            Debug.LogWarning("No chip slots available to try locks.");
            return; // No chip slots to process
        }

        foreach (var chip in chipSlots)
        {
            modTypes.Add(chip.modBuildType);
            rarity += chip.currentRunMod.rarity;
        }


        rarity = Mathf.Clamp(rarity / chipSlots.Count, 0, 3); // Ensure rarity is within bounds

        for (int i = 0; i < comboLocks.Count; i++)
        {
            var modIcon = UpgradeCircuitboardManager.instance.pauseModUI.GetModEntry(modCategory);
            var lockItem = comboLocks[i];
            if (lockItem.TryUnlock(modTypes))
            {
                // Handle the case when the lock is successfully unlocked
                Debug.Log("Lock unlocked!");
                if (currentRunMod != null)
                {
                    if (currentRunMod == runMods[i])
                    {
                        modIcon.gameObject.SetActive(true);
                        return; // Already empowered with this mod
                    }
                }

                empowered = true;
                RunMod runMod = runMods[i];
                _runUpgradeManager.SetModRaritybyInt(runMod, rarity);
                _runUpgradeManager.EnableModSelection(runMod);
                currentRunMod = runMod;

                if (modIcon == null)
                {
                    Debug.LogError("Mod icon not found for: " + currentRunMod.modName);
                    return;
                }
                modIcon.SetupMod(currentRunMod);
                modIcon.gameObject.SetActive(true);
                return;
            }
            else
            {
                if (modIcon != null)
                {
                    modIcon.gameObject.SetActive(false);
                }
                // Handle the case when the lock is still locked
                Debug.Log("Lock still locked.");
            }
            if (empowered)
            {
                GameManager.instance.runUpgradeManager.DisableModSelection(currentRunMod);
            }
            empowered = false;
        }
    }

    public void ShowLockInfo()
    {
        //lockUIPopup.transform.localPosition = transform.localPosition + new Vector3(0, -50, 0);
        lockUIPopup.SetupLockInfo(comboLocks, runMods);
        lockUIPopup.gameObject.SetActive(true);
    }

    public void HideLockInfo()
    {
        lockUIPopup.gameObject.SetActive(false);
    }
}
