using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrawlerSpitter : Crawler
{
    public LayerMask layerMask;
    public GameObject[] spitProjectiles = new GameObject[3];
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
        if (spitIndex >= spitProjectiles.Length)
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
            spitProjectiles[spitIndex].GetComponent<SpitProjectile>().Init(attackDamage, target);
        }
    }

    public override void Attack()
    {
        spitTimer += Time.deltaTime;
        if (spitTimer > spitSpeed)
        {
            StartCoroutine(Spit());
            spitTimer = 0;
        }
    }
}
