using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NapalmMod : WeaponMod
{
    public MyPooler pooler;
    public float _damage;
    public float dropRate;
    private float dropRateT;

    public override void Fire()
    {
        base.Fire();
        float percentage = runMod.modifiers[0].statValue / 100;
        _damage = baseWeapon.damage * percentage;
    }

    public override void Stop()
    {
        base.Stop();
        dropRateT = 0;
    }

    private void Update()
    {
        if(baseWeapon == null)
        {
            return;
        }
        if(!baseWeapon.isFiring)
        {
            return;
        }
        if (dropRateT < dropRate)
        {
            dropRateT += Time.deltaTime;
        }
        else
        {
            dropRateT = 0;
            RaycastHit hit;
            Vector3 ground = new Vector3();
            float range = baseWeapon.range * Random.Range(0.4f, 0.8f);
            if (Physics.Raycast(transform.position, transform.forward, out hit, baseWeapon.range))
            {
                ground = hit.point;
            }
            else
            {
                ground = transform.position + transform.forward * range;
            }
            GameObject burningPatch = pooler.GetBurningPatch();
            ground.y = 0.1f;
            burningPatch.transform.position = ground;
            burningPatch.SetActive(true);
            BurningPatch bp = burningPatch.GetComponent<BurningPatch>();
            bp.damageArea.damageAmount = _damage;
            bp.EnableDamageArea();
        }
    }
}
