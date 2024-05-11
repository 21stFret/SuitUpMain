using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameGrenadeController : WeaponMod
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
        if (!overideFire)
        {
            return;
        }
        shotTimer += Time.deltaTime;
        if (shotTimer >= baseWeapon.fireRate)
        {
            shotTimer = 0;

            flameGrenades[currentGrenade].SetActive(true);
            flameGrenades[currentGrenade].transform.position = transform.position + transform.forward;
            flameGrenades[currentGrenade].transform.rotation = transform.rotation;
            flameGrenades[currentGrenade].GetComponent<FlameGrenade>().Init(damage, range);
            currentGrenade++;
            if (currentGrenade >= flameGrenades.Length)
            {
                currentGrenade = 0;
            }
            baseWeapon.weaponFuelManager.weaponFuel -= baseWeapon.weaponFuelManager.weaponFuelRate;
        }
    }

    public override void Fire()
    {
        base.Fire();
        overideFire = true;
    }

    public override void Stop()
    {
        base.Stop();
        shotTimer = 0;
        overideFire = false;
    }
}
