using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class CrawlerSpitter : Crawler
{
    public GameObject DeathEffect;
    public LayerMask layerMask;
    public GameObject[] spitProjectiles = new GameObject[3];
    private int spitIndex;
    public float spitSpeed;
    private float spitTimer;
    public float escapeDistance;

    public override void Die(WeaponType killedBy)
    {
        base.Die(killedBy);
    }

    public override void Spawn()
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
        spitProjectiles[spitIndex].transform.position = transform.position + (transform.up * 3) + transform.forward;
        spitProjectiles[spitIndex].GetComponent<SpitProjectile>().Init(attackDamage, target);
    }

    public override void Attack()
    {
        crawlerMovement.speed = 0;
        spitTimer += Time.deltaTime;
        if (spitTimer > spitSpeed)
        {
            StartCoroutine(Spit());
            spitTimer = 0;
        }
    }

    public override void CheckDistance()
    {
        if(crawlerMovement.distanceToTarget < escapeDistance)
        {
            animator.SetBool("InRange", false);
            crawlerMovement.tracking = false;
            crawlerMovement.speed = speed;
            crawlerMovement.SetDestination(transform.position + (transform.position - target.position).normalized * 10);
            return;
        }
        base.CheckDistance();
        crawlerMovement.tracking = true;
    }
}
