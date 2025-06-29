using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum ModBuildType
{
    ASSAULT,
    TECH,
    TANK,
    AGILITY, 
    UPGRADE
}

public enum ModCategory
{
    MAIN,
    ALT,
    PULSE,
    DASH,
    STATS,
    Default
}
public class RunUpgradeManager : MonoBehaviour
{
    public ModUI ModUI;
    public WeaponModManager weaponModManager;
    public PulseShockwave pulseShockwave;
    public StatMultiplierManager statManager; // Reference to StatMultiplierManager
    [HideInInspector]
    public List<RunMod> runModsAssault = new List<RunMod>();
    [HideInInspector]
    public List<RunMod> runModsTech = new List<RunMod>();
    [HideInInspector]
    public List<RunMod> runModsTank = new List<RunMod>();
    [HideInInspector]
    public List<RunMod> runModsAgility = new List<RunMod>();
    [HideInInspector]
    public List<RunMod> runModsWeaponUpgrades = new List<RunMod>();
    [HideInInspector]
    public List<RunMod> listMods = new List<RunMod>();
    public List<RunMod> currentEquipedMods = new List<RunMod>();
    public RunModifierDataReader runModifierDataReader;
    [HideInInspector]
    public List<ModBuildType> randomlySelectedBuilds = new List<ModBuildType>();

    private BattleMech BattleMech;

    [InspectorButton("OnButtonClicked")]
    public bool ReadData;

    public ModCategory OverideCategory;

    private ModBuildType currentBuildType;

    public int RerollCost;

    public bool freeReroll = false;
    public int runningModID = 0;

    public void SelectNextBuilds()
    {
        randomlySelectedBuilds.Clear();
        for (int i = 0; i < 3; i++)
        {
            ModBuildType build = (ModBuildType)Random.Range(0, 4);
            if (randomlySelectedBuilds.Contains(build))
            {
                i -= 1;
                continue;
            }
            randomlySelectedBuilds.Add(build);
        }
    }

    private void OnButtonClicked()
    {
        LoadData();
    }

    public void LoadData()
    {
        ResetAllData();
        runModifierDataReader.LoadFromExcell(this);
        BattleMech = BattleMech.instance;
    }

    private void ResetAllData()
    {
        runModsAssault.Clear();
        runModsTech.Clear();
        runModsTank.Clear();
        runModsAgility.Clear();
        runModsWeaponUpgrades.Clear();
    }

    public void GenerateListOfUpgradesFromAll(ModBuildType build)
    {
        Debug.LogWarning("Generating Mods for: " + build);
        listMods.Clear();
        int maxAttempts = 100; // Prevent infinite loop
        currentBuildType = build;
        List<RunMod> selectedMods = new List<RunMod>();

        switch (build)
        {
            case ModBuildType.ASSAULT:
                selectedMods = new List<RunMod>(runModsAssault);
                break;
            case ModBuildType.TECH:
                selectedMods = new List<RunMod>(runModsTech);
                break;
            case ModBuildType.TANK:
                selectedMods = new List<RunMod>(runModsTank);
                break;
            case ModBuildType.AGILITY:
                selectedMods = new List<RunMod>(runModsAgility);
                break;
            case ModBuildType.UPGRADE:
                selectedMods = new List<RunMod>(runModsWeaponUpgrades);
                selectedMods = RandomiseModType(selectedMods);
                break;
            default:
                break;
        }

        int maxMods = 0;
        // Randomly select the Mods
        for (int i = 0; i < 3 && maxAttempts > 0;)
        {
            if (selectedMods.Count > 0)
            {
                RunMod mod = new RunMod();
                mod = selectedMods[Random.Range(0, selectedMods.Count)];

                if (listMods.Contains(mod))
                {
                    selectedMods.Remove(mod);
                    maxMods++;
                    if (maxMods > 20)
                    {
                        Debug.LogWarning($"Unable to generate 3 unique mods. Only generated {listMods.Count}");
                        break;
                    }
                    continue;
                }

                if (OverideCategory != ModCategory.Default)
                {
                    if (mod.modCategory != OverideCategory)
                    {
                        maxAttempts--;
                        continue;
                    }
                }

                //can be extended to check for rare mods
                if (mod.modName == "Nano Bots")
                {
                    //roll for chance to draw
                    if (Random.Range(0, 100) > 80)
                    {
                        maxAttempts--;
                        continue;
                    }
                }
                
                if (mod.modCategory == ModCategory.ALT)
                {
                    mod = FilterAltWeaponMods(selectedMods);
                }
                if (mod.modCategory == ModCategory.MAIN)
                {
                    mod = FilterMainWeaponMods(selectedMods);
                }

                SetModRarity(mod);

                if (mod.modCategory != ModCategory.STATS)
                {
                    if (currentEquipedMods.Contains(mod))
                    {
                        var _mod = currentEquipedMods.Find(m => m.modName == mod.modName);
                        if (_mod.rarity >= mod.rarity)
                        {
                            maxAttempts--;
                            Debug.Log($"Already equipped a better or same rarity of this Mod. Load a new Mod instead.");
                            continue;
                        }
                    }
                }

                if (listMods.Contains(mod))
                {
                    selectedMods.Remove(mod);
                    maxAttempts--;
                    continue;
                }

                if(mod == null)
                {
                    Debug.LogWarning($"No mod found for {build}");
                    maxAttempts--;
                    continue;
                }

                mod.ID = runningModID++;
                listMods.Add(mod);

                i++;
            }
            else
            {
                Debug.LogWarning($"No mod found for {build}");
            }

            maxAttempts--;
        }

        if (listMods.Count < 3)
        {
            Debug.LogWarning($"Unable to generate 3 unique mods. Only generated {listMods.Count}");
        }

        ModUI.OpenModUI(build);
    }

