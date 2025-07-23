using Micosmo.SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShieldAlt : MechWeapon
{
    public Animator _animator;
    public ParticleSystem shieldEffect;
    private MechHealth mechHealth;
    private bool isShieldActive = false;
    
    public override void Init()
    {
        mechHealth = BattleMech.instance.mechHealth;
        weaponFuelManager.Init(this);
    }

    void Update()
    {
        if (weaponFuelManager == null || weaponData == null)
        {
            return;
        }
        if (weaponFuelManager.weaponFuel <= 0)
        {
            isShieldActive = false;
            _animator.SetBool("ShieldActive", false);
        }
        if(weaponFuelManager.weaponFuel == weaponFuelManager.weaponFuelMax)
        {
            isShieldActive = true;
            _animator.SetBool("ShieldActive", true);
        }

    }
    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
        if (!isShieldActive)
        {
            return;
        }
        shieldEffect.Play();
        mechHealth.altShieldActive = true;
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        shieldEffect.Stop();
        mechHealth.altShieldActive = false;
    }
}
