using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class CrawlerDaddy : Crawler
{
    public GameObject DeathEffect;
    public int spawnCount;
    public float explosionRadius = 10f;
    public float explosionForce = 1000f;
    public LayerMask layerMask;

    public override void Die()
    {
        base.Die();
        DeathEffect.SetActive(true);
        ObjectSpawner.instance.SpawnAtPoint(transform.position + RandomUtils.RandomInsideSphere(2), spawnCount);
        ExplodeIfInRange();
    }

    private void ExplodeIfInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                print("Explode");
                Vector3 direction = collider.transform.position - transform.position;
                rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
                if(rb.GetComponent<TargetHealth>().mechHealth != null)
                {
                    rb.GetComponent<TargetHealth>().TakeDamage(attackDamage, this);
                }
            }
        }
    }

    public override void Spawn()
    {
        base.Spawn();
        meshRenderer.enabled = true;
        DeathEffect.SetActive(false);
    }
}

public static class RandomUtils
{
    public static Vector3 RandomInsideSphere(float radius)
    {
        Vector3 randomPoint = UnityEngine.Random.insideUnitSphere * radius;
        return randomPoint;
    }
}
