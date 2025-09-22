using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMod : MonoBehaviour
{
    public MechWeapon baseWeapon;
    public WeaponType modType;
    public float modFuelCost;
    public float damage;
    public float range;
    public RunMod runMod;
    public bool assaultMod;
    public RunUpgradeManager runUpgradeManager;

    public virtual void Init()
    {
        runUpgradeManager = FindObjectOfType<RunUpgradeManager>();
        assaultMod = baseWeapon.weaponFuelManager == null;
        range = baseWeapon.range;
        baseWeapon.weaponOverride = false;
        damage = baseWeapon.damage;
        if (!assaultMod)
        {
            baseWeapon.weaponFuelManager.constantUse = true;
            modFuelCost = baseWeapon.weaponFuelManager.weaponFuelRate;
        }
    }

    public virtual void RemoveMods()
    {
        if (runUpgradeManager == null)
        {
            runUpgradeManager = FindObjectOfType<RunUpgradeManager>();
        }
        if (runMod != null)
        {
            runUpgradeManager.RemoveMod(runMod);
        }
        if (baseWeapon != null)
        {
            baseWeapon.weaponOverride = false;
            baseWeapon.weaponMod = null;
            baseWeapon.damage = damage;
            baseWeapon.range = range;
        }
    }

    public virtual void Fire()
    {
        //print("Firing Weapon Mod");
    }

    public virtual void Stop()
    {
        //print("Stopping Weapon Mod");
    }
}
