using FORGE3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamMod : WeaponMod
{

    public F3DBeam beam;
    public float beamTime;
    private float beamTimer;
    public float chargeTime;
    private float chargeTimer;
    public float reloadTime;
    private float reloadTimer;
    private bool loaded;
    private bool firing;
    private bool canfire;
    private PlasmaGun gun;

    public GameObject beamReadyLight;

    public override void Init()
    {
        base.Init();
        gun = baseWeapon as PlasmaGun;
        baseWeapon.weaponOverride = true;
        runUpgradeManager.ApplyStatModifiers(RunMod);
        beam.enabled = true;
        beam.beamDamage = baseWeapon.damage;
        beamReadyLight.SetActive(true);
    }

    public void Update()
    {
        if (loaded)
        {
            if (firing)
            {
                if (chargeTimer > 0)
                {
                    chargeTimer -= Time.deltaTime;
                    if (chargeTimer <= 0)
                    {
                        canfire = true;
                    }
                }
                if (!canfire)
                {
                    return;
                }
                if (beamTimer > 0)
                {
                    beamTimer -= Time.deltaTime;
                    beam.gameObject.SetActive(true);
                    beamReadyLight.SetActive(false);
                    if (beamTimer <= 0)
                    {
                        beam.gameObject.SetActive(false);
                        chargeTimer = chargeTime;
                        loaded = false;
                        canfire = false;
                    }
                }
            }
        }
        else
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                reloadTimer = reloadTime;
                beamTimer = beamTime;
                loaded = true;
                beamReadyLight.SetActive(true);
            }
        }
    }

    public override void Fire()
    {
        base.Fire();
        firing = true;
        gun.muzzlecharge.Play();
    }

    public override void Stop()
    {
        base.Stop();
        firing = false;
        beam.gameObject.SetActive(false);
        chargeTimer = chargeTime;
        beamTimer = beamTime;
        gun.muzzlecharge.Stop();
    }
}

