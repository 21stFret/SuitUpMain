using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsHanger : MonoBehaviour
{
    public Transform[] mainWeaponSlots;
    public Transform[] AltWeaponSlots;

    public void SetMainWeaponPositionToSlot(MechWeapon weapon)
    {
        Transform weaponT = weapon.gameObject.transform;
        weaponT.SetParent(mainWeaponSlots[(int)weapon.weaponData.weaponIndex]);
        weaponT.position = mainWeaponSlots[(int)weapon.weaponData.weaponIndex].position;
        weaponT.rotation = mainWeaponSlots[(int)weapon.weaponData.weaponIndex].rotation;
        weaponT.localScale = Vector3.one;
    }

    public void SetAltWeaponPositionToSlot(MechWeapon weapon)
    {
        Transform weaponT = weapon.gameObject.transform;
        weaponT.SetParent(AltWeaponSlots[(int)weapon.weaponData.weaponIndex]);
        weapon.gameObject.transform.position = AltWeaponSlots[(int)weapon.weaponData.weaponIndex].position;
        weapon.gameObject.transform.rotation = AltWeaponSlots[(int)weapon.weaponData.weaponIndex].rotation;
        weapon.gameObject.transform.localScale = Vector3.one;
    }
}
