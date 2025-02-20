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
    public float attackDamage;
    public float attackRange;
    public float seekRange;
    public float speed;
    private float _finalSpeed;
    public float randomScale;
    public ParticleSystem DeathBlood;
    public ParticleSystem _spawnEffect;
    public Animator animator;
    [HideInInspector]
    public bool dead;
    public AudioSource deathNoise;
    public bool overrideDeathNoise;
    protected bool inRange;
    [SerializeField]
    private bool immune;
    public float immuneTime = 1;
    public float crawlerScale;
    public int cashWorth;
    public int expWorth;
    public int dropRate;
    public float ScreamChance;
    public bool isStunned;
    public bool canBeStunned = true;

    public TriggerSensor.ObjectStateHandler objectStateHandler;


    public CrawlerType crawlerType;

    public GameObject partPrefab;

    public bool forceSpawn;

    public TargetHealth _targetHealth;
    [HideInInspector]
    public bool triggeredAttack;

    protected CrawlerBehavior _crawlerBehavior;
    [HideInInspector]
    public Vector3 spawnLocation;
    public float SpawnForce;

    public bool dummy;

    [InspectorButton("TestSpawn")]
    public bool spawn;

    public AudioClip[] deathSounds;

    [Header("Elite Settings")]
    public bool isElite = false;
    public bool becomeElite = false;
    public Material eliteMaterial;
    public float eliteDamageMultiplier = 2f;
    public float eliteHealthMultiplier = 2f;
    public float eliteSpeedMultiplier = 1.5f;
    public float eliteSteerMultiplier = 1.25f;
    protected Material originalMaterial;
    public float eliteChance = 5;

    private void TestSpawn()
    {
        Spawn();
    }

    public virtual void Init()
    {
        dead = false;
        overrideDeathNoise = false;
        _targetHealth = GetComponent<TargetHealth>();
        _targetHealth.Init(this);
        _collider = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        crawlerMovement = GetComponent<CrawlerMovement>();
        crawlerMovement.m_crawler = this;
        rangeSensor = GetComponent<RangeSensor>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.material;
        }
        SetSpeed();
        rb = GetComponent<Rigidbody>();
        _collider.enabled = false;
        crawlerMovement.enabled = false;
        meshRenderer.enabled = false;
    }

    public virtual void MakeElite(bool _becomeElite)
    {
        if(isElite)
        {
            if (!_becomeElite)
            {
                meshRenderer.material = originalMaterial;
                attackDamage /= eliteDamageMultiplier;
                _targetHealth.maxHealth /= eliteHealthMultiplier;
                _targetHealth.health = _targetHealth.maxHealth;
                if (crawlerMovement != null)
                {
                    crawlerMovement.speedFinal /= eliteSpeedMultiplier;
                    crawlerMovement.steerSpeed /= eliteSteerMultiplier;
                }
                crawlerScale /= 1.2f;
                isElite = false;
            }
            return;
        }
        if(becomeElite)
        {
            // Apply material
            if (meshRenderer != null && eliteMaterial != null)
            {
                meshRenderer.material = eliteMaterial;
            }
            
            // Modify stats
            attackDamage *= eliteDamageMultiplier;
            _targetHealth.maxHealth *= eliteHealthMultiplier;
            _targetHealth.health = _targetHealth.maxHealth;
            
            // Modify movement
            if (crawlerMovement != null)
            {
                crawlerMovement.speedFinal *= eliteSpeedMultiplier;
                crawlerMovement.steerSpeed *= eliteSteerMultiplier;
            }
            
            crawlerScale *= 1.2f; // Make them slightly bigger
            isElite = true;
        }

    }

    private void EnableBrain()
    {
        _crawlerBehavior = GetComponent<CrawlerBehavior>();
        _crawlerBehavior.enabled = true;
        _crawlerBehavior.Init();
    }

    private void Start()
    {
        if (forceSpawn && _targetHealth==null)
        {
            crawlerSpawner = FindObjectOfType<CrawlerSpawner>();
            Init();
            Spawn();
        }
    }

    public IEnumerator StunCralwer(float stunTime)
    {
        if(!canBeStunned)
        {
            yield break;
        }
        if(isStunned)
        {
            yield break;
        }
        isStunned = true;
        crawlerMovement.enabled = false;
        animator.speed = 0;
        yield return new WaitForSeconds(stunTime);

        if (dead)
        {
            yield break;
        }

        crawlerMovement.enabled = true;
        animator.speed = 1;
        // Stun immunity for 1 second
        yield return new WaitForSeconds(1);
        isStunned = false;
    }

    private void SetSpeed()
    {
        float scaleFActor = speed * randomScale;
        float min = speed - scaleFActor;
        float max = speed + scaleFActor;
        _finalSpeed = Random.Range(min, max);
    }

    private void Update()
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

    public void FindClosestTarget(float trueFind = 0)
    {
        float range = trueFind>0? trueFind : seekRange;
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

    public void SetTarget(Transform _target)
    {
        target = _target;
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
        Vector3 attackloc = transform.position + (transform.forward * transform.localScale.x);

        print("Crawler position is " + transform.position + ". Attack hit at " + attackloc + " for target location " + target.transform.position);

        if (Vector3.Distance(attackloc, target.transform.position) >= attackRange)
        {
            animator.SetBool("InRange", false);
            //print("Target out of range");
            return;
        }

         target.GetComponent<TargetHealth>().TakeDamage(attackDamage, WeaponType.Cralwer, 0, this);
    }

    public void EndAttack()
    {
        triggeredAttack = false;
    }

    private IEnumerator SpawnImmunity()
    {
        _targetHealth.invincible = true;
        meshRenderer.material.SetColor("_Flash_Color", Color.green);
        meshRenderer.material.SetFloat("_FlashOn", 1);
        yield return new WaitForSeconds(immuneTime);
        meshRenderer.material.SetFloat("_FlashOn", 0);
        _targetHealth.invincible = false;
    }

    public virtual void TakeDamageOveride()
    {
        
    }

    
    public virtual void TakeDamage(float damage, WeaponType killedBy, float stunTime = 0,bool invincible = false)
    {
        if(dead)
        {
            return;
        }

        if (stunTime > 0)
        {
            StartCoroutine(StunCralwer(stunTime));
        }

        if (target == null)
        {
            FindClosestTarget(100);
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
        if(_targetHealth.invincible)
        {
            return;
        }
        StartCoroutine(FlashRedCoroutine());
    }

    private IEnumerator FlashRedCoroutine()
    {
        meshRenderer.material.SetColor("_Flash_Color", Color.red);
        meshRenderer.material.SetFloat("_FlashOn", 1);
        yield return new WaitForSeconds(0.1f);
        meshRenderer.material.SetFloat("_FlashOn", 0);
    }

    public virtual void Die(WeaponType weapon)
    {
        dead = true;
        _targetHealth.health = 0;
        _targetHealth.alive = false;
        PlayDeathNoise(overrideDeathNoise);
        tag = "Untagged";
        rb.constraints = RigidbodyConstraints.FreezeAll;
        _collider.enabled = false;
        crawlerMovement.enabled = false;
        meshRenderer.enabled = false;
        target = null;
        DeathBlood.transform.SetParent(null);
        DeathBlood.Play();
        _crawlerBehavior.OnDeath();

        CheckForModsOnDeath(weapon);

        if(crawlerSpawner != null)
        {
            crawlerSpawner.AddtoRespawnList(this, crawlerType);
        }

        gameObject.SetActive(false);

        var BM = BattleManager.instance;

        if (BM == null)
        {
            return;
        }

        if (BM._usingBattleType == BattleType.Exterminate)
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

    private void CheckForModsOnDeath(WeaponType weapon)
    {
        if(GameManager.instance?.runUpgradeManager == null)
        {
            return;
        }
        RunMod __selectMod = GameManager.instance.runUpgradeManager.HasModByName("Nano Bots");
        if (__selectMod != null)
        {
            BattleMech.instance.mechHealth.Heal(BattleMech.instance.targetHealth.maxHealth * (__selectMod.modifiers[0].statValue/100));
        }
        switch (weapon)
        {
            case WeaponType.Plasma:
                RunMod selectMod = GameManager.instance.runUpgradeManager.HasModByName("Energy Blast");
                if (selectMod != null)
                {
                    var burningPatch = MyPooler.Instance.GetBurningPatch();
                    burningPatch.transform.position = transform.position;
                    burningPatch.SetActive(true);
                    var burningPatchScript = burningPatch.GetComponent<BurningPatch>();
                    burningPatchScript.damageArea.damageAmount = 0.5f;
                    burningPatchScript.burnDuration = selectMod.modifiers[0].statValue;
                    burningPatchScript.EnableDamageArea();

                }
            break;
            case WeaponType.Cryo:
                RunMod _selectMod = GameManager.instance.runUpgradeManager.HasModByName("Fracture");
                if (_selectMod != null)
                {
                    var fractureEffect = MyPooler.Instance.GetFractureEffect();
                    fractureEffect.transform.position = transform.position;
                    fractureEffect.SetActive(true);
                    var particleSystem = fractureEffect.GetComponent<ParticleSystem>();
                    particleSystem.Play();
                    var colliders = Physics.OverlapSphere(transform.position, 5f);
                    float damage = (_selectMod.modifiers[0].statValue /100) * BattleMech.instance.weaponController.altWeaponEquiped.damage;
                    foreach (var col in colliders)
                    {
                        var health = col.GetComponent<TargetHealth>();
                        var rb = col.GetComponent<Rigidbody>();
                        if (health)
                        {
                            health.TakeDamage(damage, WeaponType.Cryo);
                        }
                        if (rb)
                        {
                            rb.AddForce((col.transform.position - transform.position).normalized * 50, ForceMode.Impulse);
                        }
                    }
                }
                break;

        }        
    }

    public void PlayDeathNoise(bool overrideDeathNoise)
    {
        if(!overrideDeathNoise)
        {
            deathNoise.clip = AudioManager.instance.GetRandomDeathNoise();
        }
        deathNoise.transform.SetParent(null);
        deathNoise.gameObject.SetActive(true);
        deathNoise.transform.position = transform.position;
        deathNoise.pitch = Random.Range(0.8f, 1.2f);
        deathNoise.Play();
    }

    public virtual void Spawn(bool daddy = false)
    {
        meshRenderer.material.SetFloat("_FlashOn", 0);
        dead = false;
        EnableBrain();
        gameObject.SetActive(true);
        meshRenderer.enabled = true;
        _collider.enabled = true;
        rb.velocity = Vector3.zero;
        _targetHealth.health = _targetHealth.maxHealth;
        _targetHealth.alive = true;
        SetSpeed();
        crawlerMovement.speedFinal = _finalSpeed;
        transform.localScale = Vector3.zero;
        StartCoroutine(SpawnEffect(daddy));
        StartCoroutine(SpawnImmunity());
        DeathBlood.transform.SetParent(transform);
        tag = "Enemy";      
    }

    private IEnumerator SpawnEffect(bool daddy)
    {
        MakeElite(becomeElite);
        _spawnEffect.Play();
        SetCrawlerScale();
        RandomSpawnScream();
        yield return new WaitForSeconds(0.1f);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        float force =SpawnForce;
        if(daddy)
        {
            force = SpawnForce / 2;
        }
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
        yield return new WaitForSeconds(0.2f);
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

    public void RandomSpawnScream()
    {
        if(Random.Range(0, 100) > ScreamChance)
        {
            return;
        }
        if (!overrideDeathNoise)
        {
            deathNoise.clip = AudioManager.instance.GetRandomSpawnNoise();
        }
        else
        {
            deathNoise.clip = deathSounds[Random.Range(0, deathSounds.Length)]; 
        }
        deathNoise.transform.SetParent(null);
        deathNoise.gameObject.SetActive(true);
        deathNoise.transform.position = transform.position;
        deathNoise.pitch = Random.Range(0.8f, 1.2f);
        deathNoise.Play();
    }
}

