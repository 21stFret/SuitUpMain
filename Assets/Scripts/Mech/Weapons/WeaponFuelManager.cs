using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFuelManager : MonoBehaviour
{
    public MechWeapon weapon;
    public bool _enabled;
    public WeaponUI weaponUI;
    public float weaponFuel;
    public float weaponFuelMax = 100;
    public float weaponFuelBonus = 0;
    public float weaponRechargeRate;
    public float weaponFuelRate;
    public bool canRecharge = true;
    public bool constantUse = false;
    public bool weaponInUse;

    public void Init(MechWeapon mechWeapon)
    {
        constantUse = true;
        weapon = mechWeapon;
        weaponFuel = weaponFuelMax;
        _enabled = true;
        weaponFuelRate = weapon.weaponFuelUseRate;
        weaponRechargeRate = 15;
    }

    public IEnumerator BoostRecharge(float time)
    {
        weaponRechargeRate = 30;
        yield return new WaitForSeconds(time);
        weaponRechargeRate = 15;
    }

    public void RefillFuel(float value)
    {
        weaponFuel += value;
        if (weaponFuelBonus != 0)
        {
            float total = weaponFuelBonus;
            if (weaponFuel >= total)
            {
                weaponFuel = total;
                return;
            }
        }
        else
        {
            if (weaponFuel >= weaponFuelMax)
            {
                weaponFuel = weaponFuelMax;
                return;
            }
        }
    }

    public bool isFull()
    {
        return weaponFuel >= weaponFuelMax;
    }

    void Update()
    {
        if(!_enabled)
        {
            return;
        }
        if (!constantUse)
        {
            BurstFuel();
            return;
        }
        FuelManagement();
    }

    public void SetBonus()
    {
        weaponFuelBonus = BattleMech.instance.statMultiplierManager.GetCurrentValue(StatType.Fuel_Tank);
    }

    private void Recharge()
    {
        if(!canRecharge)
        {
            return;
        }

        RefillFuel(Time.deltaTime * weaponRechargeRate);
    }

    private void BurstFuel()
    {
        if (weaponInUse)
        {
            if (weaponFuel <= 0)
            {
                weapon.Stop();
                return;
            }
        }
        else
        {
            Recharge();
        }
        weaponUI.UpdateWeaponUI(weaponFuel);
    }

    private void FuelManagement()
    {
        if(weapon == null)
        {
            return;
        }
                
        if (weaponInUse)
        {
            if (weaponFuel <= 0)
            {
                weapon.Stop();
                return;
            }
            weaponFuel -= Time.deltaTime * weaponFuelRate;
        }
        else
        {
            Recharge();
        }
        weaponUI.UpdateWeaponUI(weaponFuel);
    }
}
