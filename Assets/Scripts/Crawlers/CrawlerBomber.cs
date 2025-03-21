using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerBomber : Crawler
{
    public GameObject[] bombsacks;

    public float explosionRadius = 10f;
    public float explosionForce = 1000f;
    public LayerMask layerMask;

    public float distanceToGrow;

    public override void Die(WeaponType killedBy)
    {
        overrideDeathNoise = true;
        //deathNoise.clip = deathSound;
        ExplodeIfInRange();
        base.Die(killedBy);
        //eggs.SetActive(false);
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

    void Update()
    {
        if(crawlerMovement!=null)
        {
            if(crawlerMovement.distanceToTarget <= distanceToGrow)
            {
                foreach (var item in bombsacks)
                {
                    item.transform.localScale = Vector3.Lerp(item.transform.localScale, Vector3.one*3, 1 - crawlerMovement.distanceToTarget / distanceToGrow);
                }
            }
            else
            {
                foreach (var item in bombsacks)
                {
                    item.transform.localScale = Vector3.Lerp(item.transform.localScale, Vector3.one, Time.deltaTime * 2);
                }
            }
        }
    }

}
