using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWallMod : WeaponMod
{
    public ParticleSystem iceWallEffect;
    public GameObject iceWallCollider;
    public float icewallDuration;
    private bool active;

    public override void Init()
    {
        base.Init();
        modType = WeaponType.Grenade;
    }

    public override void Fire()
    {
        base.Fire();
        if (active)
        {
            return;
        }
        if (baseWeapon.weaponFuelManager.weaponFuel >= modFuelCost)
        {
            baseWeapon.weaponFuelManager.weaponFuel -= modFuelCost;
            StartCoroutine(ActivateIceWall());
        }
    }

    private IEnumerator ActivateIceWall()
    {
        active = true;
        RaycastHit hit;
        Vector3 IceWallLocation = baseWeapon.transform.position + (baseWeapon.transform.forward * 3);
        if (Physics.Raycast(IceWallLocation, Vector3.down, out hit, 100))
        {
            transform.position = hit.point;
        }
        transform.rotation = baseWeapon.transform.rotation;
        iceWallCollider.gameObject.SetActive(true);
        iceWallEffect.Play();

        yield return new WaitForSeconds(icewallDuration);
        iceWallCollider.gameObject.SetActive(false);
        active = false;
    }
}
