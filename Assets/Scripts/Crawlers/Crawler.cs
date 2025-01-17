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
    //[HideInInspector]
    public Transform target;
    private Collider _collider;
    [HideInInspector]
    public CrawlerSpawner crawlerSpawner;


    [SerializeField]
    public int attackDamage;
    public float attackRange;
    public float seekRange;
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

    public TargetHealth _targetHealth;

    public bool triggeredAttack;

    public CrawlerBehavior _crawlerBehavior;
    public Vector3 spawnLocation;


    public bool dummy;

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
        crawlerMovement.enabled = false;
        meshRenderer.enabled = false;
    }

    private void EnableBrain()
    {
        _crawlerBehavior = GetComponent<CrawlerBehavior>();
        _crawlerBehavior.Init();
    }

    private void Start()
    {
        if (forceSpawn && _targetHealth==null)
        {
            crawlerSpawner = FindObjectOfType<CrawlerSpawner>();
            Init();
            meshRenderer.enabled = false;
            Invoke("Spawn", 0.1f);
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
        float range = trueFind? 100 : seekRange;
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
        rb.AddForce(transform.forward * 50, ForceMode.Impulse);
        Vector3 attackloc = transform.position + (transform.forward * (transform.localScale.x*2));

        print("Crawler position is " + transform.position + ". Attack hit at " + attackloc + " for target location " + target.transform.position);

        if (Vector3.Distance(attackloc, target.transform.position) >= attackRange)
        {
            animator.SetBool("InRange", false);
            print("Target out of range");
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

    
    public virtual void TakeDamage(float damage, WeaponType killedBy, float stunTime = 0,bool invincible = false)
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

        if (immune)
        {
            return;
        }

        if(!dummy)
        {
            _targetHealth.health -= damage;
        }   

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
        _targetHealth.TakeDamage(damage, weapon);
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

        if(weapon == WeaponType.Plasma)
        {
            RunMod selectMod = GameManager.instance.runUpgradeManager.HasModByName("Energy Blast");
            if (selectMod != null)
            {
                var burningPatch = BurningPatchPooler.Instance.GetBurningPatch();
                burningPatch.SetActive(true);
                burningPatch.transform.position = transform.position;
                var burningPatchScript = burningPatch.GetComponent<BurningPatch>();
                burningPatchScript.damageArea.damageAmount = 0.5f;
                burningPatchScript.burnDuration = selectMod.modifiers[0].statValue;
                burningPatchScript.EnableDamageArea();

            }
        }

        if(crawlerSpawner != null)
        {
            crawlerSpawner.AddtoRespawnList(this, crawlerType);
        }

        var BM = BattleManager.instance;

        if (BM == null)
        {
            return;
        }

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

        BattleMech.instance.droneController.ChargeDroneOnHit(expWorth);

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
        meshRenderer.material.SetFloat("_FlashOn", 0);
        dead = false;
        EnableBrain();
        gameObject.SetActive(true);
        meshRenderer.enabled = true;
        _collider.enabled = true;
        rb.velocity = Vector3.zero;
        _targetHealth.health = _targetHealth.maxHealth;
        SetSpeed();
        crawlerMovement.speedFinal = _finalSpeed;
        transform.localScale = Vector3.zero;
        StartCoroutine(SpawnEffect());
        StartCoroutine(SpawnImmunity());
        DeathBlood.transform.SetParent(transform);
    }

    private IEnumerator SpawnEffect()
    {
        _spawnEffect.Play();
        SetCrawlerScale();
        yield return new WaitForSeconds(0.1f);
        rb.AddForce(transform.forward * 10, ForceMode.Impulse);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        yield return new WaitForSeconds(0.2f);
        tag = "Enemy";
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

