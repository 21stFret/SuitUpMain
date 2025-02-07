using FORGE3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrikeMod : WeaponMod
{
    [Header("Lightning Strike")]
    public LightningStrike strike;
    private LightningRodController lightningRodController;
    public float timer;
    public float shotTimer;
    public ParticleSystem chargeEffect;
    private bool overideFire;

    public override void Init()
    {
        base.Init();
        lightningRodController = baseWeapon.GetComponent<LightningRodController>();
        lightningRodController.arcOverride = true;
        strike.damage = damage * (runMod.modifiers[0].statValue/100);
    }

    public void Update()
    {
        if (!overideFire)
        {
            return;
        }
        timer += Time.deltaTime;
        Vector3 pos = lightningRodController.lightning.rayImpact.position;
        strike.transform.position = pos;
        chargeEffect.transform.position = pos;
        if (timer <= shotTimer)
        {
            return;
        }

        strike.Strike(pos);
        //baseWeapon.weaponFuelManager.weaponFuel -= modFuelCost;
        timer = 0;
    }

    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
        overideFire = true;
        if (baseWeapon.weaponFuelManager.weaponFuel - modFuelCost <= 0)
        {
            return;
        }
        chargeEffect.transform.SetParent(this.transform);
        chargeEffect.Play();
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        timer = 0;
        overideFire = false;
        chargeEffect.transform.SetParent(null);
        chargeEffect.Stop();
    }
}
