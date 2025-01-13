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
    public float stunTime;
    public bool canRegenFuel;
    public float regenAmount;
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

    public void ApplyMod(StatType type, float value)
    {
        ResetMods();
        switch (type)
        {
            case StatType.Health:
                canHeal = true;
                healAmount =  value;
                break;
            case StatType.Assault_Damage:
                canDamage = true;
                damage = value;
                break;
            case StatType.Fuel_Tank:
                canRegenFuel = true;
                regenAmount = value;
                break;
            case StatType.Pulse_Range:
                range = value;
                break;
            case StatType.Stun_Time:
                stunTime = value;
                canStun = true;
                break;
        }
    }

    public void ResetMods()
    {
        canHeal = false;
        canDamage = false;
        canRegenFuel = false;
        canStun = false;
    }

    private void PulseWave()
    {
        canUsePulseWave = false;
        pulseBar.SetActive(false);
        pulsewave.Play();
        AudioManager.instance.PlaySFXFromClip(pulseWaveSound);
        ApplyForceToCrawlers();
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
        float crawlerCount = 0;
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, crawlerLayer);
        float baseDam = BattleMech.instance.weaponController.mainWeaponEquiped.damage;
        float dam = baseDam * (damage/100);
        foreach (Collider collider in colliders)
        {
            Crawler crawler = collider.GetComponent<Crawler>();
            if (crawler != null)
            {
                float stunDuration = canStun? stunTime :0.2f;
                crawler.StartCoroutine(crawler.StunCralwer(stunDuration));
                crawlerCount++;
            }
            if(!canStun)
            {
                Vector3 forceDirection = (collider.transform.position - transform.position).normalized;
                Mathf.Clamp(forceDirection.y, 0.1f, 1);
                if (collider.attachedRigidbody != null)
                {
                    collider.attachedRigidbody.AddForce(forceDirection * forceMagnitude, ForceMode.Impulse);
                }
            }
            if (canDamage)
            {
                TargetHealth targetHealth = collider.GetComponent<TargetHealth>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(dam, WeaponType.AoE);
                }
            }
        }
        if (canHeal)
        {
            BattleMech.instance.RepairArmour(healAmount);
        }
        if (canRegenFuel)
        {
            weaponFuelManager.RefillFuel(regenAmount * crawlerCount);
        }
        if (crawlerCount > 8)
        {
            PlayerAchievements.instance.SetAchievement("SHOCKWAVE_1");
        }
    }
}
