using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameGrenadeMod : WeaponMod
{
    public GameObject[] flameGrenades;
    private int currentGrenade;
    private float shotTimer;
    public float grenadeShotTime;
    private bool overideFire;
    public float modFuelCostOveride;

    public override void Init()
    {
        base.Init();
        baseWeapon.weaponOverride = true;
        baseWeapon.weaponFuelManager.constantUse = false;
        damage = baseWeapon.damage * (runMod.modifiers[0].statValue/100);
    }

    public void Update()
    {
        if (!overideFire)
        {
            return;
        }

        if (baseWeapon.weaponFuelManager.weaponFuel - modFuelCostOveride <=0)
        {
            return;
        }

        shotTimer += Time.deltaTime;

        if (shotTimer <= grenadeShotTime)
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
        baseWeapon.weaponFuelManager.UseFuel(modFuelCostOveride);
        shotTimer = 0;
    }

    public override void Fire()
    {
        base.Fire();
        overideFire = true;
        shotTimer = grenadeShotTime;
    }

    public override void Stop()
    {
        base.Stop();
        overideFire = false;
    }
}
