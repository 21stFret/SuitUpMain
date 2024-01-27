using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechLoadOut : MonoBehaviour
{
    public bool battleLoadout;
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
        if (weaponsManager.mainWeapon < 0) { return; }
        mainWeapon = weaponsManager._mainWeapons[weaponsManager.mainWeapon];
        mainWeapon.gameObject.SetActive(true);
        mainWeapon.transform.SetParent(mainWeaponMount);
        mainWeapon.transform.localPosition = Vector3.zero;
        mainWeapon.transform.localRotation = Quaternion.identity;

        if(battleLoadout)
        {
            mainWeapon.Init();
        }

    }

    public void EquipAltWeapon()
    {
        RemoveAltWeapon();
        if (weaponsManager.altWeapon < 0) { return; }
        altWeapon = weaponsManager._altWeapons[weaponsManager.altWeapon];
        altWeapon.transform.SetParent(altWeaponMount);
        altWeapon.gameObject.SetActive(true);
        altWeapon.transform.localPosition = Vector3.zero;
        altWeapon.transform.localRotation = Quaternion.identity;

        if(battleLoadout)
        {
            altWeaponController.Init(altWeapon);
            altWeapon.Init();
        }

    }

    public void RemoveMainWeapon()
    {
        if(mainWeapon != null)
        {
            mainWeapon.transform.SetParent(weaponsManager.weaponsHolder.transform);
            weaponsHanger.SetMainWeaponPositionToSlot(mainWeapon);
            mainWeapon.gameObject.SetActive(false);
            mainWeapon = null;
        }
    }

    public void RemoveAltWeapon()
    {
        if (altWeapon != null)
        {
            altWeapon.transform.SetParent(weaponsManager.weaponsHolder.transform);
            weaponsHanger.SetAltWeaponPositionToSlot(altWeapon);
            //altWeapon.gameObject.SetActive(false);
            altWeapon = null;
        }
    }

}
