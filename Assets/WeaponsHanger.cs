using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsHanger : MonoBehaviour
{
    public Transform[] mainWeaponSlots;
    public Transform[] AltWeaponSlots;

    public void SetMainWeaponPositionToSlot(MechWeapon weapon)
    {
        weapon.gameObject.transform.localScale = Vector3.one;
        weapon.gameObject.transform.position = mainWeaponSlots[(int)weapon.weaponData.weaponIndex].position;
        weapon.gameObject.transform.rotation = mainWeaponSlots[(int)weapon.weaponData.weaponIndex].rotation;
    }

    public void SetAltWeaponPositionToSlot(MechWeapon weapon)
    {
        weapon.gameObject.transform.localScale = Vector3.one;
        weapon.gameObject.transform.position = AltWeaponSlots[(int)weapon.weaponData.weaponIndex].position;
        weapon.gameObject.transform.rotation = AltWeaponSlots[(int)weapon.weaponData.weaponIndex].rotation;
    }
}
