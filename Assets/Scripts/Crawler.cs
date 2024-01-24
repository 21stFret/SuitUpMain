using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Micosmo.SensorToolkit;
using Unity.VisualScripting;

public class Crawler : MonoBehaviour
{
    private RangeSensor rangeSensor;
    public Transform target;
    public int health;
    public int healthMax;
    public int damage;
    public NavMeshAgent navMeshAgent;
    public ParticleSystem DeathBlood;
    public Animator animator;
    public bool hasTarget;
    private bool dead;
    public AudioSource deathNoise;
    public float attackDistance;
    public float DistanceToTarget;
    public float speed;
    public float randomRadius;
    public bool canSeeTarget;
    public SkinnedMeshRenderer meshRenderer;
    public bool stunned;
    private Collider _collider;
    public int IgnorelayerMask;
    public int ShootlayerMask;
    public bool immune;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
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
        navMeshAgent.enabled = false;
        yield return new WaitForSeconds(stunTime);
        navMeshAgent.enabled = true;
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
            navMeshAgent.speed = 0;
            return;
        }
        else
        {
            navMeshAgent.speed = speed;
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
            navMeshAgent.SetDestination(target.position);
        }
        else if (navMeshAgent.destination == transform.position)
        {
            Vector3 randomPosition = Random.insideUnitSphere * randomRadius;
            randomPosition += target.position;
            NavMeshHit hitSample;
            if (NavMesh.SamplePosition(randomPosition, out hitSample, randomRadius, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hitSample.position);
            }
        }
        var blah = navMeshAgent.steeringTarget;
        transform.LookAt(new Vector3(blah.x, transform.position.y, blah.z));
    }

    private void Attack()
    {
        DistanceToTarget = Vector3.Distance(transform.position, target.position);
        if (DistanceToTarget < attackDistance)
        {
            animator.SetBool("Attack", true);
            navMeshAgent.speed = 0;
        }
        else
        {
            animator.SetBool("Attack", false);
            navMeshAgent.speed = speed;
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

        targethealth.TakeDamage(damage, this);
    }

    private IEnumerator SpawnImmunity()
    {
        immune = true;
        yield return new WaitForSeconds(2);
        immune = false;
    }

    public void TakeDamage(int damage, float stunTime = 0)
    {
        if(immune)
        {
            return;
        }
        if(stunTime >0)
        {
            StartCoroutine(SwitchOffNavMesh(stunTime));
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
        navMeshAgent.enabled = false;
        target = null;
        navMeshAgent.speed = 0;
        animator.SetTrigger("Die");
        DeathBlood.Play();
        StartCoroutine(delayedColision());
        CashCollector.Instance.AddCash(10);
        GameManager.instance.UpdateKillCount(1);
    }

    public void PlayDeathNoise()
    {
        deathNoise.pitch = Random.Range(0.8f, 1.2f);
        deathNoise.Play();
    }

    public virtual void Respawn()
    {
        if(dead)
        { 
            animator.SetTrigger("Respawn"); 
        }
        tag = "Enemy";
        gameObject.layer = ShootlayerMask;
        gameObject.SetActive(true);
        _collider.enabled = true;
        navMeshAgent.enabled = true;
        navMeshAgent.speed = speed;
        dead = false;
        StartCoroutine(SpawnImmunity());
    }

    private IEnumerator delayedColision()
    {
        yield return new WaitForSeconds(2f);
        _collider.enabled = false;
        ObjectSpawner.instance.AddtoRespawnList(this);
    }
}

