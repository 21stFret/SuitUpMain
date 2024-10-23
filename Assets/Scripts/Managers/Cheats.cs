using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    public TargetHealth targetHealth;
    public GameObject UI;
    public WeaponController altWeaponController;
    private float savedFuelUseRate;

    public void ToggleInvincible(bool toggle)
    {
        targetHealth.invincible = toggle;
    }

    public void ToggleUI(bool toggle)
    {
        UI.SetActive(!toggle);
    }

    public void ToggleAltWeaponAmmo(bool toggle)
    {
        if(savedFuelUseRate == 0)
        {
            savedFuelUseRate = altWeaponController.altWeaponEquiped.weaponFuelUseRate;
        }

        if(toggle)
        {
            altWeaponController.altWeaponEquiped.weaponFuelUseRate = 0;
        }
        else
        {
            altWeaponController.altWeaponEquiped.weaponFuelUseRate = savedFuelUseRate;
        }

    }
}
