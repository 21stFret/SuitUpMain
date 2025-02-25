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
    public float bonusDamage;
    public MeshRenderer meshRenderer;
    public bool autoTurret;

    private float _timer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        meshRenderer.material.SetFloat("_Flash_Strength", 0);
        bounces = 0;
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
        _animator.SetBool("HasTarget", hasTarget);

        if(hasTarget && autoTurret)
        {
            _timer += Time.deltaTime;
            if (_timer > fireRate)
            {
                weaponController.Minigun(damage + bonusDamage, bounces);
                _timer = 0.0f;
            }
            return;
        }

        if(isFiring)
        {
            _animator.SetBool("HasTarget", true);
            _timer += Time.deltaTime;
            if (_timer > fireRate)
            {
                weaponController.Minigun(damage + bonusDamage, bounces);
                _timer = 0.0f;
            }
        }
        else
        {
            _timer = 0.0f;
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
    }
}
