using Micosmo.SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MechWeapon
{
    public bool hasTarget;
    public GameObject gunturret;
    private Animator _animator;
    public ProjectileWeapon weaponController;

    private float _timer;

    public int shotsPerBurst;
    public float spreadAngle;
    public float stunTime;
    public bool shockRounds;
    public float shockDamage;
    public bool fired;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        shockRounds = false; 
    }

    private void Update()
    {
        var target = sensor.GetNearestDetection("Enemy");
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
            _timer += Time.deltaTime;
            if (_timer > fireRate)
            {
                _timer = 0.0f;
                FireShotgun();
            }
        }
        else
        {
            _timer = 0.0f;
        }

    }

    public void FireShotgun()
    {
        fired = true;
        for (int i = 0; i < shotsPerBurst; i++)
        {
            int newI = i - (shotsPerBurst / 2);
            weaponController.range = range;
            weaponController.Shotgun(damage, force, newI, spreadAngle, shotsPerBurst, i, stunTime, shockRounds, shockDamage);
        }
        _animator.SetTrigger("Recoil");
    }
}

