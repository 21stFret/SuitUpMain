using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModManager : MonoBehaviour
{
    public List<WeaponMod> mods;
    public MechWeapon assualtWeapon;
    public MechWeapon techWeapon;
    public WeaponController weaponController;
    public WeaponMod currentTechMod;
    public WeaponMod currentAssaultMod;
    public List<WeaponMod> currentMods = new List<WeaponMod>();

    [InspectorButton("EquipAssaultModTEst")]
    public bool equipMod;

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

    public void EquipTechWeaponMod(WeaponMod mod)
    {
        if(mod == null)
        {
            print("No mod found");
            return;
        }
        if(currentAssaultMod != null)
        {
            currentAssaultMod.RemoveMods();
        }

        currentTechMod = mod;
        techWeapon.weaponMod = currentTechMod;
        currentTechMod.baseWeapon = techWeapon;
        currentTechMod.transform.SetParent(assualtWeapon.transform);
        currentTechMod.transform.localPosition = Vector3.zero;
        currentTechMod.transform.localRotation = Quaternion.identity;
        currentTechMod.enabled = true;
        currentTechMod.Init();
        //altWeapon.SetAltWeaponInputs();
        GameUI.instance.CloseModUI();
    }

    public void EquipAssaultMod(WeaponMod mod)
    {
        if(currentAssaultMod == null)
        {
            return;
        }
        currentAssaultMod = mod;
        assualtWeapon.weaponMod = currentAssaultMod;
        currentAssaultMod.baseWeapon = assualtWeapon;
        currentAssaultMod.enabled = true;
        currentAssaultMod.transform.SetParent(assualtWeapon.transform);
        currentAssaultMod.transform.localPosition = Vector3.zero;
        currentAssaultMod.transform.localRotation = Quaternion.identity;
        currentAssaultMod.Init();
    }

    public void EquipAssaultModTEst()
    {
        if (currentAssaultMod == null)
        {
            return;
        }
        
        assualtWeapon.weaponMod = currentAssaultMod;
        currentAssaultMod.baseWeapon = assualtWeapon;
        currentAssaultMod.enabled = true;
        currentAssaultMod.transform.SetParent(assualtWeapon.transform);
        currentAssaultMod.transform.localPosition = Vector3.zero;
        currentAssaultMod.transform.localRotation = Quaternion.identity;
        currentAssaultMod.Init();
    }
}
