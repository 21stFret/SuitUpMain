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


    public virtual void GetBaseWeapon(MechWeapon weapon)
    {
        baseWeapon = weapon;
    }

    public virtual void Init()
    {
        runUpgradeManager = FindObjectOfType<RunUpgradeManager>();
        assaultMod = baseWeapon.weaponFuelManager == null;
        damage = baseWeapon.damage;
        range = baseWeapon.range;
        baseWeapon.weaponOverride = false;
        if(!assaultMod)
        {
            baseWeapon.weaponFuelManager.constantUse = true;
            modFuelCost = baseWeapon.weaponFuelManager.weaponFuelRate;
        }
        ApplyMods();
    }

    private void ApplyMods()
    {
        foreach(Modifier mod in runMod.modifiers)
        {
            var value = mod.statValue / 100; 

            switch(mod.statType)
            {
                case StatType.Assault_Damage:     
                    damage += value * damage;
                    break;
                case StatType.Fire_Rate:
                    baseWeapon.fireRate -= value * baseWeapon.fireRate;
                    break;
                case StatType.Fuel_Tank:
                    modFuelCost -= value * modFuelCost;
                    break;
                case StatType.Unique:
                    break;
            }
        }
    }

    public void RemoveMods()
    {
        damage = baseWeapon.damage;
        range = baseWeapon.range;
        modFuelCost = baseWeapon.weaponFuelManager.weaponFuelRate;
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
