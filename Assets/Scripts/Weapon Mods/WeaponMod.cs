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

    public void RemoveMods()
    {
        if(runUpgradeManager == null)
        {
            runUpgradeManager = FindObjectOfType<RunUpgradeManager>();
        }
        if(runMod != null)
        {
            if (runMod.modifiers.Count > 0)
            {
                runUpgradeManager.RemoveMod(runMod);
            }
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
