using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBaseDataReader : MonoBehaviour
{
    public void LoadFromExcell(WeaponsManager weaponsManager)
    {
        List<Dictionary<string, object>> data = CSVReader.Read("WeaponBaseData - WeaponStats");
        for (var i = 0; i < data.Count; i++)
        {
            BaseWeaponInfo BWD;
            if (i < weaponsManager._mainWeapons.Length)
            {
                BWD = weaponsManager._mainWeapons[i].baseWeaponInfo;
            }
            else
            {
                BWD = weaponsManager._altWeapons[i - weaponsManager._mainWeapons.Length].baseWeaponInfo;
            }

            BWD._damage = new float[5];
            BWD._fireRate = new float[5];
            BWD._range = new float[5];
            BWD._cost = new int[5];
            BWD._weaponFuelUseRate = new float[5];
            BWD._uniqueValue = new float[5];
            BWD.weaponName = data[i]["Weapon Name"].ToString();
            BWD.weaponDescription = data[i]["weaponDescription"].ToString();
            for(int j = 0; j<5; j++)
            {
                BWD._damage[j] = (float)data[i]["damage " + (j + 1)];
            }
            for (int j = 0; j < 5; j++)
            {
                BWD._fireRate[j] = (float)data[i]["fireRate " + (j + 1)];
            }
            for (int j = 0; j < 5; j++)
            {
                BWD._range[j] = (float)data[i]["range " + (j + 1)];
            }
            for (int j = 0; j < 4; j++)
            {
                BWD._cost[j] = (int)data[i]["cost " + (j + 1)];
            }
            for (int j = 0; j < 5; j++)
            {
                BWD._weaponFuelUseRate[j] = (float)data[i]["weaponFuelUseRate " + (j + 1)];
            }
            for (int j = 0; j < 5; j++)
            {
                BWD._uniqueValue[j] = (float)data[i]["Unique " + (j + 1)];
            }
            BWD._unlockCost = (int)data[i]["Unlock Cost"];
            if (i < weaponsManager._mainWeapons.Length)
            {
                weaponsManager._mainWeapons[i].baseWeaponInfo = BWD;
            }
            else
            {
                weaponsManager._altWeapons[i - weaponsManager._mainWeapons.Length].baseWeaponInfo = BWD;
            }


        }
    }
}
