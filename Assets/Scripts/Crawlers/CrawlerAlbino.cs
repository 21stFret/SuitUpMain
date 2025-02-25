using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using FORGE3D;

public class CrawlerAlbino : Crawler
{
    public ParticleSystem smashEffect;
    public float smashRadius;
    public float smashDistance;
    public float smashForce;
    public Transform smashLocation;
    public LayerMask smashLayerMask;
    private float smashTimer;
    public float smashCooldown;
    public AudioClip smashSound;
    private bool smashed;
    public bool charged;
    public CrawlerBurstSpawner burstSpawner;

    [Header("Charge Settings")]
    private bool chargeEnabled;
    public float chargeCooldown = 5f;
    private float chargeTimer;
    public float chargeDuration = 2f;
    public float chargeSpeed = 30f;
    public float chargeRadius;
    public float buildChargeTime;
    public float buildChargeTimer;
    public ParticleSystem chargeEffect;
    public AudioClip chargeSound;

    [ColorUsage(true, true)]
    public Color chargeColor;
    [ColorUsage(true, true)]
    public Color originalColor;

    public Cinemachine.CinemachineImpulseSource impulseSource;
    
    public override void Init()
    {
        base.Init();
        chargeEnabled = false;
        overrideDeathNoise = true;
    }

    public void Update()
    {
        if(target == null)
        {
            return;
        }
        if (smashTimer > 0)
        {
            smashTimer -= Time.deltaTime;
        }
        ChargeUpRushAttack();
    }

    private void ChargeUpRushAttack()
    {
        if(!chargeEnabled)
        {
            return;
        }

        if (chargeTimer > 0)
        {
            chargeTimer -= Time.deltaTime;
        }
        // Check for charge opportunity
        if (chargeTimer <= 0 && 
            Vector3.Distance(transform.position, target.position) < smashDistance * 1.5f && 
            !smashed)
        {
            charged = true;
            chargeTimer = chargeCooldown;
            _crawlerBehavior.TransitionToState(typeof(AlbinoChargeState));
            DOTween.Sequence()
                .Append(DOTween.To(() => meshRenderer.material.GetColor("_Emmission"),
                    x => meshRenderer.material.SetColor("_Emmission", x),
                    chargeColor,
                    buildChargeTime-0.5f)
                    .SetEase(Ease.InOutQuad))
                .AppendCallback(() => {
                    if (chargeEffect != null)
                    {
                        chargeEffect.Play();
                        RandomSpawnScream();
                    }
                });
            return;
        }
    }

    public void Smash()
    {
        smashEffect.transform.parent = null;
        smashEffect.transform.localScale = Vector3.one;
        smashEffect.transform.position = smashLocation.position;
        smashEffect.Play();
         float dam = 0;
        Collider[] colliders = Physics.OverlapSphere(smashLocation.position, smashRadius, smashLayerMask);
        foreach (Collider collider in colliders)
        {
            TargetHealth targetHealth = collider.GetComponent<TargetHealth>();
            if (targetHealth == null)
            {
                continue;
            }
            float dist = Vector3.Distance(smashLocation.position, collider.transform.position);
            float ratio1 = Mathf.Clamp01(1 - dist / smashRadius);
            dam = attackDamage * 2 * ratio1;
            dam = Mathf.Clamp(dam, 1, attackDamage * 2);
            targetHealth.TakeDamage(dam, WeaponType.Cralwer);
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = collider.transform.position - smashLocation.position;
                rb.AddForce(direction.normalized * smashForce, ForceMode.Impulse);
            }
        }
        triggeredAttack = false;
        float damagePercent = Mathf.Clamp(dam / 10f, 0.5f,1f);
        impulseSource.GenerateImpulse(damagePercent);
        deathNoise.clip = smashSound;
        deathNoise.Play();
        smashed = false;
    }

    public override void TakeDamageOveride()
    {
        base.TakeDamageOveride();
        if (_targetHealth.health < _targetHealth.maxHealth / 2)
        {
            chargeEnabled = true;
        }
    }

    private void TriggerSmashAnimation()
    {
        smashed = true;
        animator.SetTrigger("Smash Attack");
    }

    public override void Attack()
    {
        if(triggeredAttack)
        {
            return;
        }
        triggeredAttack = true;
        if(charged)
        {
            return;
        }
        if(smashTimer <= 0)
        {
            smashTimer = smashCooldown;
            TriggerSmashAnimation();
            return;
        }
        animator.SetBool("InRange", true);
        animator.SetTrigger("Attack");
    }

    public override void Die(WeaponType weapon)
    {
        base.Die(weapon);
        PlayerSavedData.instance._gameStats.totalBosses++;
        if (PlayerSavedData.instance._gameStats.totalBosses == 1)
        {
            PlayerAchievements.instance.SetAchievement("BOSS_1");
        }
        if (PlayerSavedData.instance._gameStats.totalBosses == 5)
        {
            PlayerAchievements.instance.SetAchievement("BOSS_5");
        }
        if (PlayerSavedData.instance._gameStats.totalBosses == 10)
        {
            PlayerAchievements.instance.SetAchievement("BOSS_10");
        }
    }

    public override void Spawn(bool daddy = false)
    {
        base.Spawn();
        smashTimer = 0;
        tag = "Boss";
        burstSpawner = GetComponent<CrawlerBurstSpawner>();
        burstSpawner.crawlerSpawner = crawlerSpawner;
        burstSpawner.Init();

    }
}
