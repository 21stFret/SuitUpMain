using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModManager : MonoBehaviour
{
    public List<WeaponMod> mods;
    public MechWeapon weapon;
    public ManualWeaponController altWeapon;
    public WeaponMod currentMod;
    public List<WeaponMod> currentMods = new List<WeaponMod>();

    public void LoadCurrentWeaponMods(WeaponType type)
    {
        currentMods.Clear();
        for (int i = 0; i < mods.Count; i++)
        {
            var mod = mods[i];
            if (mod.modType == type)
            {
                currentMods.Add(mod);
            }
        }
    }

    public void EquipMod(int modIndex)
    {
        currentMod = currentMods[modIndex];
        weapon.weaponMod = currentMod;
        currentMod.baseWeapon = weapon;
        currentMod.transform.SetParent(weapon.transform);
        currentMod.transform.localPosition = Vector3.zero;
        currentMod.transform.localRotation = Quaternion.identity;
        currentMod.Init();
        //altWeapon.SetAltWeaponInputs();
    }
}
