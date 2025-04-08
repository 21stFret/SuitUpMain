using Micosmo.SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minigun : MechWeapon
{
    public bool hasTarget;
    private Animator _animator;
    public ProjectileWeapon weaponController;
    public float miniGunBonusDamage;
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
        Vector3 location = transform.forward;
        if (target != null)
        {
            hasTarget = true;
            var hunter = target.GetComponent<CrawlerHunter>();
            if(hunter != null)
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
            hasTarget = false;
            location = transform.forward;
        }


        gunturret.transform.forward = Vector3.Lerp(gunturret.transform.forward, location, Time.deltaTime * autoAimSpeed);
        _animator.SetBool("HasTarget", hasTarget);

        if(hasTarget && autoTurret)
        {
            _timer += Time.deltaTime;
            if (_timer > fireRate)
            {
                weaponController.Minigun(damage + miniGunBonusDamage, bounces);
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
                weaponController.Minigun(damage + miniGunBonusDamage, bounces);
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
