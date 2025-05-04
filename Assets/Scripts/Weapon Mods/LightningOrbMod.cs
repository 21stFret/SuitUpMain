using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningOrbMod : WeaponMod
{
    private LightningRodController lightningRodController;
    public GameObject[] lightningOrbs;
    private int currentGrenade;
    public float verrideshotTimer;
    private float verrideshotTimerT;
    private bool overideFire;
    public float overrideFuelCost;

    public override void Init()
    {
        base.Init();
        baseWeapon.weaponOverride = true;
        baseWeapon.weaponFuelManager.constantUse = false;
        lightningRodController = baseWeapon.GetComponent<LightningRodController>();
        modFuelCost = overrideFuelCost;
    }

    public void Update()
    {
        verrideshotTimerT += Time.deltaTime;
        if (!overideFire)
        {
            return;
        }

        if (baseWeapon.weaponFuelManager.weaponFuel - modFuelCost <= 0)
        {
            return;
        }


        if (verrideshotTimerT <= verrideshotTimer)
        {
            return;
        }
        lightningOrbs[currentGrenade].SetActive(true);
        lightningOrbs[currentGrenade].transform.position = transform.position + transform.forward;
        lightningOrbs[currentGrenade].transform.rotation = transform.rotation;
        LightningOrb lightningOrb = lightningOrbs[currentGrenade].GetComponent<LightningOrb>();
        lightningOrb.chainAmount = lightningRodController.chainAmount/2;
        lightningOrb.stunTime = lightningRodController.stunTime/2;
        lightningOrb.fireRate = lightningRodController.fireRate;
        lightningOrb.Init(baseWeapon.damage, baseWeapon.range/3);
        currentGrenade++;
        if (currentGrenade >= lightningOrbs.Length)
        {
            currentGrenade = 0;
        }
        baseWeapon.weaponFuelManager.UseFuel(modFuelCost);
        verrideshotTimerT = 0;
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
