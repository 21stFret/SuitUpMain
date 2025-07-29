using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainsawExtenderMod : WeaponMod
{
    public ChainSaw chainSawPrefab;

    public override void Init()
    {
        base.Init();
        runUpgradeManager.ApplyMod(runMod);
        chainSawPrefab =(ChainSaw) baseWeapon;
        chainSawPrefab.transform.localScale *= 1.8f; // Increase the size of the chainsaw
        chainSawPrefab.weaponData = baseWeapon.weaponData;
    }

    public override void RemoveMods()
    {
        base.RemoveMods();
        if (chainSawPrefab == null)
        {
            return;
        }
        chainSawPrefab.transform.localScale /= 1.8f; // Reset the size of the chainsaw
    }
}
