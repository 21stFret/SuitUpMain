using Micosmo.SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigun : MechWeapon
{
    public LOSSensor sensor;
    public bool hasTarget;
    public GameObject gunturret;
    private Animator _animator;
    public WeaponController weaponController;

    private float _timer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        var enemy = sensor.GetNearestDetection("Enemy");
        if (enemy != null)
        {
            hasTarget = true;
            gunturret.transform.LookAt(enemy.transform);
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
