using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModManager : MonoBehaviour
{
    public List<WeaponMod> mods;
    public MechWeapon weapon;
    public ManualWeaponController altWeapon;
    public WeaponMod currentMod;

    public void EquipMod(int modIndex)
    {
        currentMod = mods[modIndex];
        weapon.weaponMod = currentMod;
        currentMod.baseWeapon = weapon;
        altWeapon.SetAltWeaponInputs();

    }
}
