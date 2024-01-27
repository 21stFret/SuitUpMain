using Micosmo.SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigun : MechWeapon
{
    public bool hasTarget;
    public GameObject gunturret;
    private Animator _animator;
    public FORGE3dProjectileWeapon weaponController;

    public GameObject target;

    private float _timer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        target = sensor.GetStrongestDetection("Enemy");

        if (target != null)
        {
            hasTarget = true;
            //gunturret.transform.LookAt(target.transform);
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
           if(_timer > fireRate)
            {
                weaponController.Fire(damage);
                _timer = 0.0f;
                //print("Fired");
            }
        }
        
    }
}
