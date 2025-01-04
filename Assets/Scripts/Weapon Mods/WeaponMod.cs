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
    public RunMod RunMod;

    public virtual void GetBaseWeapon(MechWeapon weapon)
    {
        baseWeapon = weapon;
    }

    public virtual void Init()
    {
        modFuelCost = baseWeapon.weaponFuelManager.weaponFuelRate;
        damage = baseWeapon.damage;
        range = baseWeapon.range;
        baseWeapon.weaponOverride = false;
        baseWeapon.weaponFuelManager.constantUse = true;
        ApplyMods();
    }

    private void ApplyMods()
    {
        foreach(Modifier mod in RunMod.modifiers)
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
