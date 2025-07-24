using DamageNumbersPro;
using Micosmo.SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    private float health;
    private float healthMax;
    public TargetHealth _targetHealth;

    public Collider _collider;

    [InspectorButton("RefreshProp")]
    public bool init;

    protected WeaponType killedBy;

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
            _targetHealth = GetComponent<TargetHealth>();
            if (_targetHealth == null)
            {
                //print("No target health found on " + gameObject.name);
                return;
            }
        }
        _targetHealth.Init(null, null, this);
        healthMax = _targetHealth.maxHealth;
        health = healthMax;
        _collider.enabled = true;
    }

    public void TakeDamage(float damage, WeaponType weapon)
    {
        health -= damage;

        if (health  <= 0)
        {
            killedBy = weapon;
            Die();        
        }
    }

    public virtual void Die()
    {
        if (_targetHealth != null)
        {
            _targetHealth.alive = false;
        }

        _collider.enabled = false;
        //print(gameObject.name + " has died");
    }

    public virtual void RefreshProp()
    {
        _collider.enabled = true;
        gameObject.SetActive(true);
        Init();
    }

    // Add this to the object with the collider
    void OnDisable()
    {
        // Force trigger exit when disabled
        var sensors = FindObjectsOfType<TriggerSensor>();
        foreach (var sensor in sensors)
        {
            sensor.TriggerExit(GetComponent<Collider>());
        }
    }
}
