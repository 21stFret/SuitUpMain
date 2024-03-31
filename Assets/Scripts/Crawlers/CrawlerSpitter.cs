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

    public override void Die(WeaponType killedBy)
    {
        base.Die(killedBy);
    }

    public override void Spawn()
    {
        base.Spawn();
    }

    private void CycleProjectiles()
    {
        spitIndex++;
        if (spitIndex >= spitProjectiles.Length)
        {
            spitIndex = 0;
        }
    }

    public void Spit()
    {
        animator.SetTrigger("Spit");
        crawlerMovement.speed = 0;
        CycleProjectiles();
        spitProjectiles[spitIndex].transform.position = transform.position + transform.up * 3;
        spitProjectiles[spitIndex].GetComponent<SpitProjectile>().Init(attackDamage, target);
    }

    public override void Attack()
    {
        spitTimer += Time.deltaTime;
        if (spitTimer >= spitSpeed)
        {
            Spit();
            spitTimer = 0;
        }
    }
}
