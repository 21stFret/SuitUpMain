using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechLoader : MonoBehaviour
{
    public bool battleLoadout;
    [HideInInspector]
    public WeaponsManager weaponsManager;
    public MechWeapon mainWeapon;
    public MechWeapon altWeapon;
    public WeaponController weaponController;
    public Transform mainWeaponMount;
    public Transform altWeaponMount;
    public WeaponsHanger weaponsHanger;
    public WeaponModManager weaponModManager;
    public bool loadMainWeapon;
    public bool loadAltWeapon;
    public TargetHealth targetHealth;
    public MYCharacterController characterController;

    public void Init()
    {
        if(battleLoadout)
        {
            targetHealth.Init(null, GetComponent<MechHealth>());
        }

        weaponsManager = WeaponsManager.instance;
        if(loadMainWeapon)
        {
            EquipMainWeapon();
        }
        if(loadAltWeapon)
        {
            EquipAltWeapon();
        }

    }

    public void EquipMainWeapon()
    {
        RemoveMainWeapon();
        if (weaponsManager.mainWeapon < 0) { return; }
        mainWeapon = weaponsManager._mainWeapons[weaponsManager.mainWeapon];
        mainWeapon.transform.SetParent(mainWeaponMount);
        mainWeapon.transform.localPosition = Vector3.zero;
        mainWeapon.transform.localRotation = Quaternion.identity;

        if(battleLoadout)
        {
            weaponController.enabled = true;
            weaponController.Init(mainWeapon);
            mainWeapon.Init();
        }

    }

    public void EquipAltWeapon()
    {
        RemoveAltWeapon();
        if (weaponsManager.altWeapon < 0) { return; }
        altWeapon = weaponsManager._altWeapons[weaponsManager.altWeapon];
        altWeapon.weaponFuelManager = transform.GetComponent<WeaponFuelManager>();
        altWeapon.transform.SetParent(altWeaponMount);
        altWeapon.transform.localPosition = Vector3.zero;
        altWeapon.transform.localRotation = Quaternion.identity;

        if(battleLoadout)
        {
            weaponController.enabled = true;
            weaponController.Init(altWeapon);
            altWeapon.Init();
            weaponModManager.weapon = altWeapon;
            weaponModManager.altWeapon = weaponController;
        }

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
