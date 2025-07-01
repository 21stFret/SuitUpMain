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
    public bool equipAssMod;

    [InspectorButton("EquipTechModTEst")]
    public bool equipTEchMod;

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

    public void RemoveCurrentMods()
    {
        currentMods.Clear();
        currentTechMod = null;
        currentAssaultMod = null;
    }

    public void RemoveAssutaltMod()
    {
        if (currentAssaultMod != null)
        {
            currentAssaultMod.RemoveMods();
            currentAssaultMod = null;
            assualtWeapon.weaponMod = null;
        }
    }

    public void RemoveTechMod()
    {
        if (currentTechMod != null)
        {
            currentTechMod.RemoveMods();
            currentTechMod = null;
            techWeapon.weaponMod = null;
        }
    }

    public WeaponMod FindModByName(string name)
    {
        for (int i = 0; i < mods.Count; i++)
        {
            if (mods[i].runMod.modName == name)
            {
                mods[i].runMod = GameManager.instance.runUpgradeManager.GetWeaponModByName(name);
                return mods[i];
            }
        }
        return null;
    }

    public void EquipTechWeaponMod(WeaponMod mod)
    {
        if(mod == null)
        {
            print("No mod found");
            return;
        }
        if(currentTechMod != null)
        {
            currentTechMod.RemoveMods();
        }

        currentTechMod = mod;
        techWeapon.weaponMod = currentTechMod;
        currentTechMod.baseWeapon = techWeapon;
        currentTechMod.transform.SetParent(techWeapon.gunturret.transform);
        currentTechMod.transform.localPosition = Vector3.zero;
        currentTechMod.transform.localRotation = Quaternion.identity;
        currentTechMod.enabled = true;
        currentTechMod.Init();
    }

    public void EquipAssaultMod(WeaponMod mod)
    {
        if(currentAssaultMod != null)
        {
            currentAssaultMod.RemoveMods();
        }
        currentAssaultMod = mod;
        assualtWeapon.weaponMod = currentAssaultMod;
        currentAssaultMod.baseWeapon = assualtWeapon;
        currentAssaultMod.enabled = true;
        currentAssaultMod.transform.SetParent(assualtWeapon.gunturret.transform);
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

        RunMod runMod = GameManager.instance.runUpgradeManager.GetWeaponModByName(currentAssaultMod.runMod.modName);

        GameManager.instance.runUpgradeManager.EnableModSelection(runMod);
    }

    public void EquipTechModTEst()
    {
        if (currentTechMod == null)
        {
            return;
        }
        RunMod runMod = GameManager.instance.runUpgradeManager.GetWeaponModByName(currentAssaultMod.runMod.modName);

        GameManager.instance.runUpgradeManager.EnableModSelection(runMod);
    }
}
