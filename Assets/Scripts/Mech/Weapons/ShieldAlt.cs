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
    public AudioSource audioSource;
    public AudioClip shieldActivateSound;
    public AudioClip shieldDeactivateSound;
    public AudioClip shieldLoopSound;

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
        if (weaponFuelManager.weaponFuel == weaponFuelManager.weaponFuelMax)
        {
            isShieldActive = true;
            _animator.SetBool("ShieldActive", true);
        }

    }
    // Fire Weapon
    public override void Fire()
    {
        if (!isShieldActive)
        {
            return;
        }
        base.Fire();
        shieldEffect.Play();
        mechHealth.altShieldActive = true;
        StartCoroutine(ActivateShield());

    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        shieldEffect.Stop();
        mechHealth.altShieldActive = false;
        if (audioSource != null)
        {
            audioSource.clip = shieldDeactivateSound;
            audioSource.loop = false;
            audioSource.Play();
        }
    }
    
    private IEnumerator ActivateShield()
    {
        if (audioSource != null)
        {
            audioSource.clip = shieldActivateSound;
            audioSource.Play();
        }
        yield return new WaitForSeconds(0.5f);
        if (audioSource != null)
        {
            audioSource.clip = shieldLoopSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
