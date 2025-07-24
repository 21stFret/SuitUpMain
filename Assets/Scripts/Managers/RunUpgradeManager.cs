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
    UPGRADE,
    Default
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
    public ModCategory OverideCategory;
    private ModBuildType currentBuildType;
    public int RerollCost;
    public bool freeReroll = false;
    private int runningModCount = 0;
    public bool upgradePickup = false;
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
                break; // No mods for upgrades, handled separately
            default:
                break;
        }

        int maxMods = 0;
        // Randomly select the Mods
        for (int i = 0; i < 3 && maxAttempts > 0;)
        {
            if (selectedMods.Count > 0)
            {
                RunMod originalMod = selectedMods[Random.Range(0, selectedMods.Count)];

                RunMod mod = CreateModCopy(originalMod);

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

                SetModRarity(mod);

                mod.ID = runningModCount;
                runningModCount++;
                listMods.Add(mod);
                selectedMods.Remove(originalMod);
                i++;
            }
            else
            {
                Debug.LogWarning($"No mod found for {build}");
                break;
            }

            maxAttempts--;
        }

        ModUI.OpenModUI(build);
    }

    private void SetModRarity(RunMod mod)
    {
        int rand = Random.Range(0, 100);
        if (rand <= 75)
            mod.rarity = 0;
        else if (rand <= 90)
            mod.rarity = 1;
        else
            mod.rarity = 2;

        if (upgradePickup)
        {
            mod.rarity++;
            if (mod.rarity > 2)
            {
                mod.rarity = 2; // Ensure rarity does not exceed 2
            }
        }
        for (int j = 0; j < mod.modifiers.Count; j++)
        {
            mod.modifiers[j].statValue = mod.modValues[j].values[mod.rarity];
        }
    }

    public void SetModRaritybyInt(RunMod mod, int rarity)
    {
        if (rarity < 0 || rarity > 2)
        {
            Debug.LogError("Rarity must be between 0 and 2");
            return;
        }
        mod.rarity = rarity;

        for (int j = 0; j < mod.modifiers.Count; j++)
        {
            mod.modifiers[j].statValue = mod.modValues[j].values[mod.rarity];
        }
    }

    #region Button Methods

    // Triggers when a mod button is clicked in the UI
    public void SelectModButton(int index)
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        var mod = listMods[index];
        UpgradeCircuitboardManager.instance.currentRunMod = mod;
        ModUI.CloseModUI();
        ModUI.OpenCircuitBoard(false);
        GameManager.instance.SpawnPortalsToNextRoom();
        upgradePickup = false; // Reset after use
    }

    public void ReRollMods()
    {
        if (freeReroll)
        {
            GenerateListOfUpgradesFromAll(currentBuildType);
            AudioManager.instance.PlaySFX(SFX.Confirm);
            return;
        }
        if (PlayerProgressManager.instance.crawlerParts >= RerollCost)
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
    #endregion

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
                break;
            case ModCategory.DASH:
                BattleMech.myCharacterController.dashModsManager.RemoveMods();
                break;
            case ModCategory.STATS:
                RemoveMod(mod);
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

    public Dictionary<StatType, float> GetAllCurrentStats()
    {
        return statManager.GetAllCurrentValues();
    }

    public Dictionary<StatType, float> GetAllCurrentMultipliers()
    {
        return statManager.GetAllCurrentMultipliers();
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

    public RunMod HasModByName(string ModName)
    {
        if (currentEquipedMods.Any(mod => mod.modName == ModName))
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

    public List<RunMod> GetWeaponModsByCategory(ModCategory category)
    {
        List<RunMod> _runModsWeaponUpgrades = runModsWeaponUpgrades.FindAll(mod => mod.modCategory == category);
        return _runModsWeaponUpgrades;
    }
    
    public List<RunMod> FilterModsbyWeapon(List<RunMod> mods, WeaponType weaponType)
    {
        return mods.Where(mod => mod.weaponType == weaponType).ToList();
    }

    public List<RunMod> FilterModsbyBuildType(List<RunMod> mods, ModBuildType buildType)
    {
        return mods.Where(mod => mod.modBuildType == buildType).ToList();
    }

    private RunMod CreateModCopy(RunMod original)
    {
        RunMod copy = new RunMod();
        copy.modCategory = original.modCategory;
        copy.modBuildType = original.modBuildType;
        copy.weaponType = original.weaponType;
        copy.sprite = original.sprite;
        copy.modName = original.modName;
        copy.modDescription = original.modDescription;
        copy.rarity = original.rarity;
        copy.ID = -1; // Will be set later

        // Deep copy modifiers
        copy.modifiers = new List<Modifier>();
        foreach (var modifier in original.modifiers)
        {
            copy.modifiers.Add(new Modifier
            {
                statType = modifier.statType,
                statValue = modifier.statValue
            });
        }

        // Deep copy modValues
        copy.modValues = new List<ModValues>();
        foreach (var modValue in original.modValues)
        {
            ModValues newModValue = new ModValues();
            for (int i = 0; i < modValue.values.Length; i++)
            {
                newModValue.values[i] = modValue.values[i];
            }
            copy.modValues.Add(newModValue);
        }

        return copy;
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


