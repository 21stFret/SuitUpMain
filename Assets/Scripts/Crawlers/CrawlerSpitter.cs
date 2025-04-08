using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrawlerSpitter : Crawler
{
    public LayerMask layerMask;
    public List<GameObject> spitProjectiles = new List<GameObject>();
    public GameObject spitPrefab, eliteSpitPrefab;
    private int spitIndex;
    public float spitSpeed;
    private float spitTimer;
    public Transform spitLocation;

    public override void Die(WeaponType killedBy)
    {
        base.Die(killedBy);
    }

    public override void TakeDamageOveride()
    {
    }

    public override void Spawn(bool daddy = false)
    {
        base.Spawn();
        spitTimer = spitSpeed;
    }

    private void CycleProjectiles()
    {
        spitIndex++;
        if (spitIndex >= spitProjectiles.Count-1)
        {
            spitIndex = 0;
        }
    }

    public IEnumerator Spit()
    {
        animator.SetTrigger("Spit");
        yield return new WaitForSeconds(0.3f);
        CycleProjectiles();
        spitProjectiles[spitIndex].transform.SetParent(null);
        spitProjectiles[spitIndex].transform.position = spitLocation.position;

        if(target != null)
        {
            var projectile = spitProjectiles[spitIndex].GetComponent<SpitProjectile>();
            if(projectile.inflight)
            {
                CycleProjectiles();
            }
            else
            {
                projectile.Init(attackDamage, target);
            }
        }
    }

    public override void Attack()
    {
        spitTimer += Time.deltaTime;
        if (spitTimer > spitSpeed)
        {
            StartCoroutine(Spit());
            spitTimer = Random.Range(-0.5f, 0.5f);
        }
    }

    public override void MakeElite(bool _becomeElite)
    {
        base.MakeElite(_becomeElite);
        spitProjectiles.Clear();
        if(isElite)
        {
            for (int i = 0; i < 5; i++)
            {
                spitProjectiles.Add(Instantiate(eliteSpitPrefab));
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                spitProjectiles.Add(Instantiate(spitPrefab));
            }
        }
    }
}
