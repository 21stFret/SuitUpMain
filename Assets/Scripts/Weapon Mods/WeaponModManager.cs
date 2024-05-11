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
        currentMod.transform.SetParent(weapon.transform);
        currentMod.transform.localPosition = Vector3.zero;
        currentMod.transform.localRotation = Quaternion.identity;
        currentMod.Init();
        //altWeapon.SetAltWeaponInputs();
    }
}
