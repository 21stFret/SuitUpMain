using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMod : MonoBehaviour
{
    public MechWeapon baseWeapon;
    public WeaponType modType;
    public string modName;
    public string modDescription;
    public List<Modifier> modifiers = new List<Modifier>(3);
    public float modFuelCost;
    public Sprite sprite;
    public float damage;
    public float range;

    public virtual void GetBaseWeapon(MechWeapon weapon)
    {
        baseWeapon = weapon;
    }

    public virtual void Init()
    {
        modFuelCost = baseWeapon.weaponFuelManager.weaponFuelRate;
        damage = baseWeapon.damage;
        range = baseWeapon.range;
        // set for each weapon.
        baseWeapon.weaponOverride = false;
        baseWeapon.weaponFuelManager.constantUse = true;
        // apply modifiers
        ApplyMods();
    }

    private void ApplyMods()
    {
        if(modifiers.Count > 0)
        {
            foreach(Modifier mod in modifiers)
            {
                var value = mod.modValue / 100; 

                switch(mod.modType)
                {
                    case ModifierType.Damage:     
                        damage += value * damage;
                        break;
                    case ModifierType.Range:
                        range += value * range;
                        break;
                    case ModifierType.FireRate:
                        baseWeapon.fireRate -= value * baseWeapon.fireRate;
                        break;
                    case ModifierType.FuelRate:
                        modFuelCost -= value * modFuelCost;
                        break;
                    case ModifierType.Unique:
                        break;
                }
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
