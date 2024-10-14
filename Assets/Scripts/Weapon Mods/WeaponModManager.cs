using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModManager : MonoBehaviour
{
    public List<WeaponMod> mods;
    public MechWeapon weapon;
    public ManualWeaponController altWeapon;
    public WeaponMod currentMod;
    public List<WeaponMod> currentMods = new List<WeaponMod>();

    public void LoadCurrentWeaponMods(WeaponType type)
    {
        currentMods.Clear();
        for (int i = 0; i < mods.Count; i++)
        {
            var mod = mods[i];
            if (mod.modType == type)
            {
                currentMods.Add(mod);
            }
        }
    }

    public WeaponMod FindModByName(string name)
    {
        var mod = mods.Find(x => x.RunMod.modName == name);
        return mod;
    }

    public void EquipWeaponMod(WeaponMod mod)
    {
        if(mod == null)
        {
            print("No mod found");
            return;
        }
        if(currentMod != null)
        {
            currentMod.RemoveMods();
        }

        currentMod = mod;
        weapon.weaponMod = currentMod;
        currentMod.baseWeapon = weapon;
        currentMod.transform.SetParent(weapon.transform);
        currentMod.transform.localPosition = Vector3.zero;
        currentMod.transform.localRotation = Quaternion.identity;
        currentMod.Init();
        //altWeapon.SetAltWeaponInputs();
        GameUI.instance.CloseModUI();
    }
}
