using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGun : MechWeapon
{
    public bool hasTarget;
    public GameObject gunturret;
    private Animator _animator;
    public ProjectileWeapon weaponController;
    private float _timer;
    public int pierceCount;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        var target = sensor.GetNearestDetection("Enemy");

        if (target != null)
        {
            hasTarget = true;
            gunturret.transform.forward = Vector3.Lerp(gunturret.transform.forward, target.transform.position - gunturret.transform.position + aimOffest, Time.deltaTime * 10.0f);
        }
        else
        {
            hasTarget = false;
            //_timer = 0.0f;
        }

        //_animator.SetBool("HasTarget", hasTarget);


        if (isFiring)
        {
            _timer += Time.deltaTime;
            if (_timer > fireRate)
            {
                weaponController.Laser(damage, pierceCount);
                _timer = 0.0f;
            }
        }

    }
}
