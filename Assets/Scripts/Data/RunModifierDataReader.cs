using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class RunModifierDataReader : MonoBehaviour
{
    public void LoadFromExcell(RunUpgradeManager runUpgradeManager)
    {
        List<Dictionary<string, object>> data = CSVReader.Read("Suit Up Data - Mods");
        for (var i = 0; i < data.Count; i++)
        {
            //print(i);
            List<RunMod> runMods = new List<RunMod>();
            string buildType = SafeGetString(data[i], "Build")?.Trim();

            switch (buildType)
            {
                case "ASSAULT":
                case "ASSAULT ": // Handle extra space
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
                default:
                    Debug.LogWarning($"Unknown build type: '{buildType}' at index {i}");
                    continue; // Skip this entry
            }

            RunMod rMod = new RunMod();

            // Safely parse enum values with proper error handling
            try
            {
                rMod.modBuildType = (ModBuildType)System.Enum.Parse(typeof(ModBuildType), buildType);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Failed to parse ModBuildType from '{buildType}' at index {i}: {ex.Message}");
                continue; // Skip this entry if we can't determine the basic build type
            }

            string categoryStr = SafeGetString(data[i], "Category")?.Trim();
            try
            {
                rMod.modCategory = (ModCategory)System.Enum.Parse(typeof(ModCategory), categoryStr);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Failed to parse ModCategory from '{categoryStr}' at index {i}: {ex.Message}");
                continue; // Skip this entry if we can't determine the category
            }

            // Handle weapon type (if present)
            string weaponStr = SafeGetString(data[i], "Weapon")?.Trim();
            if (!string.IsNullOrEmpty(weaponStr))
            {
                try
                {
                    rMod.weaponType = (WeaponType)System.Enum.Parse(typeof(WeaponType), weaponStr);
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"Failed to parse WeaponType from '{weaponStr}' at index {i}: {ex.Message}");
                    // We can continue as weapon type might be optional
                }
            }

            // Handle name and description
            string nameStr = SafeGetString(data[i], "Name");
            if (!string.IsNullOrEmpty(nameStr))
            {
                rMod.modName = nameStr;
            }

            string descStr = SafeGetString(data[i], "Description");
            if (!string.IsNullOrEmpty(descStr))
            {
                rMod.modDescription = descStr;
            }

            // Load sprite
            string path = rMod.modBuildType.ToString().ToLower() + "_" + rMod.modCategory.ToString().ToLower();
            Sprite sprite = Resources.Load<Sprite>("ModIcons/" + path);
            if (sprite != null)
            {
                rMod.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"Could not load sprite for path 'ModIcons/{path}'");
            }

            // Process modifiers
            for (int j = 0; j < 3; j++)
            {
                string modifierKey = "Modifier" + j;
                if (!data[i].ContainsKey(modifierKey))
                {
                    break;
                }

                string mod = SafeGetString(data[i], modifierKey)?.Trim();
                if (!string.IsNullOrEmpty(mod))
                {
                    try
                    {
                        // Add new modifier
                        rMod.modifiers.Add(new Modifier());
                        rMod.modifiers[j].statType = (StatType)System.Enum.Parse(typeof(StatType), mod);

                        // Add values for this modifier
                        rMod.modValues.Add(new ModValues());

                        // Process the three possible values for this modifier
                        for (int k = 0; k < 3; k++)
                        {
                            string valueKey = "Value" + j + k;

                            if (data[i].ContainsKey(valueKey))
                            {
                                var valueObj = data[i][valueKey];

                                // Try multiple approaches to parse the float
                                if (valueObj != null)
                                {
                                    // First try direct conversion if it's already a number
                                    try
                                    {
                                        rMod.modValues[j].values[k] = System.Convert.ToSingle(valueObj);
                                        continue; // Success, move to next value
                                    }
                                    catch
                                    {
                                        // Fall through to string parsing
                                    }

                                    // Try parsing as string with invariant culture
                                    string valueStr = valueObj.ToString();
                                    if (float.TryParse(valueStr,
                                                      NumberStyles.Any,
                                                      CultureInfo.InvariantCulture,
                                                      out float result))
                                    {
                                        rMod.modValues[j].values[k] = result;
                                    }
                                    // Try with current culture if invariant fails
                                    else if (float.TryParse(valueStr,
                                                           NumberStyles.Any,
                                                           CultureInfo.CurrentCulture,
                                                           out result))
                                    {
                                        rMod.modValues[j].values[k] = result;
                                        Debug.Log($"Parsed '{valueStr}' with current culture to {result}");
                                    }
                                    else
                                    {
                                        Debug.LogWarning($"Failed to parse float value '{valueStr}' for {valueKey} at index {i}");
                                        rMod.modValues[j].values[k] = 0f; // Set a default value
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning($"Null value for {valueKey} at index {i}");
                                    rMod.modValues[j].values[k] = 0f;
                                }
                            }
                            else
                            {
                                Debug.LogWarning($"Missing key {valueKey} at index {i}");
                                rMod.modValues[j].values[k] = 0f;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"Error processing modifier {j} at index {i}: {ex.Message}");
                        // Skip this modifier but continue with the mod
                    }
                }
            }

            // Add the mod to the appropriate list
            if (rMod.modCategory != ModCategory.STATS)
            {
                runUpgradeManager.runModsWeaponUpgrades.Add(rMod);
            }
            else
            {
                runMods.Add(rMod);
            }
        }
    }

    // Helper method to safely get string values
    private string SafeGetString(Dictionary<string, object> data, string key)
    {
        if (data.ContainsKey(key) && data[key] != null)
        {
            return data[key].ToString();
        }
        return null;
    }
}