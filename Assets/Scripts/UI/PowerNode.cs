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
    public GameObject nodeHighlight;

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
            return;
        }

        List<HashSet<ModBuildType>> usedCombinations = new List<HashSet<ModBuildType>>();
        int maxAttempts = 100; // Prevent infinite loop

        foreach (var lockItem in comboLocks)
        {
            bool uniqueCombinationFound = false;
            int attempts = 0;

            while (!uniqueCombinationFound && attempts < maxAttempts)
            {
                // Create the random lock first
                lockItem.CreateRandomLock();
                
                // Get the combination from the lock (you'll need to add a method to ComboLock to get this)
                HashSet<ModBuildType> currentCombination = GetLockCombination(lockItem);
                
                // Check if this combination already exists
                bool isDuplicate = false;
                foreach (var existingCombination in usedCombinations)
                {
                    if (currentCombination.SetEquals(existingCombination))
                    {
                        isDuplicate = true;
                        break;
                    }
                }
                
                if (!isDuplicate)
                {
                    usedCombinations.Add(currentCombination);
                    uniqueCombinationFound = true;
                }
                
                attempts++;
            }
            
            if (!uniqueCombinationFound)
            {
                Debug.LogWarning($"Could not generate unique combination for lock after {maxAttempts} attempts.");
            }
        }
    }

    // Helper method to get the combination from a ComboLock
    private HashSet<ModBuildType> GetLockCombination(ComboLock comboLock)
    {
        HashSet<ModBuildType> combination = new HashSet<ModBuildType>();
        
        // You'll need to expose the lock combination from ComboLock class
        // This assumes ComboLock has a property or method to get its required combination
        List<ModBuildType> lockCombination = comboLock.GetRequiredCombination();
        
        foreach (var buildType in lockCombination)
        {
            combination.Add(buildType);
        }
        
        return combination;
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
            var modIcon = modEntry;
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
                //RunMod runMod = _runUpgradeManager.FilterModsbyBuildType(runMods, modTypes[i])[0];
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
                currentRunMod = null;
            }
            empowered = false;
        }
    }

    public void ShowLockInfo()
    {
        nodeHighlight.SetActive(true);
        lockUIPopup.SetupLockInfo(comboLocks, runMods);
        lockUIPopup.gameObject.SetActive(true);
        if(currentRunMod != null)
        {
            modEntry.OnHoverEnter();
        }
    }

    public void HideLockInfo()
    {
        nodeHighlight.SetActive(false);
        lockUIPopup.gameObject.SetActive(false);
        if (currentRunMod != null)
        {
            modEntry.OnHoverExit();
        }
    }
}
