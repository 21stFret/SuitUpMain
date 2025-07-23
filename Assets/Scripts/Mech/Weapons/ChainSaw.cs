using Micosmo.SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChainSaw : MechWeapon
{
    private Animator _animator;
    public ProjectileWeapon weaponController;
    public MeshRenderer meshRenderer;
    private List<TargetHealth> targetHealths = new List<TargetHealth>();
    public ParticleSystem sparks;
    private float _timer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        //meshRenderer.materials[1].SetColor("_EmissionColor", Color.white);
        bounces = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponent<TargetHealth>();
        if (health != null && !targetHealths.Contains(health))
        {
            targetHealths.Add(health);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var health = other.GetComponent<TargetHealth>();
        if (health != null && targetHealths.Contains(health))
        {
            targetHealths.Remove(health);
        }
    }

    void Update()
    {
        if(isFiring)
        {
            _timer += Time.deltaTime;
            if (_timer > fireRate)
            {
                // Iterate backwards to safely remove items
                for (int i = targetHealths.Count - 1; i >= 0; i--)
                {
                    var health = targetHealths[i];
                    if (health == null || !health.alive)
                    {
                        targetHealths.RemoveAt(i);
                        continue;
                    }
                    health.TakeDamage(damage, WeaponType.Melee);
                }
                _timer = 0.0f;
            }
        }
        else
        {
            _timer = 0.0f;
        }
    }
    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
        sparks.Play();
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        sparks.Stop();
        //targetHealths.Clear();
    }
}
