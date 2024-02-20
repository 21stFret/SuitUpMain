using Micosmo.SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigun : MechWeapon
{
    public bool hasTarget;
    public GameObject gunturret;
    private Animator _animator;
    public ProjectileWeapon weaponController;

    public GameObject target;

    private float _timer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        target = sensor.GetNearestDetection("Enemy");

        if (target != null)
        {
            hasTarget = true;
            gunturret.transform.forward = Vector3.Lerp(gunturret.transform.forward, target.transform.position - gunturret.transform.position + aimOffest, Time.deltaTime * 10.0f);
        }
        else
        {
            hasTarget = false;
            _timer = 0.0f;
        }

        _animator.SetBool("HasTarget", hasTarget);

        
        if(hasTarget)
        {
            _timer += Time.deltaTime;
           if(_timer > speed)
            {
                weaponController.Minigun(damage);
                _timer = 0.0f;
                //print("Fired");
            }
        }
        
    }
}
