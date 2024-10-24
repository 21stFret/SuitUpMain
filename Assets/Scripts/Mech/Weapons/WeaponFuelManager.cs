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

    private void Recharge()
    {
        if(!canRecharge)
        {
            return;
        }

        weaponFuelBonus = BattleMech.instance.statMultiplierManager.GetCurrentValue(StatType.FuelRate);
        if (weaponFuelBonus !=0)
        {
            float total = weaponFuelBonus;
            if(weaponFuel>= total)
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


        weaponFuel += Time.deltaTime * weaponRechargeRate;
    }

    private void BurstFuel()
    {
        if (weapon.isFiring)
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
                
        if (weapon.isFiring)
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
