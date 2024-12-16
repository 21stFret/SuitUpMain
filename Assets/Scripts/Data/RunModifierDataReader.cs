using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RunModifierDataReader : MonoBehaviour
{
    public void LoadFromExcell(RunUpgradeManager runUpgradeManager)
    {
        List<Dictionary<string, object>> data = CSVReader.Read("WeaponBaseData - Mods");
        for (var i = 0; i < data.Count; i++)
        {
            //print(i);
            List<RunMod> runMods = new List<RunMod>();
            switch((string)data[i]["Build"])
            {
                case "ASSAULT ":
                    runMods = runUpgradeManager.runModsAssault;
                    break;
                case "TECH":
                    runMods = runUpgradeManager.runModsTech;
                    break;
                case "TANK":
                    runMods = runUpgradeManager.runModsTank;
                    break;
                case "AGILITY":
                    runMods = runUpgradeManager.runModsAgility;
                    break;
            }
            RunMod rMod = new RunMod();
            rMod.modBuildType = (ModBuildType)System.Enum.Parse(typeof(ModBuildType), (string)data[i]["Build"]);
            rMod.modCategory = (ModCategory)System.Enum.Parse(typeof(ModCategory), (string)data[i]["Category"]);
            if((string)data[i]["Weapon"] != "")
            {
                rMod.weaponType = (WeaponType)System.Enum.Parse(typeof(WeaponType), (string)data[i]["Weapon"]);
            }
            if ((string) data[i]["Name"] != "")
            {
                rMod.modName = (string)data[i]["Name"];
            }
            if((string)data[i]["Description"] != "")
            {
                rMod.modDescription = (string)data[i]["Description"];
            }

            string path = rMod.modBuildType.ToString().ToLower() + "_" + rMod.modCategory.ToString().ToLower();
            Sprite sprite = Resources.Load<Sprite>("ModIcons/" +path);
            if (sprite != null)
            {
                rMod.sprite = sprite;
            }


            for (int j = 0; j < 3; j++)
            {
                if (!data[i].ContainsKey("Modifier" + j))
                {
                    break;
                }
                var mod = (string)data[i]["Modifier" + j];
                if (mod != "")
                {
                    rMod.modifiers.Add(new Modifier());
                    rMod.modifiers[j].statType = (StatType)System.Enum.Parse(typeof(StatType), mod);
                    rMod.modValues.Add(new ModValues());
                    for (int k = 0; k < 3; k++)
                    {
                        string valueKey = "Value" + j + k;
                        if (data[i].ContainsKey(valueKey) && float.TryParse(data[i][valueKey].ToString(), out float result))
                        {
                            rMod.modValues[j].values[k] = result;
                        }
                        else
                        {
                            Debug.LogWarning($"Failed to parse float value for {valueKey} at index {i}");
                            rMod.modValues[j].values[k] = 0f; // Set a default value
                        }
                    }
                }
            }
            runMods.Add(rMod);
        }
    }
}
