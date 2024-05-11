using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSwapper : MonoBehaviour
{
    public MechLoadOut loadOut;
    public WeaponsManager weaponsManager;
    public Dropdown mainWeaponDropdown;

    private void Start()
    {
        weaponsManager = WeaponsManager.instance;
    }

    public void SwapLoadout(int mainIndex, int altIndex)
    {
        loadOut.mainWeapon = weaponsManager._mainWeapons[mainIndex];
        loadOut.altWeapon = weaponsManager._altWeapons[altIndex];
    }

    public void SwapAltWeapon(int Index)
    {
        weaponsManager.altWeapon = Index;
        loadOut.EquipAltWeapon();
    }

    public void SwapMainWeapon(int Index)
    {
        weaponsManager.mainWeapon = Index;
        loadOut.EquipMainWeapon();
    }

    public void SwapMod(int index)
    {
        loadOut.weaponModManager.EquipMod(index);
    }
}
