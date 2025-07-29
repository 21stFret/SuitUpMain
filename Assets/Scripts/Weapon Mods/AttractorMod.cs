using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractorMod : WeaponMod
{
    private ShieldAlt shieldAlt;
    public float burstTimer;
    float burstTime;
    float timer;
    public bool firing;
    public bool cooldown;
    public float stunRadius;
    public float pullforce;
    public ParticleSystem attractorEffect;

    public override void Init()
    {
        base.Init();
        shieldAlt = (ShieldAlt)baseWeapon;
        float percent = Mathf.Abs(runMod.modifiers[0].statValue) / 100;
        float dam = BattleMech.instance.weaponController.mainWeaponEquiped.damage;
        damage = dam * percent;
    }

    // Fire Weapon
    public override void Fire()
    {
        if (!shieldAlt.isShieldActive)
        {
            return;
        }
        base.Fire();
        firing = true;
        attractorEffect.Play();

    }

    private void Update()
    {
        if (firing)
        {
            timer += Time.deltaTime;
            if (timer > burstTime)
            {
                var forceCenter = transform.position + transform.forward * 5f - transform.right * 2f;
                Collider[] hitColliders = Physics.OverlapSphere(forceCenter, stunRadius);
                foreach (var hitCollider in hitColliders)
                {
                    if(hitCollider.CompareTag("Player"))
                    {
                        continue; // Skip player layer
                    }
                    // Apply stun effect to enemies within the radius
                    TargetHealth enemy = hitCollider.GetComponent<TargetHealth>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage, WeaponType.Shield);
                    }
                    Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddForce((forceCenter - rb.transform.position).normalized * pullforce, ForceMode.Impulse);
                    }
                }
            }
        }
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        firing = false;
        timer = 0;
        attractorEffect.Stop();
    }

}
