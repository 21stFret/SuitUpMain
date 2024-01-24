using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechLoadOut : MonoBehaviour
{
    public WeaponsManager weaponsManager;
    public MechWeapon mainWeapon;
    public MechWeapon altWeapon;
    public AltWeaponController altWeaponController;
    public Transform mainWeaponMount;
    public Transform altWeaponMount;
    public WeaponsHanger weaponsHanger;

    public void Init()
    {
        weaponsManager = WeaponsManager.instance;
        EquipMainWeapon();
        EquipAltWeapon();
    }

    public void EquipMainWeapon()
    {
        RemoveMainWeapon();
        mainWeapon = weaponsManager._mainWeapons[weaponsManager.mainWeapon];
        mainWeapon.transform.SetParent(mainWeaponMount);
        mainWeapon.transform.localPosition = Vector3.zero;
        mainWeapon.transform.localRotation = Quaternion.identity;
        mainWeapon.Init();
    }

    public void EquipAltWeapon()
    {
        RemoveAltWeapon();
        altWeapon = weaponsManager._altWeapons[weaponsManager.altWeapon];
        altWeapon.transform.SetParent(altWeaponMount);
        altWeaponController.Init(altWeapon);
        altWeapon.transform.localPosition = Vector3.zero;
        altWeapon.transform.localRotation = Quaternion.identity;
    }

    public void RemoveMainWeapon()
    {
        if(mainWeapon != null)
        {
            mainWeapon.transform.SetParent(weaponsManager.weaponsHolder.transform);
            weaponsHanger.SetMainWeaponPositionToSlot(mainWeapon);
            mainWeapon = null;
        }
    }

    public void RemoveAltWeapon()
    {
        if (altWeapon != null)
        {
            altWeapon.transform.SetParent(weaponsManager.weaponsHolder.transform);
            weaponsHanger.SetAltWeaponPositionToSlot(altWeapon);
            altWeapon = null;
        }
    }

}
