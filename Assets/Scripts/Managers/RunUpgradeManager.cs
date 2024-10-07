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
    STATS
}
public class RunUpgradeManager : MonoBehaviour
{
    public Pickup Pickup;
    public ModUI ModUI;
    public WeaponModManager weaponModManager;
    public PulseShockwave pulseShockwave;
    public MYCharacterController mYCharacterController;
    public List<RunMod> runModsAssault = new List<RunMod>();
    public List<RunMod> runModsTech = new List<RunMod>();
    public List<RunMod> runModsTank = new List<RunMod>();
    public List<RunMod> runModsAgility = new List<RunMod>();
    public List<RunMod> listMods = new List<RunMod>();
    public List<ModCategory> modCategories = new List<ModCategory>();
    public RunModifierDataReader runModifierDataReader;
    public List<ModBuildType> randomlySelectedBuilds = new List<ModBuildType>();

    [InspectorButton("OnButtonClicked")]
    public bool ReadData;

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

    private void LoadData()
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

    public void GenerateListOfUpgrades(ModBuildType build)
    {
        listMods.Clear();
        HashSet<int> usedCategories = new HashSet<int>();
        int maxAttempts = 10; // Prevent infinite loop

        // Select the categories
        for (int i = 0; i < 3 && maxAttempts > 0;)
        {
            int rand = Random.Range(0, 6);
            if (usedCategories.Add(rand)) // Returns true if the item was added (i.e., it wasn't already in the set)
            {
                ModCategory selectedCategory = (ModCategory)rand;
                modCategories[i] = selectedCategory;

                List<RunMod> selectedMod = GetModsForBuild(build, selectedCategory);

                if (selectedCategory == ModCategory.ALT)
                {
                    //selectedMod = FilterAltWeaponMods(selectedMod);
                }

                if (selectedMod.Count > 0)
                {
                    listMods.Add(selectedMod[Random.Range(0, selectedMod.Count)]);
                    i++;
                }
                else
                {
                    Debug.LogWarning($"No mod found for category: {selectedCategory} in {build}");
                    usedCategories.Remove(rand); // Allow this category to be selected again
                }
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

                if(mod.modCategory == ModCategory.ALT)
                {
                    mod = FilterAltWeaponMods(selectedMods);
                }
                if (mod.modCategory == ModCategory.MAIN)
                {
                    mod = FilterMainWeaponMods(selectedMods);
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
            mod.modifiers[j].modValue = mod.modValues[j].values[mod.rarity];
        }
    }

    public void SelectModButton(int index)
    {
        var mod = listMods[index];
        EnableModSelection(mod);
    }

    public void EnableModSelection(RunMod mod)
    {
        switch (mod.modCategory)
        {
            case ModCategory.MAIN:
                /*
                WeaponMod Wmod = WeaponModManager.FindModByName(mod.modName);
                WeaponModManager.EquipWeaponMod(Wmod);
                */
                break;
            case ModCategory.ALT:
                WeaponMod Wmod = weaponModManager.FindModByName(mod.modName);
                weaponModManager.EquipWeaponMod(Wmod);
                break;
            case ModCategory.DRONE:
                break;
            case ModCategory.PULSE:
                for (int i = 0; i < mod.modifiers.Count; i++)
                {
                    var modifier = mod.modifiers[i];
                    pulseShockwave.ApplyMod(modifier.modType, modifier.modValue);
                }
                break;
            case ModCategory.DASH:
                break;
            case ModCategory.STATS:
                for (int i = 0; i < mod.modifiers.Count; i++)
                {
                    var modifier = mod.modifiers[i];
                    MechStats.instance.ApplyStats(modifier.modType, modifier.modValue);
                }
                break;
        }

        ModUI.CloseModUI();
        GameManager.instance.SpawnPortalsToNextRoom();
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


