using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceWallMod : WeaponMod
{
    public ParticleSystem iceWallEffect;
    public GameObject iceWallCollider;
    public float icewallDuration;
    private bool active;
    public float hitRate;
    private float hitTimer;
    public float targetRate;
    private float targetTimer;
    public float hitRange;
    private List<TargetHealth> targets = new List<TargetHealth>();

    public override void Init()
    {
        base.Init();
        baseWeapon.weaponFuelManager.constantUse = true;
        baseWeapon.weaponOverride = true;
        Vector3 IceWallLocation = baseWeapon.transform.position + (baseWeapon.transform.forward * 1);
        transform.position = IceWallLocation;
        transform.rotation = baseWeapon.transform.rotation;
        runUpgradeManager.ApplyStatModifiers(runMod);
    }

    public override void Fire()
    {
        base.Fire();
        if (active)
        {
            return;
        }
        active = true;
        iceWallCollider.gameObject.SetActive(true);
        iceWallEffect.Play();
    }

    public override void Stop()
    {
        base.Stop();
        active = false;
        iceWallCollider.gameObject.SetActive(false);
        iceWallEffect.Stop();
    }

    private void Update()
    {
        if(!active)
        {
            return;
        }
        hitTimer += Time.deltaTime;
        targetTimer += Time.deltaTime;
        if(targetTimer >= targetRate)
        {
            targetTimer = 0;
            targets.Clear();
        }
        if(hitTimer >= hitRate)
        {
            hitTimer = 0;
            var hits = Physics.OverlapSphere(iceWallCollider.transform.position, hitRange);
            foreach (var hit in hits)
            {
                TargetHealth target = hit.GetComponent<TargetHealth>();
                if (target == null)
                {
                    continue;
                }
                if (targets.Contains(target))
                {
                    continue;
                }
                if(target == BattleMech.instance.targetHealth)
                {
                    targets.Add(target);
                    continue;
                }
                target.TakeDamage(damage, baseWeapon.weaponType);
                targets.Add(target);
            }
        }
    }

}