    private List<RunMod> RandomiseModType(List<RunMod> mods)
    {
        ModCategory category = (ModCategory)Random.Range(0, 4);
        print("Selected Category: " + category);
        List<RunMod> mod = mods.FindAll(mod => mod.modCategory == category);
        if (mod.Count != 0)
        {
            return mod;
        }
        else
        {
            return null;
        }
    }

    private List<RunMod> GetModsForBuild(ModBuildType build, ModCategory category)
    {
        switch (build)
        {
            case ModBuildType.ASSAULT:
                return runModsAssault.FindAll(mod => mod.modCategory == category);
            case ModBuildType.TECH:
                return runModsTech.FindAll(mod => mod.modCategory == category);
            case ModBuildType.TANK:
                return runModsTank.FindAll(mod => mod.modCategory == category);
            case ModBuildType.AGILITY:
                return runModsAgility.FindAll(mod => mod.modCategory == category);
            default:
                Debug.LogError($"Unknown ModBuildType: {build}");
                return new List<RunMod>();
        }
    }

    private RunMod FilterAltWeaponMods(List<RunMod> mods)
    {
        List<RunMod> mod = mods.FindAll(mod => mod.weaponType == WeaponsManager.instance.currentAltWeapon.weaponType);
        if (mod.Count != 0)
        {
            var _mod = mod[Random.Range(0, mod.Count)];
            _mod.weaponType = WeaponsManager.instance.currentAltWeapon.weaponType;
            _mod.modCategory = ModCategory.ALT;
            return _mod;
        }
        else
        {
            return null;
        }
    }

    private RunMod FilterMainWeaponMods(List<RunMod> mods)
    {
        List<RunMod> mod = mods.FindAll(mod => mod.weaponType == WeaponsManager.instance.currentMainWeapon.weaponType);
        if (mod.Count != 0)
        {
            var _mod = mod[Random.Range(0, mod.Count)];
            _mod.weaponType = WeaponsManager.instance.currentMainWeapon.weaponType;
            _mod.modCategory = ModCategory.MAIN;
            return _mod;
        }
        else
        {
            return null;
        }
    }

    private void SetModRarity(RunMod mod)
    {
        int rand = Random.Range(0, 100);
        if (rand <= 50)
            mod.rarity = 0;
        else if (rand <= 85)
            mod.rarity = 1;
        else
            mod.rarity = 2;

        for (int j = 0; j < mod.modifiers.Count; j++)
        {
            mod.modifiers[j].statValue = mod.modValues[j].values[mod.rarity];
        }
    }

