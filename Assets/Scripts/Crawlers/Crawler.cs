using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Micosmo.SensorToolkit;
using Unity.VisualScripting;
using DG.Tweening;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using UnityEditor;
using DamageNumbersPro;

public enum CrawlerType
{
    Crawler,
    Daddy,
    DaddyCrawler,
    Albino,
    Spitter,
    Charger
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
    [HideInInspector]
    public CrawlerSpawner crawlerSpawner;


    [SerializeField]
    public float health;
    public int healthMax;
    public int attackDamage;
    public float speed;
    public float _randomSpeed;
    public float randomScale;
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

    public bool forceSpawn;

    private float pulseCheckTime = 0.5f;

    public DamageNumber damageNumberPrefab;
    public bool damageNumbersOn;
    private TargetHealth _targetHealth;

    public void Init()
    {
        hasTarget = false;
        dead = false;
        _targetHealth = GetComponent<TargetHealth>();
        _targetHealth.Init();
        _collider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        crawlerMovement = GetComponent<CrawlerMovement>();
        rangeSensor = GetComponent<RangeSensor>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        rb = GetComponent<Rigidbody>();
        _randomSpeed = Random.Range(speed, speed + randomScale);
        _collider.enabled = false;
        //crawlerMovement.groundCollider.enabled = false;
        crawlerMovement.enabled = false;
        meshRenderer.enabled = false;
        if (damageNumberPrefab == null)
        {
            damageNumbersOn = false;
            return;
        }
        damageNumbersOn = true;
    }

    private void Start()
    {
        hasTarget = false;
        dead = false;
        if (forceSpawn)
        {
            Init();
            Spawn();
        }
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

        pulseCheckTime -= Time.deltaTime;
        if(pulseCheckTime <= 0)
        {
            pulseCheckTime = 1f;
            FindClosestTarget();
        }

        if(!target)
        {
            return;
        }

        TargetHealth targetHealth = target.GetComponent<TargetHealth>();

        if(targetHealth != null)
        {
            if (!targetHealth.alive)
            {
                hasTarget = false;
                target = null;
                return;
            }
        }

        crawlerMovement.SetTarget(target);
        CheckDistance();
    }
    public void FindClosestTarget()
    {
        if (!rangeSensor.GetNearestDetection())
        {
            return;
        }
        target = rangeSensor.GetNearestDetection().transform;

        hasTarget = true;
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
            crawlerMovement.speedFinal = _randomSpeed;
        }
    }

    public virtual void Attack()
    {
        animator.SetTrigger("Attack");
        crawlerMovement.speedFinal = 0;
    }

    public void DoDamage()
    {
        //Called by animation event
        if (target == null)
        {
            hasTarget = false;
            return;
        }

        if (crawlerMovement.distanceToTarget >= crawlerMovement.stoppingDistance)
        {
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

    public virtual void TakeDamageOveride()
    {
        
    }

    public void DamageNumbers(float dam, WeaponType weapon)
    {
        DamageNumber newPopup = damageNumberPrefab.Spawn(transform.position, dam);
        newPopup.SetFollowedTarget(transform);
        newPopup.SetScale(5);
        switch (weapon)
        {
            case WeaponType.Minigun:
                newPopup.SetColor(Color.white);
                break;
            case WeaponType.Shotgun:
                break;
            case WeaponType.Flame:
                newPopup.SetColor(Color.red);
                break;
            case WeaponType.Lightning:
                newPopup.SetColor(Color.cyan);
                break;
            case WeaponType.Cryo:
                newPopup.SetColor(Color.blue);
                break;
            case WeaponType.Grenade:
                break;
            case WeaponType.Plasma:
                newPopup.SetColor(Color.magenta);
                break;
            case WeaponType.AoE:
                break;
            case WeaponType.Cralwer:
                break;
            case WeaponType.Default:
                break;
            default:
                break;
        }
    }

    
    public void TakeDamage(float damage, WeaponType killedBy, float stunTime = 0)
    {


        if (stunTime > 0)
        {
            StartCoroutine(StunCralwer(stunTime));
        }

        if(dead)
        {
            return;
        }

        FlashRed();
        TakeDamageOveride();
        if (damageNumbersOn)
        {
            DamageNumbers(damage, killedBy);
        }

        if (immune)
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
        health = 0;
        PlayDeathNoise();
        tag = "Untagged";
        rb.constraints = RigidbodyConstraints.FreezeAll;
        _collider.enabled = false;
        crawlerMovement.groundCollider.gameObject.layer = 2;
        crawlerMovement.enabled = false;
        meshRenderer.enabled = false;
        target = null;
        crawlerMovement.speedFinal = 0;
        //animator.SetTrigger("Die");
        DeathBlood.Play();

        if(crawlerSpawner != null)
        {
            crawlerSpawner.AddtoRespawnList(this, crawlerType);
        }

        

        if (weapon == WeaponType.Default)
        {
            return;
        }

        if (CashCollector.instance != null)
        {
            CashCollector.instance.AddCash(cashWorth);
            if (Random.Range(0, 100) < 15)
            {
                GameObject go = Instantiate(partPrefab, transform.position + (transform.up * 2), Quaternion.identity);
                go.transform.SetParent(CashCollector.instance.crawlerPartParent.transform);
            }
        }

        if(GameManager.instance != null)
        {
            GameManager.instance.UpdateKillCount(1, weapon);
            GameManager.instance.AddExp(expWorth);
        }


    }

    public void PlayDeathNoise()
    {
        deathNoise.pitch = Random.Range(0.8f, 1.2f);
        deathNoise.Play();
    }

    public virtual void Spawn()
    {
        gameObject.SetActive(true);
        dead = false;
        meshRenderer.enabled = true;
        _collider.enabled = true;
        crawlerMovement.groundCollider.enabled = true;
        crawlerMovement.groundCollider.gameObject.layer = 8;
        rb.velocity = Vector3.zero;
        health = healthMax;
        if (transform.position.y< crawlerMovement.groundLevel)
        {
            print("Crawler spawned below ground");
            transform.position = new Vector3(transform.position.x, crawlerMovement.groundLevel + 1, transform.position.z);
        }
        transform.localScale = Vector3.zero;
        animator.SetTrigger("Respawn");
        StartCoroutine(SpawnEffect());
        StartCoroutine(SpawnImmunity());
        rb.AddForce(transform.forward * Random.Range(5, 10), ForceMode.Impulse);
        pulseCheckTime = 0;
    }

    private IEnumerator SpawnEffect()
    {
        _spawnEffect.Play();
        transform.DOScale(Random.Range(crawlerScale-0.1f, crawlerScale+0.1f), 0.4f);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        yield return new WaitForSeconds(0.4f);
        tag = "Enemy";
        crawlerMovement.enabled = true;
        _randomSpeed = Random.Range(speed, speed + randomScale);
        crawlerMovement.speedFinal = _randomSpeed;
        animator.speed = 1;
    }
}

