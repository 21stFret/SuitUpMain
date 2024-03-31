using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameGrenadeController : MechWeapon
{
    public GameObject[] flameGrenades;
    private int currentGrenade;

    public override void Init()
    {
        base.Init();
        //modType = WeaponType.Grenade;
    }

    public override void FireAlt()
    {
        base.Fire();
        /*
        if (baseWeapon.weaponFuelManager.weaponFuel >= modFuelCost)
        {
            baseWeapon.weaponFuelManager.weaponFuel -= modFuelCost;
            flameGrenades[currentGrenade].SetActive(true);
            flameGrenades[currentGrenade].transform.position = baseWeapon.transform.position + baseWeapon.transform.forward;
            flameGrenades[currentGrenade].transform.rotation = baseWeapon.transform.rotation;
            flameGrenades[currentGrenade].GetComponent<FlameGrenade>().Init(baseWeapon.damage, baseWeapon.range);
            currentGrenade++;
            if (currentGrenade >= flameGrenades.Length)
            {
                currentGrenade = 0;
            }
        }
        */
    }

    public override void StopAlt()
    {
        base.Stop();
    }
}
