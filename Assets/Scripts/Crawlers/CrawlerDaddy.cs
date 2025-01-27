using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerDaddy : Crawler
{
    public GameObject DeathEffect;
    public int spawnCount;
    public float explosionRadius = 10f;
    public float explosionForce = 1000f;
    public LayerMask layerMask;
    public GameObject eggs;

    public override void Die(WeaponType killedBy)
    {
        if(killedBy != WeaponType.Default)
        {
            Vector3 pos = transform.position;
            pos.y += 3;
            spawnCount = Random.Range(2, 5);
            crawlerSpawner.SpawnAtPoint(transform, spawnCount);
            DeathEffect.transform.SetParent(null);
            DeathEffect.SetActive(true);
            ExplodeIfInRange();
        }
        base.Die(killedBy);
        eggs.SetActive(false);
    }

    private void ExplodeIfInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = collider.transform.position - transform.position;
                rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
                if(rb.GetComponent<TargetHealth>() != null)
                {
                    rb.GetComponent<TargetHealth>().TakeDamage(attackDamage, WeaponType.Cralwer);
                }
            }
        }
    }

    public override void Spawn(bool daddy = false)
    {
        base.Spawn();
        eggs.SetActive(true);
        DeathEffect.transform.SetParent(transform);
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
