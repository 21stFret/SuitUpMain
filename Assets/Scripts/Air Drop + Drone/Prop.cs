using DamageNumbersPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    private float health;
    private float healthMax;
    public TargetHealth _targetHealth;
    public DamageNumber damageNumberPrefab;
    public bool damageNumbersOn;

    private Collider _collider;

    [InspectorButton("RefreshProp")]
    public bool init;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {

        if (_targetHealth == null)
        {
            print("No target health found on " + gameObject.name);
            return;
        }
        _targetHealth.Init(null, null, this);
        healthMax = _targetHealth.maxHealth;
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
        
        _targetHealth.alive = false;
        _collider.enabled = false;
        print(gameObject.name + " has died");
    }

    public virtual void RefreshProp()
    {
        _targetHealth.health = _targetHealth.maxHealth;
        _targetHealth.alive = true;
        _collider.enabled = true;
        gameObject.SetActive(true);
    }
}
