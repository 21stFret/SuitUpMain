using DamageNumbersPro;
using Micosmo.SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public TargetHealth _targetHealth;

    public Collider _collider;

    [InspectorButton("RefreshProp")]
    public bool init;

    protected WeaponType killedBy;

    private bool _isInitialized = false;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _isInitialized = false;
    }

    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        if (_isInitialized)
            return;
        _isInitialized = true;
        if (_targetHealth == null)
        {
            _targetHealth = GetComponent<TargetHealth>();
            if (_targetHealth == null)
            {
                //print("No target health found on " + gameObject.name);
                return;
            }
        }
        if(_collider == null)
        {
            _collider = GetComponent<Collider>();
            if (_collider == null)
            {
                //print("No collider found on " + gameObject.name);
                return;
            }
        }
        _targetHealth.Init(null, null, this);
        _collider.enabled = true;
    }

    public void TakeDamage(WeaponType weapon)
    {
        if (_targetHealth.health <= 0)
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
        Init();
        if (_collider != null)
        {
            _collider.enabled = true;
        }
        if (_targetHealth != null)
        {
            _targetHealth.Init();
        }
        gameObject.SetActive(true);
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
