using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Micosmo.SensorToolkit;
using Unity.VisualScripting;
using DG.Tweening;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class Crawler : MonoBehaviour
{
    private RangeSensor rangeSensor;
    public Transform target;
    public float health;
    public int healthMax;
    public int attackDamage;
    public float speed;
    public CrawlerMovement crawlerMovement;
    public ParticleSystem DeathBlood;
    public ParticleSystem _spawnEffect;
    public Animator animator;
    public bool hasTarget;
    private bool dead;
    public AudioSource deathNoise;
    public bool canSeeTarget;
    public SkinnedMeshRenderer meshRenderer;
    public bool stunned;
    private Collider _collider;
    public int IgnorelayerMask;
    public int ShootlayerMask;
    public bool immune;
    public float randomRadius;
    public float crawlerScale;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        crawlerMovement = GetComponent<CrawlerMovement>();
        rangeSensor = GetComponent<RangeSensor>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        hasTarget = false;
        health = healthMax;
    }

    public IEnumerator SwitchOffNavMesh(float stunTime)
    {
        stunned = true;
        crawlerMovement.enabled = false;
        yield return new WaitForSeconds(stunTime);

        if (dead)
        {
            stunned = false;
            yield break;
        }

        crawlerMovement.enabled = true;
        stunned = false;
    }

    void Update()
    {
        if(dead)
        {
            return;
        }

        if (target == null)
        {
            hasTarget = false;
            FindClosestTarget();
            return; 
        }

        if (target.GetComponent<TargetHealth>().health <= 0)
        {
            hasTarget = false;
            target = null;
            return;
        }

        if(stunned)
        {
            crawlerMovement.speed = 0;
            return;
        }
        else
        {
            crawlerMovement.speed = speed;
        }

        SetTargetDestination();
        Attack();
    }

    private void SetTargetDestination()
    {
        if (Physics.Raycast(transform.position, target.position - transform.position, out RaycastHit hit, 100f))
        {
            if (hit.transform == target)
            {
                canSeeTarget = true;
            }
            else
            {
                canSeeTarget = false;
            }
        }
        else
        {
            canSeeTarget = false;
        }

        if (canSeeTarget)
        {
            crawlerMovement.SetTarget(target);
        }
        else
        {
            Vector3 randomPosition = Random.insideUnitSphere * randomRadius;
            randomPosition += target.position;
            NavMeshHit hitSample;
            if (NavMesh.SamplePosition(randomPosition, out hitSample, randomRadius, NavMesh.AllAreas))
            {
                crawlerMovement.SetDestination(hitSample.position);
            }
        }
    }

    public virtual void Attack()
    {
        if (crawlerMovement.distanceToTarget < crawlerMovement.stoppingDistance)
        {
            animator.SetBool("Attack", true);
            crawlerMovement.speed = 0;
        }
        else
        {
            animator.SetBool("Attack", false);
            crawlerMovement.speed = speed;
        }
    }

    public void DoDamage()
    {
        //Called by animation event
        if (target == null)
        {
            hasTarget = false;
            return;
        }

        var targethealth = target.GetComponent<TargetHealth>();

        if(targethealth.health<=0)
        {
            hasTarget = false;
            target = null;
            return;
        }

        targethealth.TakeDamage(attackDamage, this);
    }

    private IEnumerator SpawnImmunity()
    {
        immune = true;
        yield return new WaitForSeconds(2);
        immune = false;
    }

    public void TakeDamage(float damage, float stunTime = 0)
    {
        if(immune)
        {
            return;
        }

        if(dead)
        {
            return;
        }
        health -= damage;
        FlashRed();
        if (health <= 0 )
        {
            Die();
        }

        else if (stunTime > 0)
        {
            StartCoroutine(SwitchOffNavMesh(stunTime));
        }

    }

    public void DealyedDamage(float damage, float delay)
    {
        StartCoroutine(DealyedDamageCoroutine(damage, delay));
    }

    private IEnumerator DealyedDamageCoroutine(float damage, float delay)
    {
        yield return new WaitForSeconds(delay);
        TakeDamage(damage);
    }

    private void FlashRed()
    {
        StartCoroutine(FlashRedCoroutine());
    }

    private IEnumerator FlashRedCoroutine()
    {
        meshRenderer.material.SetFloat("_FlashOn", 1);
        yield return new WaitForSeconds(0.1f);
        meshRenderer.material.SetFloat("_FlashOn", 0);
    }


    public void FindClosestTarget()
    {
        if(hasTarget)
        { return; }

        if(!rangeSensor.GetNearestDetection())
        {
            return;
        }
        target = rangeSensor.GetNearestDetection().transform;

        hasTarget = true;
    }

    public virtual void Die()
    {
        dead = true;
        PlayDeathNoise();
        tag = "Untagged";
        gameObject.layer = IgnorelayerMask;
        crawlerMovement.enabled = false;
        meshRenderer.enabled = false;
        target = null;
        crawlerMovement.speed = 0;
        animator.SetTrigger("Die");
        DeathBlood.Play();
        StartCoroutine(delayedColision());
        CashCollector.Instance.AddCash(10);
        GameManager.instance.UpdateKillCount(1);
        ObjectSpawner.instance.AddtoRespawnList(this);
    }

    public void PlayDeathNoise()
    {
        deathNoise.pitch = Random.Range(0.8f, 1.2f);
        deathNoise.Play();
    }

    public virtual void Spawn()
    {
        if(dead)
        { 
            animator.SetTrigger("Respawn"); 
        }
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        StartCoroutine(SpawnEffect());
        StartCoroutine(SpawnImmunity());
    }

    private IEnumerator SpawnEffect()
    {
        _spawnEffect.Play();
        transform.DOScale(Random.Range(crawlerScale-0.1f, crawlerScale+0.1f), 0.4f);
        yield return new WaitForSeconds(0.4f);
        tag = "Enemy";
        meshRenderer.enabled = true;
        gameObject.layer = ShootlayerMask;
        _collider.enabled = true;
        crawlerMovement.enabled = true;
        crawlerMovement.speed = speed;
        dead = false;
    }

    private IEnumerator delayedColision()
    {
        yield return new WaitForSeconds(2f);
        _collider.enabled = false;
    }
}

