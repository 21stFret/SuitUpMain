using Micosmo.SensorToolkit.Example;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashModsManager : MonoBehaviour
{
    public MYCharacterController characterController;
    public bool canDamage;
    public float chargeForce;
    public float damage;
    public bool invincible;
    [ColorUsage(true, true)]
    public Color invincibleColor;    
    [ColorUsage(true, true)]
    public Color attackColor;
    public bool hologram;
    public GameObject hologramPrefab;
    public ParticleSystem holoExplosion;
    public AudioClip hologramExplode;
    public float speedBoost;
    public float time;
    public LayerMask layerMask;
    private List<GameObject> hitTargets = new List<GameObject>();

    public void Init()
    {
        characterController = GetComponent<MYCharacterController>();
    }

    public void ApplyMod(StatType type, float value)
    {
        speedBoost = 0;
        invincible = false;
        canDamage = false;
        hologram = false;
        characterController.ResetStats();
        damage = 0;
        float percent = (Mathf.Abs(value) / 100);
        switch (type)
        {
            case StatType.Assault:
                canDamage = true;
                float dam = BattleMech.instance.weaponController.mainWeaponEquiped.damage;
                damage = dam * percent;
                break;
            case StatType.Tech:
                hologram = true;
                float _dam = BattleMech.instance.weaponController.altWeaponEquiped.damage;
                damage = _dam * percent;
                break;
            case StatType.Seconds:
                time = value;
                break;
            case StatType.Speed:
                float speed = characterController.Speed;
                speedBoost = speed * percent;
                break;
            case StatType.Dash_Cooldown:
                float cooldown = characterController.dashCooldown;
                characterController.dashCooldown = cooldown * percent;
                break;
            case StatType.Invincible:
                invincible = true;
                time = value;
                break;
        }
    }

    public void RemoveMods()
    {
        speedBoost = 0;
        invincible = false;
        canDamage = false;
        hologram = false;
        characterController.ResetStats();
        damage = 0;
        time = 0;
    }

    public void UseMod()
    {
        if (canDamage)
        {
            hitTargets.Clear();
            StartCoroutine(DoDamage());
        }
        if (hologram)
        {
            hologramPrefab.SetActive(true);
            hologramPrefab.transform.position = transform.position;
            hologramPrefab.transform.SetParent(null);
            StartCoroutine(Hologram());
        }
        if (speedBoost > 0)
        {
            StartCoroutine(SpeedBoost());
        }
    }

    public void InvincibleCall()
    {
        StartCoroutine(Invinsible());
    }

    private IEnumerator Invinsible()
    {
        try
        {
            BattleMech.instance.mechHealth.shieldMaterial.SetColor("_Flash_Color", invincibleColor);
            BattleMech.instance.mechHealth.shieldMaterial.SetFloat("_FlashOn", 1f);
            characterController.battleMech.targetHealth.invincible = true;
            yield return new WaitForSeconds(time);
        }
        finally
        {
            // This will always execute, even if the coroutine is stopped
            BattleMech.instance.mechHealth.shieldMaterial.SetFloat("_FlashOn", 0);
            characterController.battleMech.targetHealth.invincible = false;
        }
    }

    private IEnumerator DoDamage()
    {
        try
        {
            BattleMech.instance.mechHealth.shieldMaterial.SetColor("_Flash_Color", attackColor);
            BattleMech.instance.mechHealth.shieldMaterial.SetFloat("_FlashOn", 1f);
            
            while (characterController.isDodging)
            {
                var colliders = Physics.OverlapSphere(transform.position, 5f, layerMask);
                foreach (var col in colliders)
                {
                    if (hitTargets.Contains(col.gameObject))
                    {
                        continue;
                    }
                    var health = col.GetComponent<TargetHealth>();
                    var rb = col.GetComponent<Rigidbody>();
                    if (health)
                    {
                        health.TakeDamage(damage, WeaponType.AoE);
                        hitTargets.Add(col.gameObject);
                    }
                    if(rb)
                    {
                        rb.AddExplosionForce(chargeForce, transform.position, 7f, 0.5f, ForceMode.Impulse);
                    }
                }
                yield return new WaitForSeconds(0.2f);
            }
        }
        finally
        {
            // This will always execute, even if the coroutine is stopped
            BattleMech.instance.mechHealth.shieldMaterial.SetFloat("_FlashOn", 0);
        }
    }

    public IEnumerator Hologram()
    {
        var _colliders = Physics.OverlapSphere(hologramPrefab.transform.position, 20f, layerMask);
        foreach (var col in _colliders)
        {
            var crawler = col.GetComponent<Crawler>();
            if (crawler)
            {
                crawler.SetTarget(hologramPrefab.transform);
            }
        }
        yield return new WaitForSeconds(time);
        holoExplosion.transform.SetParent(null);
        holoExplosion.transform.position = hologramPrefab.transform.position;
        hologramPrefab.SetActive(false);
        holoExplosion.Play();
        var colliders = Physics.OverlapSphere(hologramPrefab.transform.position, 5f, layerMask);
        AudioManager.instance.PlaySFXFromClip(hologramExplode);
        foreach (var col in colliders)
        {
            var health = col.GetComponent<TargetHealth>();
            if (health)
            {
                health.TakeDamage(damage, WeaponType.AoE);
            }
        }
    }

    public IEnumerator SpeedBoost()
    {
        characterController.Speed += speedBoost;
        yield return new WaitForSeconds(time);
        characterController.Speed -= speedBoost;
    }
}
