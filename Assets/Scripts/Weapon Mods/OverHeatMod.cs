using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverHeatMod : WeaponMod
{
    [Header("Overheat")]
    float overHeatTime;
    float timer;
    float overHeatDamage;
    float bonusDamage;
    bool firing;
    bool cooldown;
    Minigun minigun;
    public AudioClip overHeatSound;
    private Material overHeatMaterial;

    public override void Init()
    {
        base.Init();

        overHeatDamage = runMod.modifiers[0].statValue;
        overHeatTime = runMod.modifiers[1].statValue;
        bonusDamage = baseWeapon.damage * (overHeatDamage /100);
        minigun = baseWeapon as Minigun;
        if (minigun == null)
        {
            Debug.LogError("Base weapon is not a Minigun.");
            return;
        }
        else
        {
            Debug.Log($"Mini Gun found with {overHeatDamage} extra damage and {overHeatTime}'s shoooting time.");

        }
        overHeatMaterial = minigun.meshRenderer.material;
        overHeatMaterial.SetFloat("_FlashOn", 1);
    }

    // Fire Weapon
    public override void Fire()
    {
        if(cooldown)
        {
            return;
        }

        base.Fire();
        firing = true;
    }

    private void Update()
    {
        float bonusDam = 0;
        if(firing)
        {
            timer += Time.deltaTime;
            if (timer > 0.1f)
            {
                bonusDam = Mathf.Lerp(0, bonusDamage, timer / overHeatTime);
            }
            if(timer > overHeatTime)
            {
                bonusDam = 0;
                firing = false;
                baseWeapon.isFiring = false;
                baseWeapon.weaponOverride = true;
                cooldown = true;
                Stop();
                timer = 2;
                AudioManager.instance.PlaySFXFromClip(overHeatSound);
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                timer = 0;
                cooldown = false;
                baseWeapon.weaponOverride = false;
            }
        }
        minigun.miniGunBonusDamage = bonusDam;
        overHeatMaterial.SetFloat("_Flash_Strength", timer / overHeatTime);
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        firing = false;
    }
}
