using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningOrbMod : WeaponMod
{
    private LightningRodController lightningRodController;
    public GameObject[] lightningOrbs;
    private int currentGrenade;
    public float shotTimer;
    private bool overideFire;

    public override void Init()
    {
        base.Init();
        baseWeapon.weaponOverride = true;
        baseWeapon.weaponFuelManager.constantUse = false;
        lightningRodController = baseWeapon.GetComponent<LightningRodController>();
    }

    public void Update()
    {
        shotTimer += Time.deltaTime;
        if (!overideFire)
        {
            return;
        }

        if (baseWeapon.weaponFuelManager.weaponFuel - modFuelCost <= 0)
        {
            return;
        }


        if (shotTimer <= baseWeapon.fireRate)
        {
            return;
        }
        lightningOrbs[currentGrenade].SetActive(true);
        lightningOrbs[currentGrenade].transform.position = transform.position + transform.forward;
        lightningOrbs[currentGrenade].transform.rotation = transform.rotation;
        LightningOrb lightningOrb = lightningOrbs[currentGrenade].GetComponent<LightningOrb>();
        lightningOrb.chainAmount = lightningRodController.chainAmount;
        lightningOrb.stunTime = lightningRodController.stunTime;
        lightningOrb.fireRate = lightningRodController.fireRate;
        lightningOrb.Init(baseWeapon.damage, baseWeapon.range);
        currentGrenade++;
        if (currentGrenade >= lightningOrbs.Length)
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
