using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponBaseDataReader : MonoBehaviour
{
    private const int MAX_LEVELS = 10;

    public void LoadFromExcell(WeaponsManager weaponsManager)
    {
        // Updated CSV filename (removed hyphen)
        List<Dictionary<string, object>> data = CSVReader.Read("Suit Up Data - WeaponStats");
        
        // Process each weapon (columns in CSV)
        // First column (index 0) is for row labels, so we start from index 1
        // The weapon columns are: Minigun, Shotgun, Plasma, Flamer, Shocker, Cryo
        for (int weaponIndex = 0; weaponIndex < 8; weaponIndex++) // 8 weapons total
        {
            BaseWeaponInfo BWD;
            if (weaponIndex < weaponsManager._assaultWeapons.Length)
            {
                BWD = weaponsManager._assaultWeapons[weaponIndex].baseWeaponInfo;
            }
            else
            {
                BWD = weaponsManager._techWeapons[weaponIndex - weaponsManager._assaultWeapons.Length].baseWeaponInfo;
            }

            // Initialize arrays
            BWD._damage = new float[MAX_LEVELS];
            BWD._fireRate = new float[MAX_LEVELS];
            BWD._range = new float[MAX_LEVELS];
            BWD._cost = new int[MAX_LEVELS];
            BWD._weaponFuelUseRate = new float[MAX_LEVELS];
            BWD._uniqueValue = new float[MAX_LEVELS];

            // Get column name for this weapon
            string columnName = data[0].Keys.ElementAt(weaponIndex + 1); // Skip first column which is row labels
            BWD.weaponName = columnName;

            // The CSV has 62 rows now, so we need to adjust our row indices
            // Assuming the first 10 rows are for damage, next 10 for fire rate, etc.
            
            // Load level-based stats
            for (int level = 0; level < MAX_LEVELS; level++)
            {
                // Assuming data is organized in blocks of 10 rows for each stat type
                BWD._damage[level] = ParseFloat(data[level][columnName]);
                BWD._fireRate[level] = ParseFloat(data[level + 10][columnName]);
                BWD._range[level] = ParseFloat(data[level + 20][columnName]); 
                BWD._uniqueValue[level] = ParseFloat(data[level + 30][columnName]); 
                BWD._cost[level] = (int)ParseFloat(data[level + 40][columnName]); 
                BWD._weaponFuelUseRate[level] = ParseFloat(data[level + 50][columnName]); 
            }

            BWD._unlockCost = (int)ParseFloat(data[MAX_LEVELS+51][columnName]);
            
            BWD.weaponDescription = data[MAX_LEVELS+50][columnName].ToString();

            // Update the weapon data
            if (weaponIndex < weaponsManager._assaultWeapons.Length)
            {
                weaponsManager._assaultWeapons[weaponIndex].baseWeaponInfo = BWD;
            }
            else
            {
                weaponsManager._techWeapons[weaponIndex - weaponsManager._assaultWeapons.Length].baseWeaponInfo = BWD;
            }
        }
    }

    private float ParseFloat(object value)
    {
        if (value == null) return 0f;
        if (float.TryParse(value.ToString(), 
            System.Globalization.NumberStyles.Any, 
            System.Globalization.CultureInfo.InvariantCulture, 
            out float result))
        {
            return result;
        }
        return 0f;
    }
}