using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHealth : MonoBehaviour
{
    public float health;
    public float healthMax;
    public bool invincible;

    private Crawler _crawler;
    private MechHealth _mech;
    private Prop _prop;

    private void Start()
    {
        health = healthMax;
        _crawler = GetComponent<Crawler>();
        _mech = GetComponent<MechHealth>();
        _prop = GetComponent<Prop>();
        if(_mech != null)
        {
            _mech.UpdateHealth(health, true);
        }
    }

    public void TakeDamage(float damage, WeaponType weaponType, float stunTime = 0)
    {
        if (invincible)
        {
            damage = 0;
        }

        if (_crawler != null)
        {
            if(weaponType == WeaponType.Cralwer)
            {
                return;
            }
            _crawler.TakeDamage(damage, weaponType, stunTime);
        }

        if (_mech != null)
        {
            _mech.TakeDamage(damage);
        }

        if (_prop != null)
        {
            _prop.TakeDamage(damage);
        }
    }
}
