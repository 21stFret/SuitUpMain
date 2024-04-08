using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : Prop
{
    public float explosionForce;
    public float explosionRadius;
    public float upwardsModifier;
    public float damage;
    public ParticleSystem explosionEffect;
    public LayerMask layerMask;
    public GameObject prefab;

    public override void Die()
    {
        Explode();
    }
    private void Explode()
    {
        GetComponent<Collider>().enabled = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
        foreach (Collider collider in colliders)
        {
            TargetHealth targetHealth = collider.GetComponent<TargetHealth>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage, WeaponType.AoE);
            }
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
        }
        explosionEffect.Play();
        prefab.SetActive(false);

    }
}
