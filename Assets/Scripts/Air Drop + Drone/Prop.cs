using DamageNumbersPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public float health;
    public float healthMax;
    private bool isDead;
    private TargetHealth _targetHealth;

    public DamageNumber damageNumberPrefab;
    public bool damageNumbersOn;

    public virtual void Init()
    {
        _targetHealth = GetComponent<TargetHealth>();
        if (_targetHealth != null)
        {
            _targetHealth.Init();
        }
        healthMax = _targetHealth.healthMax;
        health = healthMax;
    }

    private void DamageNumbers(float dam, WeaponType weapon)
    {
        DamageNumber newPopup = damageNumberPrefab.Spawn(transform.position, dam);
        newPopup.SetFollowedTarget(transform);
        newPopup.SetScale(5);
        switch (weapon)
        {
            case WeaponType.Minigun:
                newPopup.SetColor(Color.white);
                break;
            case WeaponType.Shotgun:
                break;
            case WeaponType.Flame:
                newPopup.SetColor(Color.red);
                break;
            case WeaponType.Lightning:
                newPopup.SetColor(Color.cyan);
                break;
            case WeaponType.Cryo:
                newPopup.SetColor(Color.blue);
                break;
            case WeaponType.Grenade:
                break;
            case WeaponType.Plasma:
                newPopup.SetColor(Color.magenta);
                break;
            case WeaponType.AoE:
                break;
            case WeaponType.Cralwer:
                break;
            case WeaponType.Default:
                break;
            default:
                break;
        }
    }

    public void TakeDamage(float damage, WeaponType weapon)
    {
        // Destroy the prop
        if (isDead)
        {
            return;
        }
        health -= damage;

        if (damageNumbersOn)
        {
            DamageNumbers(damage, weapon);
        }

        if (health  <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        isDead = true;
        print(gameObject.name + " has died");
    }
}