    public void SelectModButton(int index)
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        var mod = listMods[index];
        UpgradeCircuitboardManager.instance.currentRunMod = mod;
        UpgradeCircuitboardManager.instance.UpdateModEntry(mod);
        ModUI.CloseModUI();
        ModUI.OpenCircuitBoard();
        GameManager.instance.SpawnPortalsToNextRoom();
    }

    public void ReRollMods()
    {
        if(freeReroll)
        {
            GenerateListOfUpgradesFromAll(currentBuildType);
            AudioManager.instance.PlaySFX(SFX.Confirm);
            return;
        }
        if(PlayerProgressManager.instance.crawlerParts>=RerollCost)
        {
            GenerateListOfUpgradesFromAll(currentBuildType);
            CashCollector.instance.AddCrawlerPart(-RerollCost);
            RerollCost += 1;
            ModUI.SetRerollCost(RerollCost);
            AudioManager.instance.PlaySFX(SFX.Confirm);
        }
        else
        {
            AudioManager.instance.PlaySFX(SFX.Error);
            CashCollector.instance.ShowUI();
        }
    }

    public void SkipModSelection()
    {
        AudioManager.instance.PlaySFX(SFX.Select);
        CashCollector.instance.AddCrawlerPart(1);
        ModUI.CloseModUI();
        GameManager.instance.SpawnPortalsToNextRoom();
    }

    public void EnableModSelection(RunMod mod)
    {
        switch (mod.modCategory)
        {
            case ModCategory.MAIN:
                WeaponMod MWmod = weaponModManager.FindModByName(mod.modName);
                MWmod.runMod = mod;
                weaponModManager.EquipAssaultMod(MWmod);
                break;
            case ModCategory.ALT:
                WeaponMod Wmod = weaponModManager.FindModByName(mod.modName);
                Wmod.runMod = mod;
                weaponModManager.EquipTechWeaponMod(Wmod);
                break;
            case ModCategory.PULSE:
                pulseShockwave.ResetMods();
                for (int i = 0; i < mod.modifiers.Count; i++)
                {
                    var modifier = mod.modifiers[i];
                    pulseShockwave.ApplyMod(modifier.statType, modifier.statValue);
                }
                break;
            case ModCategory.DASH:
                for (int i = 0; i < mod.modifiers.Count; i++)
                {
                    var modifier = mod.modifiers[i];
                    BattleMech.myCharacterController.dashModsManager.ApplyMod(modifier.statType, modifier.statValue);
                }
                break;
            case ModCategory.STATS:
                ApplyMod(mod);
                break;
        }

        currentEquipedMods.Add(mod);
    }
    
    public void DisableModSelection(RunMod mod)
    {
        switch (mod.modCategory)
        {
            case ModCategory.MAIN:
                weaponModManager.RemoveAssutaltMod();
                break;
            case ModCategory.ALT:
                weaponModManager.RemoveTechMod();
                break;
            case ModCategory.PULSE:
                pulseShockwave.ResetMods();
                for (int i = 0; i < mod.modifiers.Count; i++)
                {
                    var modifier = mod.modifiers[i];
                    pulseShockwave.ApplyMod(modifier.statType, modifier.statValue);
                }
                break;
            case ModCategory.DASH:
                for (int i = 0; i < mod.modifiers.Count; i++)
                {
                    var modifier = mod.modifiers[i];
                    BattleMech.myCharacterController.dashModsManager.ApplyMod(modifier.statType, modifier.statValue);
                }
                break;
            case ModCategory.STATS:
                ApplyMod(mod);
                break;
        }

        currentEquipedMods.Add(mod);
    }

    public void ApplyMod(RunMod mod)
    {
        foreach (var modifier in mod.modifiers)
        {
            if (modifier.statType == StatType.Unique)
            {
                continue;
            }
            statManager.AddMultiplier(modifier.statType, modifier.statValue);
        }
    }

    public void RemoveMod(RunMod mod)
    {
        currentEquipedMods.Remove(mod);

        if (mod.modCategory == ModCategory.STATS)
        {
            foreach (var modifier in mod.modifiers)
            {
                statManager.RemoveMultiplier(modifier.statType, modifier.statValue);
            }
        }

        // Implement removal logic for other mod categories if needed
    }

    public void ResetAllMods()
    {
        foreach (var mod in currentEquipedMods)
        {
            RemoveMod(mod);
        }
        currentEquipedMods.Clear();

        // Reset all stat multipliers
        foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
        {
            statManager.ClearMultipliers(statType);
        }
    }

    // This method can be called to recalculate all stats if needed
    public void RecalculateAllStats()
    {
        ResetAllMods();
        foreach (var mod in currentEquipedMods)
        {
            if (mod.modCategory == ModCategory.STATS)
            {
                ApplyMod(mod);
            }
            // Re-apply other mod types if necessary
        }
    }

    public RunMod HasModByName(string ModName)
    {
        if(currentEquipedMods.Any(mod => mod.modName == ModName))
        {
            //Debug.Log("Mod is equipped");
        }
        else
        {
            //Debug.Log("Mod is not equipped");
        }
        return currentEquipedMods.Find(mod => mod.modName == ModName);
    }

    public List<RunMod> HasModsByName(string ModName)
    {
        return currentEquipedMods.FindAll(mod => mod.modName == ModName);
    }

    public RunMod GetWeaponModByName(string ModName)
    {
        return runModsWeaponUpgrades.Find(mod => mod.modName == ModName);
    }

}

[System.Serializable]
public class RunMod
{
    public ModCategory modCategory;
    public ModBuildType modBuildType;
    public WeaponType weaponType;
    public Sprite sprite;
    public string modName;
    public string modDescription;
    public List<Modifier> modifiers = new List<Modifier>(3);
    public int rarity;
    public List<ModValues> modValues = new List<ModValues>(3);
    public int ID = -1;
}

[System.Serializable]
public class ModValues
{
    public float[] values = new float[3];
    public ModValues()
    {
        values[0] = 0;
        values[1] = 0;
        values[2] = 0;
    }
}


