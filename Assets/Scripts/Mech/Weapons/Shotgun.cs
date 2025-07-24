using Micosmo.SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MechWeapon
{
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
        shotsPerBurst += (int)force / 2;
    }

    private void Update()
    {
        var target = sensor.GetNearestDetection();
        Vector3 location = transform.forward;
        if (target != null)
        {
            var hunter = target.GetComponent<CrawlerHunter>();
            if (hunter != null)
            {
                if (hunter.isStealthed)
                {
                    location = transform.forward;
                }
                else
                {
                    location = target.transform.position - gunturret.transform.position + aimOffest;
                }
            }
            else
            {
                location = target.transform.position - gunturret.transform.position + aimOffest;
            }

        }
        else
        {
            location = transform.forward;
        }


        gunturret.transform.forward = Vector3.Lerp(gunturret.transform.forward, location, Time.deltaTime * autoAimSpeed);

        _timer += Time.deltaTime;

        if (isFiring)
        {
            if (_timer > fireRate)
            {
                _timer = 0.0f;
                FireShotgun();
            }
        }

    }

    public void FireShotgun()
    {
        fired = true;
        for (int i = 0; i < shotsPerBurst; i++)
        {
            int newI = i - (shotsPerBurst / 2);
            weaponController.range = range;
            float spreadDamage = damage / shotsPerBurst;
            weaponController.Shotgun(spreadDamage, force, newI, spreadAngle, shotsPerBurst, i, stunTime, shockRounds, shockDamage);
        }
        _animator.SetTrigger("Recoil");
        if(PlayerProgressManager.instance != null)
        {
            StartCoroutine(MultishotKillCheck());
        }
    }
    
    private IEnumerator MultishotKillCheck()
    {
        PlayerProgressManager.instance.mutliShotKillCount = 0;
        yield return new WaitForSeconds(fireRate-0.05f);
        PlayerProgressManager.instance.CheckShotMultiKill();
    }
}

