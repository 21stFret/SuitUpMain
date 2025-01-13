using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSwapper : MonoBehaviour
{
    public MechLoader loadOut;
    public WeaponsManager weaponsManager;
    public TMP_Dropdown altWeaponDropdown;
    public TMP_Dropdown modWeaponDropdown;

    private void Start()
    {
        weaponsManager = WeaponsManager.instance;
    }

    public void SwapLoadout(int mainIndex, int altIndex)
    {
        loadOut.assaultWeapon = weaponsManager._assaultWeapons[mainIndex];
        loadOut.techWeapon = weaponsManager._techWeapons[altIndex];
    }

    public void SwapAltWeapon(int Index)
    {
        weaponsManager.altWeapon = Index;
        loadOut.EquipAltWeapon();
        SetMods();
    }

    public void SwapMainWeapon(int Index)
    {
        weaponsManager.mainWeapon = Index;
        loadOut.EquipMainWeapon();

    }

    public void SwapMod(int index)
    {
        //loadOut.weaponModManager.EquipWeaponMod(index);
    }

    private void SetMods()
    {
        loadOut.weaponModManager.LoadCurrentWeaponMods(loadOut.techWeapon.weaponType);
        modWeaponDropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < loadOut.weaponModManager.currentMods.Count; i++)
        {
            //options.Add(loadOut.weaponModManager.currentMods[i].modName);
        }
        modWeaponDropdown.AddOptions(options);
    }
}
