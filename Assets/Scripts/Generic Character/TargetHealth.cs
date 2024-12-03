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
        maxHealth = BattleMech.instance.statMultiplierManager.GetCurrentValue(StatType.Health);
        if(maxHealth<=0)
        {
            maxHealth = 1;
        }
        if(health > maxHealth)
        {
            health = maxHealth;
        }
        _mech.UpdateHealthUI(health);
    }


    public void TakeDamage(float damage, WeaponType weaponType = WeaponType.Cralwer, float stunTime = 0)
    {
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
            _mech.TakeDamage(damage);
            return;
        }

        float newDam = ApplyDamageMultiplier(damage, weaponType);

        //print("Base Damage: " + baseDamage);
        if(newDam > 0)
        {
            damage = newDam;
            //print("New Damage: " + damage);
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
            _crawler.TakeDamage(damage, weaponType, stunTime);
        }
    }

    public void ApplySlow()
    {
        if (_crawler != null)
        {
            _crawler.crawlerMovement.ApplySlow();
        }
        if (_mech != null)
        {
            BattleMech.instance.myCharacterController.ApplySlow();
        }
    }

    public float ApplyDamageMultiplier(float damage, WeaponType weaponType)
    {
        if(damage <= 0)
        {
            return 0;
        }

        float newDamage = damage;
        int multiplierType = AscertainMultiplier(weaponType);

        if (multiplierType == 1)
        {
            newDamage = BattleMech.instance.statMultiplierManager.GetCurrentValue(StatType.MWD_Increase_Percent);
        }
        else if (multiplierType == 2)
        {
            newDamage = BattleMech.instance.statMultiplierManager.GetCurrentValue(StatType.AWD_Increase_Percent);
        }

        return newDamage;
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
