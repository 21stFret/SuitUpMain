using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechLoader : MonoBehaviour
{
    public bool battleLoadout;
    [HideInInspector]
    public WeaponsManager weaponsManager;
    public MechWeapon assaultWeapon;
    public MechWeapon techWeapon;
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
        assaultWeapon = weaponsManager._assaultWeapons[weaponsManager.mainWeapon];
        assaultWeapon.transform.SetParent(mainWeaponMount);
        assaultWeapon.transform.localPosition = Vector3.zero;
        assaultWeapon.transform.localRotation = Quaternion.identity;
        assaultWeapon.transform.localScale = Vector3.one;

        if(battleLoadout)
        {
            weaponController.enabled = true;
            weaponController.Init(assaultWeapon);
            assaultWeapon.Init();
        }

    }

    public void EquipAltWeapon()
    {
        RemoveAltWeapon();
        if (weaponsManager.altWeapon < 0) { return; }
        techWeapon = weaponsManager._techWeapons[weaponsManager.altWeapon];
        techWeapon.weaponFuelManager = transform.GetComponent<WeaponFuelManager>();
        techWeapon.transform.SetParent(altWeaponMount);
        techWeapon.transform.localPosition = Vector3.zero;
        techWeapon.transform.localRotation = Quaternion.identity;
        techWeapon.transform.localScale = Vector3.one;

        if(battleLoadout)
        {
            weaponController.enabled = true;
            weaponController.Init(techWeapon);
            techWeapon.Init();
            if(weaponModManager != null)
            {
                weaponModManager.assualtWeapon = assaultWeapon;
                weaponModManager.techWeapon = techWeapon;
                weaponModManager.weaponController = weaponController;
            }
        }

    }

    public void RemoveMainWeapon()
    {
        if(assaultWeapon != null)
        {
            assaultWeapon.transform.SetParent(weaponsManager.weaponsHolder.transform);
            weaponsHanger.SetMainWeaponPositionToSlot(assaultWeapon);
            assaultWeapon = null;
        }
    }

    public void RemoveAltWeapon()
    {
        if (techWeapon != null)
        {
            techWeapon.transform.SetParent(weaponsManager.weaponsHolder.transform);
            weaponsHanger.SetAltWeaponPositionToSlot(techWeapon);
            techWeapon = null;
        }
    }

}
