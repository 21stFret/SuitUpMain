using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PulseShockwave : MonoBehaviour
{
    public ParticleSystem pulsewave;
    public float range;
    public float forceMagnitude;
    public LayerMask crawlerLayer;
    private float timeElapsed;
    public float rechargeTime;
    public bool canUsePulseWave;
    public AudioClip pulseWaveSound;
    public GameObject pulseBar;

    public bool canDamage;
    public float damage;
    public bool canStun;
    public bool canRegenFuel;
    public float regenTime;
    private WeaponFuelManager weaponFuelManager;
    public bool canHeal;
    public float healAmount;

    public void PlayPulseWave(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (!canUsePulseWave)
            {
                return;
            }
            PulseWave();
        }
    }

    public void ApplyMod(ModType type, float value)
    {
        ResetMods();
        switch (type)
        {
            case ModType.Health:
                canHeal = true;
                healAmount =  value;
                break;
            case ModType.Damage:
                canDamage = true;
                damage = value;
                break;
            case ModType.FuelRate:
                canRegenFuel = true;
                regenTime = value;
                break;
        }
    }

    public void ResetMods()
    {
        canHeal = false;
        canDamage = false;
        canRegenFuel = false;
    }

    private void PulseWave()
    {
        canUsePulseWave = false;
        pulseBar.SetActive(false);
        pulsewave.Play();
        AudioManager.instance.PlaySFXFromClip(pulseWaveSound);
        ApplyForceToCrawlers();
        if(canRegenFuel)
        {
            if(weaponFuelManager==null)
            {
                weaponFuelManager = GetComponent<WeaponFuelManager>();
            }
            StartCoroutine(weaponFuelManager.BoostRecharge(regenTime));
        }
    }

    private void Update()
    {
        if (canUsePulseWave)
        {
            return;
        }

        timeElapsed += Time.deltaTime;

        if (timeElapsed >= rechargeTime)
        {
            pulseBar.SetActive(true);
            canUsePulseWave = true;
            timeElapsed = 0;
        }
    }

    private void ApplyForceToCrawlers()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, crawlerLayer);
        foreach (Collider collider in colliders)
        {
            Crawler crawler = collider.GetComponent<Crawler>();
            if (crawler != null)
            {
                crawler.StartCoroutine(crawler.StunCralwer(0.2f));
            }
            Vector3 forceDirection = (collider.transform.position - transform.position).normalized;
            Mathf.Clamp(forceDirection.y, 0.1f, 1);
            if(collider.attachedRigidbody != null)
            {
                collider.attachedRigidbody.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
            }
            if (canDamage)
            {
                TargetHealth targetHealth = collider.GetComponent<TargetHealth>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damage, WeaponType.AoE);
                }
            }

        }
        if (colliders.Length > 8)
        {
            PlayerAchievements.instance.SetAchievement("SHOCKWAVE_1");
        }
    }
}
