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
    public bool isShieldActive = false;
    public AudioSource audioSource;
    public AudioClip shieldActivateSound;
    public AudioClip shieldDeactivateSound;
    public AudioClip shieldLoopSound;

    private float shieldDamageTimer;
    private bool shieldactive;

    public override void Init()
    {
        base.Init();
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
        if (weaponFuelManager.weaponFuel >= weaponFuelManager.weaponFuelMax)
        {
            isShieldActive = true;
            _animator.SetBool("ShieldActive", true);
        }

        if (shieldactive)
        {
            shieldDamageTimer += Time.deltaTime;
            if (shieldDamageTimer >= fireRate)
            {
                shieldDamageTimer = 0;
                var colliders = Physics.OverlapSphere(BattleMech.instance.transform.position, range, BattleMech.instance.pulseShockwave.crawlerLayer);
                foreach (var hit in colliders)
                {
                    if (hit.TryGetComponent(out TargetHealth enemyHealth))
                    {
                        enemyHealth.TakeDamage(damage);
                    }
                }
            }
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
        shieldactive = true;
        mechHealth.altShieldActive = true;
        StartCoroutine(ActivateShield());
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        shieldEffect.Stop();
        shieldactive = false;
        mechHealth.altShieldActive = false;
        if (audioSource != null)
        {
            audioSource.Stop();
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
        yield return new WaitForSeconds(0.2f);
        if (audioSource != null)
        {
            audioSource.clip = shieldLoopSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
