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
    AGILITY
}

public enum ModCategory
{
    MAIN,
    ALT,
    DRONE,
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
    public List<RunMod> listMods = new List<RunMod>();
    public List<RunMod> currentEquipedMods = new List<RunMod>();
    public RunModifierDataReader runModifierDataReader;
    [HideInInspector]
    public List<ModBuildType> randomlySelectedBuilds = new List<ModBuildType>();

    [InspectorButton("OnButtonClicked")]
    public bool ReadData;

    public ModCategory OverideCategory;

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
    }

    private void ResetAllData()
    {
        runModsAssault.Clear();
        runModsTech.Clear();
        runModsTank.Clear();
        runModsAgility.Clear();
    }

    public void GenerateListOfUpgradesFromAll(ModBuildType build)
    {
        listMods.Clear();
        int maxAttempts = 10; // Prevent infinite loop

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
            default:
                break;
        }

        // Randomly select the Mods
        for (int i = 0; i < 3 && maxAttempts > 0;)
        {
            if (selectedMods.Count > 0)
            {
                RunMod mod = selectedMods[Random.Range(0, selectedMods.Count)];

                if (listMods.Contains(mod))
                {
                    continue;
                }

                if (OverideCategory != ModCategory.Default)
                {
                    if (mod.modCategory != OverideCategory)
                    {
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

                if (listMods.Contains(mod))
                {
                    continue;
                }

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

        // Roll for rarity
        foreach (var mod in listMods)
        {
            SetModRarity(mod);
        }

        ModUI.OpenModUI(build);
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
        RunMod mod = mods.Find(mod => mod.weaponType == WeaponsManager.instance.currentAltWeapon.weaponType);
        return mod;
    }

    private RunMod FilterMainWeaponMods(List<RunMod> mods)
    {
        RunMod mod = mods.Find(mod => mod.weaponType == WeaponsManager.instance.currentMainWeapon.weaponType);
        return mod;
    }

    private void SetModRarity(RunMod mod)
    {
        int rand = Random.Range(0, 100);
        if (rand <= 50)
            mod.rarity = 0;
        else if (rand <= 80)
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
        var mod = listMods[index];
        EnableModSelection(mod);
    }

    public void EnableModSelection(RunMod mod)
    {
        if(mod.modCategory != ModCategory.STATS)
        {
            if (currentEquipedMods.Contains(mod))
            {
                var _mod = currentEquipedMods.Find(m => m == mod);
                if (_mod.rarity >= mod.rarity)
                {
                    Debug.Log($"Already equipped a better rarity. Current: {_mod.rarity}, New: {mod.rarity}");
                    return;
                }
                // Remove the old mod if we're upgrading
                RemoveMod(_mod);
            }
        }

        currentEquipedMods.Add(mod);

        switch (mod.modCategory)
        {
            case ModCategory.MAIN:
                WeaponMod MWmod = weaponModManager.FindModByName(mod.modName);
                weaponModManager.EquipWeaponMod(MWmod);
                break;
            case ModCategory.ALT:
                WeaponMod Wmod = weaponModManager.FindModByName(mod.modName);
                weaponModManager.EquipWeaponMod(Wmod);
                break;
            case ModCategory.DRONE:
                // Implement drone mod logic if needed
                break;
            case ModCategory.PULSE:
                for (int i = 0; i < mod.modifiers.Count; i++)
                {
                    var modifier = mod.modifiers[i];
                    pulseShockwave.ApplyMod(modifier.statType, modifier.statValue);
                }
                break;
            case ModCategory.DASH:
                // Implement dash mod logic if needed
                break;
            case ModCategory.STATS:
                ApplyStatModifiers(mod);
                break;
        }

        ModUI.CloseModUI();
        GameManager.instance.SpawnPortalsToNextRoom();
    }

    private void ApplyStatModifiers(RunMod mod)
    {
        foreach (var modifier in mod.modifiers)
        {
            statManager.AddMultiplier(modifier.statType, modifier.statValue);
        }
    }

    private void RemoveMod(RunMod mod)
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
                ApplyStatModifiers(mod);
            }
            // Re-apply other mod types if necessary
        }
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


