using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaGun : MechWeapon
{
    public bool hasTarget;
    public GameObject gunturret;
    public ProjectileWeapon weaponController;
    private float _timer;
    public int pierceCount;
    public ParticleSystem muzzlecharge;
    public bool mirrorRounds;
    public int splitCount;

    private void Awake()
    {
        mirrorRounds = false;
    }

    void Update()
    {
        
        var target = sensor.GetNearestDetection();
        
        Vector3 location;
        if (target != null)
        {
            hasTarget = true;
            location = target.transform.position - gunturret.transform.position + aimOffest;
        }
        else
        {
            location = transform.forward;
            hasTarget = false;
        }
        gunturret.transform.forward = Vector3.Lerp(gunturret.transform.forward, location, Time.deltaTime * 10.0f);
        
        if (isFiring)
        {
            muzzlecharge.Play();
            _timer += Time.deltaTime;
            if (_timer > fireRate)
            {
                weaponController.Laser(damage, pierceCount, mirrorRounds, splitCount);
                _timer = 0.0f;
            }
        }
        else
        {
            muzzlecharge.Stop();
        }

    }
}
