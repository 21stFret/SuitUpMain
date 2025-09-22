using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainsawStaffMod : WeaponMod
{
    public ChainSaw chainSawPrefab;

    public override void Init()
    {
        base.Init();
        runUpgradeManager.ApplyMod(runMod);
        chainSawPrefab.gameObject.SetActive(true);
        chainSawPrefab.weaponData = baseWeapon.weaponData;
        chainSawPrefab.damage = baseWeapon.damage;
    }

    public override void RemoveMods()
    {
        base.RemoveMods();
        chainSawPrefab.gameObject.SetActive(false);
    }

    public override void Fire()
    {
        base.Fire();
        if (chainSawPrefab != null)
        {
            chainSawPrefab.Fire();
        }
    }

    public override void Stop()
    {
        base.Stop();
        if (chainSawPrefab != null)
        {
            chainSawPrefab.Stop();
        }
    }

}
