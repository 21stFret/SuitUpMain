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
        if(RunMod.modifiers.Count > 0)
        {
            foreach(Modifier mod in RunMod.modifiers)
            {
                var value = mod.modValue / 100; 

                switch(mod.modType)
                {
                    case ModType.Damage:     
                        damage += value * damage;
                        break;
                    case ModType.Range:
                        range += value * range;
                        break;
                    case ModType.FireRate:
                        baseWeapon.fireRate -= value * baseWeapon.fireRate;
                        break;
                    case ModType.FuelRate:
                        modFuelCost -= value * modFuelCost;
                        break;
                    case ModType.Unique:
                        break;
                }
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
