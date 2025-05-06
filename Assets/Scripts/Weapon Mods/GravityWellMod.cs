using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWellMod : WeaponMod
{
    public GameObject[] voidOrbs;
    private int currentGrenade;
    private float shotTimer;
    public float grenadeShotTime;
    private bool overideFire;
    public float modFuelCostOveride;

    public override void Init()
    {
        base.Init();
        baseWeapon.weaponOverride = true;
        damage = baseWeapon.damage * (runMod.modifiers[0].statValue/100);
    }

    public void Update()
    {
        shotTimer += Time.deltaTime;
        if (!overideFire)
        {
            return;
        }

        if (shotTimer <= grenadeShotTime)
        {
            return;
        }

        voidOrbs[currentGrenade].SetActive(true);
        voidOrbs[currentGrenade].transform.position = transform.position + transform.forward;
        voidOrbs[currentGrenade].transform.rotation = transform.rotation;
        voidOrbs[currentGrenade].GetComponent<VoidGrenade>().Init(damage, range);
        currentGrenade++;
        if (currentGrenade >= voidOrbs.Length)
        {
            currentGrenade = 0;
        }
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
