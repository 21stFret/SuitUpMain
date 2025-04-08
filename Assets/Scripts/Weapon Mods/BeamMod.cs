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
    private bool shutdown;
    private PlasmaGun gun;

    public GameObject beamReadyLight;
    public AudioSource beamSound;
    public AudioClip beamLoop, beamopen, beamClose;

    public override void Init()
    {
        base.Init();
        gun = baseWeapon as PlasmaGun;
        baseWeapon.weaponOverride = true;
        runUpgradeManager.ApplyMod(runMod);
        beam.enabled = true;
        beam.beamDamage = baseWeapon.damage;
        beamReadyLight.SetActive(true);
        loaded = true;
        beamTimer = beamTime;
        chargeTimer = chargeTime;
        reloadTimer = reloadTime;
    }

    public void Update()
    {
        if (loaded)
        {
            if (firing)
            {
                shutdown = false;
                if(chargeTimer == chargeTime)
                {
                    beamSound.clip = beamopen;
                    beamSound.loop = false;
                    beamSound.Play();
                }
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
                if(beamTimer == beamTime)
                {
                    beamSound.clip = beamLoop;
                    beamSound.loop = true;
                    beamSound.Play();
                }
                if (beamTimer > 0)
                {
                    beamTimer -= Time.deltaTime;
                    beam.gameObject.SetActive(true);
                    beamReadyLight.SetActive(false);
                    if (beamTimer <= 0)
                    {
                        KillBeam();
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
        if (!loaded)
        {
            return;
        }
        base.Fire();
        firing = true;
        gun.muzzlecharge.Play();
    }

    public override void Stop()
    {
        base.Stop();
        firing = false;
        KillBeam();
    }

    private void KillBeam()
    {
        if(shutdown)
        {
            return;
        }
        beam.gameObject.SetActive(false);
        chargeTimer = chargeTime;
        beamTimer = beamTime;
        loaded = false;
        canfire = false;
        gun.muzzlecharge.Stop();
        beamSound.clip = beamClose;
        beamSound.loop = false;
        beamSound.Play();
        shutdown = true;
    }
}

