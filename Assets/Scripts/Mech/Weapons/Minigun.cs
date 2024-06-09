using Micosmo.SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minigun : MechWeapon
{
    public bool hasTarget;
    public GameObject gunturret;
    private Animator _animator;
    public ProjectileWeapon weaponController;

    private float _timer;
    private bool _firing;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
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
            //_timer = 0.0f;
        }

        gunturret.transform.forward = Vector3.Lerp(gunturret.transform.forward, location, Time.deltaTime * 10.0f);
        _animator.SetBool("HasTarget", hasTarget);
        if(isFiring)
        {
            _timer += Time.deltaTime;
            if (_timer > fireRate)
            {
                weaponController.Minigun(damage);
                _timer = 0.0f;
            }
        }
        else
        {
            _timer = 0.0f;
        }
        /*
        if(hasTarget)
        {
            _timer += Time.deltaTime;
           if(_timer > fireRate)
            {
                weaponController.Minigun(damage);
                _timer = 0.0f;
            }
        }
        */
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _firing = true;
        }
        else if (context.canceled)
        {
            _firing = false;
        }
    }
}
