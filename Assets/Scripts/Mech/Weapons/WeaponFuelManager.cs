using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFuelManager : MonoBehaviour
{
    public MechWeapon weapon;
    private bool _enabled;
    public bool uiEnabled;
    public WeaponUI weaponUI;
    public Sprite fuelSprite;
    public float weaponFuel;
    public float weaponFuelMax = 100;
    public float weaponRechargeRate;
    public float weaponFuelUseRate;

    public void Init(MechWeapon mechWeapon)
    {
        weapon = mechWeapon;
        weaponFuel = weaponFuelMax;
        _enabled = true;
        weaponFuelUseRate = weapon.weaponFuelUseRate;
        weaponRechargeRate = weapon.weaponRechargeRate;
        weaponUI.SetFuelImage(weapon.baseWeaponInfo.weaponSprite);
        uiEnabled = true;
    }



    void Update()
    {
        if(!_enabled)
        {
            return;
        }
        FuelManagement();
    }

    private void FuelManagement()
    {
        if (weapon.isFiring)
        {
            if (weaponFuel <= 0)
            {
                weapon.Stop();
                return;
            }
            weaponFuel -= Time.deltaTime * weaponFuelUseRate;
        }
        else
        {
            if (weaponFuel >= weaponFuelMax)
            { return; }

            weaponFuel += Time.deltaTime * weaponRechargeRate;
        }

        if (weaponUI == null || !uiEnabled)
        {
            return;
        }

        weaponUI.UpdateWeaponUI(weaponFuel);

    }
}
