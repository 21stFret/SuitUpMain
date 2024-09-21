using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHealth : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public bool invincible;
    public bool alive;

    private Crawler _crawler;
    private MechHealth _mech;
    private Prop _prop;

    public void Init()
    {
        health = maxHealth;
        alive = true;
        _crawler = GetComponent<Crawler>();
        _mech = GetComponent<MechHealth>();
        _prop = GetComponent<Prop>();
        if(_mech != null)
        {
            _mech.UpdateHealth(health, true);
        }
    }

    public void TakeDamage(float damage, WeaponType weaponType = WeaponType.Cralwer, float stunTime = 0)
    {
        if (invincible)
        {
            damage = 0;
        }

        if (_crawler != null)
        {
            damage = damage * MechStats.instance.damageMultiplier;
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
            _prop.TakeDamage(damage, weaponType);
        }
    }

    public void RepairMech(float amount)
    {
        _mech.UpdateHealth(amount, true);
    }
}
