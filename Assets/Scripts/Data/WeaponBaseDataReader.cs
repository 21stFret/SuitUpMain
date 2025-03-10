using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseDataReader : MonoBehaviour
{
    public void LoadFromExcell(WeaponsManager weaponsManager)
    {
        List<Dictionary<string, object>> data = CSVReader.Read("Suit Up Data - WeaponStats");
        for (var i = 0; i < data.Count; i++)
        {
            BaseWeaponInfo BWD;
            if (i < weaponsManager._assaultWeapons.Length)
            {
                BWD = weaponsManager._assaultWeapons[i].baseWeaponInfo;
            }
            else
            {
                BWD = weaponsManager._techWeapons[i - weaponsManager._assaultWeapons.Length].baseWeaponInfo;
            }

            BWD._damage = new float[5];
            BWD._fireRate = new float[5];
            BWD._range = new float[5];
            BWD._cost = new int[5];
            BWD._weaponFuelUseRate = new float[5];
            BWD._uniqueValue = new float[5];
            BWD.weaponName = data[i]["Weapon Name"]?.ToString() ?? "Unknown Weapon";
            BWD.weaponDescription = data[i]["weaponDescription"]?.ToString() ?? "";

            // Handle damage values
            for (int j = 0; j < 5; j++)
            {
                var damageValue = data[i]["damage " + (j + 1)];
                if (damageValue != null)
                {
                    try
                    {
                        BWD._damage[j] = System.Convert.ToSingle(damageValue);
                    }
                    catch
                    {
                        if (damageValue is string strValue)
                        {
                            if (float.TryParse(strValue,
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out float result))
                            {
                                BWD._damage[j] = result;
                            }
                            else
                            {
                                Debug.LogWarning($"Failed to parse damage value: {strValue}");
                                BWD._damage[j] = 0f;
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid damage value type: {damageValue?.GetType().Name ?? "null"}");
                            BWD._damage[j] = 0f;
                        }
                    }
                }
                else
                {
                    BWD._damage[j] = 0f;
                }
            }

            // Handle fire rate values
            for (int j = 0; j < 5; j++)
            {
                var fireRateValue = data[i]["fireRate " + (j + 1)];
                if (fireRateValue != null)
                {
                    try
                    {
                        BWD._fireRate[j] = System.Convert.ToSingle(fireRateValue);
                    }
                    catch
                    {
                        if (fireRateValue is string strValue)
                        {
                            if (float.TryParse(strValue,
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out float result))
                            {
                                BWD._fireRate[j] = result;
                            }
                            else
                            {
                                Debug.LogWarning($"Failed to parse fire rate value: {strValue}");
                                BWD._fireRate[j] = 0f;
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid fire rate value type: {fireRateValue?.GetType().Name ?? "null"}");
                            BWD._fireRate[j] = 0f;
                        }
                    }
                }
                else
                {
                    BWD._fireRate[j] = 0f;
                }
            }

            // Handle range values
            for (int j = 0; j < 5; j++)
            {
                var rangeValue = data[i]["range " + (j + 1)];
                if (rangeValue != null)
                {
                    try
                    {
                        BWD._range[j] = System.Convert.ToSingle(rangeValue);
                    }
                    catch
                    {
                        if (rangeValue is string strValue)
                        {
                            if (float.TryParse(strValue,
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out float result))
                            {
                                BWD._range[j] = result;
                            }
                            else
                            {
                                Debug.LogWarning($"Failed to parse range value: {strValue}");
                                BWD._range[j] = 0f;
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid range value type: {rangeValue?.GetType().Name ?? "null"}");
                            BWD._range[j] = 0f;
                        }
                    }
                }
                else
                {
                    BWD._range[j] = 0f;
                }
            }

            // Handle cost values - note this is only for 4 levels
            for (int j = 0; j < 4; j++)
            {
                var costValue = data[i]["cost " + (j + 1)];
                if (costValue != null)
                {
                    try
                    {
                        BWD._cost[j] = System.Convert.ToInt32(costValue);
                    }
                    catch
                    {
                        if (costValue is string strValue)
                        {
                            if (int.TryParse(strValue,
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out int result))
                            {
                                BWD._cost[j] = result;
                            }
                            else
                            {
                                Debug.LogWarning($"Failed to parse cost value: {strValue}");
                                BWD._cost[j] = 0;
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid cost value type: {costValue?.GetType().Name ?? "null"}");
                            BWD._cost[j] = 0;
                        }
                    }
                }
                else
                {
                    BWD._cost[j] = 0;
                }
            }

            // Handle weaponFuelUseRate values
            for (int j = 0; j < 5; j++)
            {
                var fuelUseValue = data[i]["weaponFuelUseRate " + (j + 1)];
                if (fuelUseValue != null)
                {
                    try
                    {
                        BWD._weaponFuelUseRate[j] = System.Convert.ToSingle(fuelUseValue);
                    }
                    catch
                    {
                        if (fuelUseValue is string strValue)
                        {
                            if (float.TryParse(strValue,
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out float result))
                            {
                                BWD._weaponFuelUseRate[j] = result;
                            }
                            else
                            {
                                Debug.LogWarning($"Failed to parse fuel use rate value: {strValue}");
                                BWD._weaponFuelUseRate[j] = 0f;
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid fuel use rate value type: {fuelUseValue?.GetType().Name ?? "null"}");
                            BWD._weaponFuelUseRate[j] = 0f;
                        }
                    }
                }
                else
                {
                    BWD._weaponFuelUseRate[j] = 0f;
                }
            }

            // Handle uniqueValue values
            for (int j = 0; j < 5; j++)
            {
                var uniqueValue = data[i]["Unique " + (j + 1)];
                if (uniqueValue != null)
                {
                    try
                    {
                        BWD._uniqueValue[j] = System.Convert.ToSingle(uniqueValue);
                    }
                    catch
                    {
                        if (uniqueValue is string strValue)
                        {
                            if (float.TryParse(strValue,
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture,
                                out float result))
                            {
                                BWD._uniqueValue[j] = result;
                            }
                            else
                            {
                                Debug.LogWarning($"Failed to parse unique value: {strValue}");
                                BWD._uniqueValue[j] = 0f;
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Invalid unique value type: {uniqueValue?.GetType().Name ?? "null"}");
                            BWD._uniqueValue[j] = 0f;
                        }
                    }
                }
                else
                {
                    BWD._uniqueValue[j] = 0f;
                }
            }

            // Handle unlock cost
            var unlockCostValue = data[i]["Unlock Cost"];
            if (unlockCostValue != null)
            {
                try
                {
                    BWD._unlockCost = System.Convert.ToInt32(unlockCostValue);
                }
                catch
                {
                    if (unlockCostValue is string strValue)
                    {
                        if (int.TryParse(strValue,
                            System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture,
                            out int result))
                        {
                            BWD._unlockCost = result;
                        }
                        else
                        {
                            Debug.LogWarning($"Failed to parse unlock cost value: {strValue}");
                            BWD._unlockCost = 0;
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Invalid unlock cost value type: {unlockCostValue?.GetType().Name ?? "null"}");
                        BWD._unlockCost = 0;
                    }
                }
            }
            else
            {
                BWD._unlockCost = 0;
            }

            // Update the weapon data
            if (i < weaponsManager._assaultWeapons.Length)
            {
                weaponsManager._assaultWeapons[i].baseWeaponInfo = BWD;
            }
            else
            {
                weaponsManager._techWeapons[i - weaponsManager._assaultWeapons.Length].baseWeaponInfo = BWD;
            }
        }
    }
}