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
    public float escapeDistance;
    public bool Runner;
    private bool runnerIdle;
    public Transform runnerTarget;

    public override void Die(WeaponType killedBy)
    {
        base.Die(killedBy);
        if(Runner)
        {
            BattleManager.instance.ObjectiveComplete();
        }
    }

    public override void TakeDamageOveride()
    {
        if(Runner)
        {
            GetNewRunnerPos();
        }
    }

    public override void Spawn(bool daddy = false)
    {
        base.Spawn();
        spitTimer = spitSpeed;

        if (Runner)
        {
            GetNewRunnerPos();

        }
    }

    private void GetNewRunnerPos()
    {
        if(!runnerIdle)
        {

            //return;
        }
        Vector3 randomPos = Random.insideUnitSphere * 30;
        randomPos.y = 1;
        runnerTarget.position = randomPos;
        runnerIdle = false;
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
