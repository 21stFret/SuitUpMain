using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Micosmo.SensorToolkit;
using Unity.VisualScripting;
using DG.Tweening;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public enum CrawlerType
{
    Crawler,
    Daddy,
    Albino,
    Spitter
}

public class Crawler : MonoBehaviour
{
    private RangeSensor rangeSensor;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public CrawlerMovement crawlerMovement;
    [HideInInspector]
    public SkinnedMeshRenderer meshRenderer;
    [HideInInspector]
    public Transform target;
    private Collider _collider;


    [SerializeField]
    private float health;
    public int healthMax;
    public int attackDamage;
    public float speed;
    public ParticleSystem DeathBlood;
    public ParticleSystem _spawnEffect;
    protected Animator animator;
    private bool hasTarget;
    private bool dead;
    public AudioSource deathNoise;
    private bool canSeeTarget;
    protected bool inRange;
    [SerializeField]
    private bool immune;
    public float randomLocationRadius;
    public float crawlerScale;
    public int cashWorth;
    public int expWorth;


    public CrawlerType crawlerType;

    public GameObject partPrefab;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        _collider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        crawlerMovement = GetComponent<CrawlerMovement>();
        rangeSensor = GetComponent<RangeSensor>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        hasTarget = false;
        dead = false;
    }

    public IEnumerator StunCralwer(float stunTime)
    {
        crawlerMovement.enabled = false;
        animator.speed = 0;
        yield return new WaitForSeconds(stunTime);

        if (dead)
        {
            yield break;
        }

        crawlerMovement.enabled = true;
        animator.speed = 1;
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

        //SetTargetDestination();
        crawlerMovement.SetTarget(target);
        CheckDistance();
    }
    public void FindClosestTarget()
    {
        if (hasTarget)
        { return; }

        if (!rangeSensor.GetNearestDetection())
        {
            return;
        }
        target = rangeSensor.GetNearestDetection().transform;

        hasTarget = true;
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
            Vector3 randomPosition = Random.insideUnitSphere * randomLocationRadius;
            randomPosition += target.position;
            crawlerMovement.SetDestination(randomPosition);
        }
    }

    public virtual void CheckDistance()
    {
        if (crawlerMovement.distanceToTarget < crawlerMovement.stoppingDistance)
        {
            inRange = true;
            animator.SetBool("InRange", true);
            Attack();
        }
        else
        {
            inRange = false;
            animator.SetBool("InRange", false);
            crawlerMovement.speed = speed;
        }
    }

    public virtual void Attack()
    {
        animator.SetTrigger("Attack");
        crawlerMovement.speed = 0;
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

        targethealth.TakeDamage(attackDamage, WeaponType.Cralwer);
    }

    private IEnumerator SpawnImmunity()
    {
        immune = true;
        yield return new WaitForSeconds(2);
        immune = false;
    }

    
    public void TakeDamage(float damage, WeaponType killedBy, float stunTime = 0)
    {
        FlashRed();

        if (stunTime > 0)
        {
            StartCoroutine(StunCralwer(stunTime));
        }

        if (immune)
        {
            return;
        }

        if(dead)
        {
            return;
        }

        health -= damage;

        if (health <= 0 )
        {
            Die(killedBy);
        }

    }
    
    public void DealyedDamage(float damage, float delay, WeaponType weapon)
    {
        StartCoroutine(StunCralwer(delay));
        StartCoroutine(DealyedDamageCoroutine(damage, delay, weapon));
    }

    private IEnumerator DealyedDamageCoroutine(float damage, float delay, WeaponType weapon)
    {
        yield return new WaitForSeconds(delay);
        TakeDamage(damage, weapon);
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

    public virtual void Die(WeaponType weapon)
    {
        dead = true;
        PlayDeathNoise();
        tag = "Untagged";
        GetComponent<Collider>().enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        _collider.enabled = false;
        crawlerMovement.groundCollider.enabled= false;
        crawlerMovement.enabled = false;
        meshRenderer.enabled = false;
        target = null;
        crawlerMovement.speed = 0;
        animator.SetTrigger("Die");
        DeathBlood.Play();
        CrawlerSpawner.instance.AddtoRespawnList(this, crawlerType);

        if (CashCollector.Instance != null)
        {
            CashCollector.Instance.AddCash(cashWorth);
        }

        if(GameManager.instance != null)
        {
            GameManager.instance.UpdateKillCount(1, weapon);
            GameManager.instance.AddExp(expWorth);
        }

        if (Random.Range(0, 100) < 15)
        {
            GameObject go = Instantiate(partPrefab, transform.position +(transform.up *2), Quaternion.identity);
            go.transform.SetParent(CashCollector.Instance.crawlerPartParent.transform);
        }
    }

    public void PlayDeathNoise()
    {
        deathNoise.pitch = Random.Range(0.8f, 1.2f);
        deathNoise.Play();
    }

    public virtual void Spawn()
    {
        rb.velocity = Vector3.zero;
        health = healthMax;
        if (transform.position.y< crawlerMovement.groundLevel)
        {
            print("Crawler spawned below ground");
            transform.position = new Vector3(transform.position.x, crawlerMovement.groundLevel + 1, transform.position.z);
        }
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        animator.SetTrigger("Respawn");
        StartCoroutine(SpawnEffect());
        StartCoroutine(SpawnImmunity());
        rb.AddForce(transform.forward * Random.Range(5, 10), ForceMode.Impulse);
    }

    private IEnumerator SpawnEffect()
    {
        _spawnEffect.Play();
        transform.DOScale(Random.Range(crawlerScale-0.1f, crawlerScale+0.1f), 0.4f);
        meshRenderer.enabled = true;
        _collider.enabled = true;
        crawlerMovement.groundCollider.enabled = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        yield return new WaitForSeconds(0.4f);
        tag = "Enemy";
        crawlerMovement.enabled = true;
        crawlerMovement.speed = speed;
        dead = false;
    }
}

