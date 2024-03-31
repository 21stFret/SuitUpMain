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

    public virtual void GetBaseWeapon(MechWeapon weapon)
    {
        baseWeapon = weapon;
    }

    public virtual void Init()
    {

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
