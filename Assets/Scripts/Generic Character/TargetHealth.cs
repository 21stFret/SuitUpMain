using DamageNumbersPro;
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

    public List<WeaponType> immuneWeapons;

    public bool damageNumbersOn;
    public DamageNumber damageNumberPrefab;

    public void Init(Crawler C = null, MechHealth M =null, Prop P=null)
    {
        health = maxHealth;
        alive = true;
        _crawler = C;
        _mech = M;
        _prop = P;
        if(_mech != null)
        {
            _mech.Init();
            _mech.UpdateHealthUI(health);
        }
    }

    public void SetNewMaxHealth()
    {
        float curtentHealthPercent = health / maxHealth;
        maxHealth = BattleMech.instance.statMultiplierManager.GetCurrentValue(StatType.Health);
        if(maxHealth<=0)
        {
            maxHealth = 1;
        }
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        health = maxHealth * curtentHealthPercent;
        _mech.UpdateHealthUI(health);
    }

    public bool isFull()
    {
        return health >= maxHealth;
    }

    public void SetDamageNumbers(bool value)
    {
        damageNumbersOn = value;
    }


    public void TakeDamage(float damage, WeaponType weaponType = WeaponType.Cralwer, float stunTime = 0)
    {
        if (!alive)
        {
            return;
        }

        if (invincible)
        {
            damage = 0;
        }

        foreach(WeaponType immuneWeapon in immuneWeapons)
        {
            if(immuneWeapon == weaponType)
            {
                damage = 0;
            }
        }

        if(weaponType == WeaponType.Cryo)
        {
            ApplySlow();
        }

        if (_mech != null)
        {
            if (damageNumbersOn)
            {
                DamageNumbers(damage, weaponType);
            }

            _mech.TakeDamage(damage);
            return;
        }

        float multiplier = ApplyDamageMultiplier(damage, weaponType);
        if(multiplier != 0)
        {
            damage *= multiplier;
        }

        if (damageNumbersOn)
        {
            DamageNumbers(damage, weaponType);
        }

        if (_prop != null)
        {
            _prop.TakeDamage(damage, weaponType);
            return;
        }

        if (_crawler != null)
        {
            if (weaponType == WeaponType.Cralwer)
            {
                return;
            }
            _crawler.TakeDamage(damage, weaponType, stunTime, invincible);
        }
    }

    public void DamageNumbers(float dam, WeaponType weapon)
    {
        if (weapon == WeaponType.Default)
        {
            return;
        }

        if(dam<0)
        {
            weapon = WeaponType.Heal;
        }
        

        DamageNumber newPopup = damageNumberPrefab.Spawn(transform.position, Mathf.Abs(dam));
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
                newPopup.SetColor(Color.red);
                break;
            case WeaponType.Heal:
                newPopup.SetColor(Color.green);
                break;
            default:
                break;
        }
    }

    public void ApplySlow()
    {
        float amount = 0.5f;
        //TODO add slow multipier to increase slowed amount in bonus mods
        if (_crawler != null)
        {
            _crawler.crawlerMovement.ApplySlow(amount);
        }
        if (_mech != null)
        {
            BattleMech.instance.myCharacterController.ApplyIce(amount);
        }
    }

    public float ApplyDamageMultiplier(float damage, WeaponType weaponType)
    {
        if(damage <= 0)
        {
            return 0;
        }

        float damageMultiplier = 0;
        int multiplierType = AscertainMultiplier(weaponType);

        if (multiplierType == 1)
        {
            damageMultiplier = BattleMech.instance.statMultiplierManager.GetCurrentMultiplier(StatType.Assault_Damage);
        }
        else if (multiplierType == 2)
        {
            damageMultiplier = BattleMech.instance.statMultiplierManager.GetCurrentMultiplier(StatType.Tech_Damage);
        }

        return damageMultiplier;
    }

    private int AscertainMultiplier(WeaponType weaponType)
    {
        switch(weaponType)
            {
            case WeaponType.Minigun:
                return 1;
            case WeaponType.Shotgun:
                return 1;
            case WeaponType.Plasma:
                return 1;
            case WeaponType.Cryo:
                return 2;
            case WeaponType.Lightning:
                return 2;
            case WeaponType.Flame:
                return 2;
            case WeaponType.Cralwer:
                return 0;
            default:
                return 0;
        }
    }
}
