using System.Collections;
using System.Collections.Generic;
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

    public override void Spawn()
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
        crawlerMovement.tracking = false;
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
        crawlerMovement.speedFinal = 0;
        spitTimer += Time.deltaTime;
        if (spitTimer > spitSpeed)
        {
            StartCoroutine(Spit());
            spitTimer = 0;
        }
    }

    public override void CheckDistance()
    {
        if (Runner)
        {
            float fillAmount = health / healthMax;
            GameUI.instance.objectiveUI.UpdateBar(fillAmount);
            if(crawlerMovement.distanceToTarget < escapeDistance)
            {
                animator.SetBool("InRange", false);
                animator.SetBool("Idle", false);
                crawlerMovement.tracking = false;
                crawlerMovement.speedFinal = _randomSpeed;
                crawlerMovement.SetDestination(transform.position + (transform.position - target.position).normalized * 10);
                return;
            }
            if(Vector3.Distance(transform.position, runnerTarget.position) < 5)
            {
                crawlerMovement.tracking = false;
                crawlerMovement.speedFinal = 0;
                animator.SetBool("Idle", true);
                runnerIdle = true;
                return;
            }
            if(runnerIdle)
            {
                crawlerMovement.SetTarget(target);
                base.CheckDistance();
            }
            else
            {
                crawlerMovement.speedFinal = _randomSpeed;
                animator.SetBool("Idle", false);
                crawlerMovement.SetDestination(runnerTarget.position);
            }

            return;
        }
        if(crawlerMovement.distanceToTarget < escapeDistance)
        {
            animator.SetBool("InRange", false);
            crawlerMovement.tracking = false;
            crawlerMovement.speedFinal = _randomSpeed;
            crawlerMovement.SetDestination(transform.position + (transform.position - target.position).normalized * 10);
            return;
        }
        base.CheckDistance();
        crawlerMovement.tracking = true;
    }
}
