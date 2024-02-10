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

    public override void Die()
    {
        base.Die();
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
        spitProjectiles[spitIndex].transform.position = transform.position;
        spitProjectiles[spitIndex].transform.rotation = transform.rotation;
        spitProjectiles[spitIndex].GetComponent<SpitProjectile>().Init(attackDamage);
    }
}
