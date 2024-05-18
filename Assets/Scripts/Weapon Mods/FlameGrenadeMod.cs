using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameGrenadeMod : WeaponMod
{
    public GameObject[] flameGrenades;
    private int currentGrenade;
    public float shotTimer;
    private bool overideFire;

    public override void Init()
    {
        base.Init();
        baseWeapon.weaponOverride = true;
        baseWeapon.weaponFuelManager.constantUse = false;
    }

    public void Update()
    {
        shotTimer += Time.deltaTime;
        if (!overideFire)
        {
            return;
        }

        if (baseWeapon.weaponFuelManager.weaponFuel - modFuelCost <=0)
        {
            return;
        }


        if (shotTimer <= baseWeapon.fireRate)
        {
            return;
        }
        flameGrenades[currentGrenade].SetActive(true);
        flameGrenades[currentGrenade].transform.position = transform.position + transform.forward;
        flameGrenades[currentGrenade].transform.rotation = transform.rotation;
        flameGrenades[currentGrenade].GetComponent<FlameGrenade>().Init(damage, range);
        currentGrenade++;
        if (currentGrenade >= flameGrenades.Length)
        {
            currentGrenade = 0;
        }
        baseWeapon.weaponFuelManager.weaponFuel -= modFuelCost;
        shotTimer = 0;
    }

    public override void Fire()
    {
        base.Fire();
        overideFire = true;
    }

    public override void Stop()
    {
        base.Stop();
        overideFire = false;
    }
}
