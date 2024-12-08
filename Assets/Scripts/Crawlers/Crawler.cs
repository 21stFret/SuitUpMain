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
    Albino,
    Spitter,
    Charger,
    Leaper,
    Hunter,
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
    public int attackDamage;
    public float attackRange;
    public float speed;
    private float _finalSpeed;
    public float randomScale;
    public ParticleSystem DeathBlood;
    public ParticleSystem _spawnEffect;
    public Animator animator;
    public bool dead;
    public AudioSource deathNoise;
    protected bool inRange;
    [SerializeField]
    private bool immune;
    public float crawlerScale;
    public int cashWorth;
    public int expWorth;
    public int dropRate;


    public CrawlerType crawlerType;

    public GameObject partPrefab;

    public bool forceSpawn;

    public DamageNumber damageNumberPrefab;
    public bool damageNumbersOn;
    public TargetHealth _targetHealth;

    public bool triggeredAttack;

    public CrawlerBehavior _crawlerBehavior;
    public Vector3 spawnLocation;

    public virtual void Init()
    {
        dead = false;
        _targetHealth = GetComponent<TargetHealth>();
        _targetHealth.Init(this);
        _collider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        crawlerMovement = GetComponent<CrawlerMovement>();
        crawlerMovement.m_crawler = this;
        rangeSensor = GetComponent<RangeSensor>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        rb = GetComponent<Rigidbody>();
        SetSpeed();
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
        Invoke("EnableBrain", 0.2f);
    }

    private void EnableBrain()
    {
        _crawlerBehavior = GetComponent<CrawlerBehavior>();
        _crawlerBehavior.Init();
    }

    private void Start()
    {
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

    private void SetSpeed()
    {
        float scaleFActor = speed * randomScale;
        float min = speed - scaleFActor;
        float max = speed + scaleFActor;
        _finalSpeed = Random.Range(min, max);
    }

    void Update()
    {
        if(dead)
        {
            return;
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
                target = null;
                return;
            }
        }
    }

    public void FindClosestTarget(bool trueFind = false)
    {
        float range = trueFind? 100 : attackRange;
        rangeSensor.SetSphereShape(range);
        target = null;
        rangeSensor.Pulse();
        var _targets = rangeSensor.GetNearestDetection();
        if (!_targets)
        {
            return;
        }
        target = _targets.transform;
        crawlerMovement.SetTarget(target);
    }

    public virtual void Attack()
    {
        if (triggeredAttack)
        {
            return;
        }
        triggeredAttack = true;
        animator.SetBool("InRange", true);
        animator.SetTrigger("Attack");
        crawlerMovement.canMove = false;
    }

    public void DoDamage()
    {
        if (!triggeredAttack)
        {
            return;
        }

        //Called by animation event
        if (target == null)
        {
            return;
        }

        if (crawlerMovement.distanceToTarget >= attackRange)
        {
            animator.SetBool("InRange", false);
            print("Player out of range");
            return;
        }

        target.GetComponent<TargetHealth>().TakeDamage(attackDamage, WeaponType.Cralwer);
    }

    public void EndAttack()
    {
        triggeredAttack = false;
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
        if(weapon == WeaponType.Default)
        {
            return;
        }
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

    
    public virtual void TakeDamage(float damage, WeaponType killedBy, float stunTime = 0)
    {
        if (stunTime > 0)
        {
            StartCoroutine(StunCralwer(stunTime));
        }

        if(dead)
        {
            return;
        }

        if(target == null)
        {
            FindClosestTarget(true);
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

        _targetHealth.health -= damage;

        _crawlerBehavior.OnDamageTaken();

        if (_targetHealth.health <= 0 )
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
        _targetHealth.health = 0;
        PlayDeathNoise();
        tag = "Untagged";
        rb.constraints = RigidbodyConstraints.FreezeAll;
        _collider.enabled = false;
        crawlerMovement.enabled = false;
        meshRenderer.enabled = false;
        target = null;
        DeathBlood.transform.SetParent(null);
        DeathBlood.Play();
        _crawlerBehavior.OnDeath();

        if(crawlerSpawner != null)
        {
            crawlerSpawner.AddtoRespawnList(this, crawlerType);
        }

        var BM = BattleManager.instance;

        if (BM.Battles[BM.currentBattleIndex].battleType == BattleType.Exterminate)
        {
            BM.StartCoroutine(BM.CheckActiveEnemies());
        }

        if (weapon == WeaponType.Default)
        {
            return;
        }

        if (CashCollector.instance != null)
        {
            CashCollector.instance.AddCash(cashWorth);
            if (Random.Range(0, 100) < dropRate)
            {
                GameObject go = Instantiate(partPrefab, transform.position + (transform.up * 2), Quaternion.identity);
                go.transform.SetParent(CashCollector.instance.crawlerPartParent.transform);
            }
        }

        if(PlayerProgressManager.instance != null)
        {
            PlayerProgressManager.instance.UpdateKillCount(1, weapon);
            PlayerProgressManager.instance.AddExp(expWorth);
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
        rb.velocity = Vector3.zero;
        _targetHealth.health = _targetHealth.maxHealth;
        SetSpeed();
        crawlerMovement.speedFinal = _finalSpeed;
        transform.localScale = Vector3.zero;
        animator.SetTrigger("Respawn");
        StartCoroutine(SpawnEffect());
        StartCoroutine(SpawnImmunity());
        rb.AddForce(transform.forward * Random.Range(5, 10), ForceMode.Impulse);
        DeathBlood.transform.SetParent(transform);
    }

    private IEnumerator SpawnEffect()
    {
        _spawnEffect.Play();
        SetCrawlerScale();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        yield return new WaitForSeconds(0.4f);
        tag = "Enemy";
        crawlerMovement.enabled = true;
        crawlerMovement.canMove = true;
        animator.speed = 1;
        spawnLocation = transform.position;
    }

    private void SetCrawlerScale()
    {
        float scaleFActor = crawlerScale * randomScale;
        float min = crawlerScale - scaleFActor;
        float max = crawlerScale + scaleFActor;
        transform.DOScale(Random.Range(min, max), 0.4f);
    }
}

