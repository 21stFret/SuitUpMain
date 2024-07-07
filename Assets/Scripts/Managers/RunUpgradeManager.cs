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
    CURRENCY
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
    public List<RunMod> runModsCurrency = new List<RunMod>();
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
            ModBuildType build = (ModBuildType)Random.Range(0, 5);
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
        runModsCurrency.Clear();
    }

    public void GenerateListOfUpgrades(ModBuildType build)
    {
        listMods.Clear();

        int[] cashedCats = new int[3] {-1,-1,-1};
        int cashcount = 0;
        // Select the categories
        for (int i = 0; i < 3; i++)
        {
            int rand = Random.Range(0, 6);
            if(cashedCats.Contains(rand))
            {
                i -= 1;
                continue;
            }
            modCategories[i] = (ModCategory)rand;
            ModCategory selectedCategory = modCategories[i];
            bool found = true;
            List<RunMod> selectedMod = new List<RunMod>();

            switch (build)
            {
                case ModBuildType.ASSAULT:

                    selectedMod = runModsAssault.FindAll(mod => mod.modCategory == selectedCategory);
                    break;
                case ModBuildType.TECH:

                    selectedMod = runModsTech.FindAll(mod => mod.modCategory == selectedCategory);
                    break;
                case ModBuildType.TANK:
                    selectedMod = runModsTank.FindAll(mod => mod.modCategory == selectedCategory);
                    break;
                case ModBuildType.AGILITY:
                    selectedMod = runModsAgility.FindAll(mod => mod.modCategory == selectedCategory);
                    break;
                case ModBuildType.CURRENCY:
                    selectedMod = runModsCurrency.FindAll(mod => mod.modCategory == selectedCategory);
                    break;
            }

            if (selectedCategory == ModCategory.ALT)
            {
                var modlist = new List<RunMod>();
                for (int k = 0; k < selectedMod.Count; k++)
                {

                    var mod = selectedMod[k];
                    if (mod.weaponType == WeaponsManager.instance.currentAltWeapon.weaponType)
                    {
                        modlist.Add(mod);
                    }
                }
                selectedMod = modlist;
            }

            if (selectedMod.Count == 1)
            {
                listMods.Add(selectedMod[0]);
            }
            else if (selectedMod.Count > 1)
            {
                listMods.Add(selectedMod[Random.Range(0, selectedMod.Count)]);
            }
            else
            {
                print("No mod found for category: " + selectedCategory + " in " + build);
                i -= 1;
                found = false;
            }
            if (!found)
            {
                continue;
            }
            cashedCats[cashcount] = rand;
            cashcount += 1;
        }
        // Roll for rarity
        for (int i = 0; i < listMods.Count; i++)
        {
            int rand = Random.Range(0, 100);
            if (rand <= 50)
            {
                listMods[i].rarity = 0;
            }
            if (rand > 50)
            {
                listMods[i].rarity = 1;
            }
            if (rand > 80)
            {
                listMods[i].rarity = 2;
            }
            //cycle for when more than 1 modifier is present
            for (int j = 0; j < listMods[i].modifiers.Count; j++)
            {
                listMods[i].modifiers[j].modValue = listMods[i].modValues[j].values[listMods[i].rarity];
            }
        }
        ModUI.OpenModUI(build);
    }

    public void SelectModButton(int index)
    {
        var mod = listMods[index];
        EnableModSelection(mod);
    }

    public void EnableModSelection(RunMod mod)
    {
        if (mod.modBuildType == ModBuildType.CURRENCY)
        {
            var modifier = mod.modifiers;
            switch (mod.modDescription)
            {
                case "Gold":
                    CashCollector.instance.AddCash((int)modifier[0].modValue);
                    break;
                case "Artifact":
                    CashCollector.instance.AddArtifact((int)modifier[0].modValue);
                    break;
                case "Hatchlings":
                    CashCollector.instance.AddCrawlerPart((int)modifier[0].modValue);
                    break;
            }
            CashCollector.instance.AddCash(100);
        }
        else
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


