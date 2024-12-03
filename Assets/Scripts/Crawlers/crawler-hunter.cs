using UnityEngine;
using System.Collections;

public class CrawlerHunter : Crawler
{
    public float stealthAttackDamage = 50f;    
    public float stealthRange = 10f;           
    public float stealthAttackRange = 3f;      
    public float stealthCooldown = 5f;         
    public float fadeSpeed = 2f;             
    public ParticleSystem stealthEffect;       
    public ParticleSystem revealEffect;
    public float minAlpha = 0.1f;
    
    private bool isStealthed;
    private float stealthTimer;
    private Material[] materials;
    private float[] originalAlpha;
    private static readonly string ALPHA_PROPERTY = "_Alpha";  // Your shader property name
    private static readonly string FRESPOWER_PROPERTY = "_FresnelPower";  

    private bool isAttacking;

    public override void Init()
    {
        base.Init();
        // Store materials and their original alpha values
        materials = meshRenderer.materials;
        originalAlpha = new float[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalAlpha[i] = materials[i].GetFloat(ALPHA_PROPERTY);
        }
    }

    public override void CheckDistance()
    {
        if (isStealthed)
        {
            // If in stealth attack range, perform powerful attack
            if (crawlerMovement.distanceToTarget <= stealthAttackRange)
            {
                StealthAttack();
                return;
            }
        }
        else
        {
            // Count down stealth cooldown
            if (stealthTimer > 0)
            {
                stealthTimer -= Time.deltaTime;
            }
            // Try to enter stealth if in range and cooldown ready
            else if (crawlerMovement.distanceToTarget <= stealthRange)
            {
                EnterStealth();
                return;
            }

            if (isAttacking)
            {
                return;
            }
            // Use normal behavior if not handling stealth
            base.CheckDistance();
        }


    }

    private void EnterStealth()
    {
        if(isAttacking || isStealthed)
        {
            return;
        }
        isStealthed = true;
        crawlerMovement.canMove = false;
        StartCoroutine(FadeOut());
        if (stealthEffect != null)
        {
            stealthEffect.Play();
            animator.SetTrigger("Spit");
        }
    }

    private void RevealFromStealth()
    {
        if (!isStealthed) return;
        crawlerMovement.canMove = true;
        isStealthed = false;
        stealthTimer = stealthCooldown;
        StartCoroutine(FadeIn());
        if (revealEffect != null)
        {
            revealEffect.Play();
        }
    }

    private void StealthAttack()
    {
        if (!isStealthed)
        {
            return;
        }
        isAttacking = true;
        // Reveal before attacking
        RevealFromStealth();
       
        // Trigger attack animation
        animator.SetTrigger("StealthAttack");
        crawlerMovement.canMove = false;
    }

    public void StealthAttackHit()
    {
        isAttacking = false;
        crawlerMovement.canMove = true;
        animator.ResetTrigger("StealthAttack");
        // Do enhanced damage
        if (target != null)
        {
            if (crawlerMovement.distanceToTarget >= stealthAttackRange)
            {
                print("Stealth Attack Missed");
            }
            else
            {
                var targetHealth = target.GetComponent<TargetHealth>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(stealthAttackDamage, WeaponType.Cralwer);
                }
            }

        }
    }

    private IEnumerator FadeOut()
    {
        float currentAlpha = 1f;
        float currentFresPower = 1.2f;
        

        while (currentAlpha > minAlpha)
        {
            currentAlpha -= Time.deltaTime * fadeSpeed;     
            currentFresPower += Time.deltaTime * fadeSpeed *12;
            foreach (Material mat in materials)
            {
                mat.SetFloat(ALPHA_PROPERTY, currentAlpha);
                mat.SetFloat(FRESPOWER_PROPERTY, currentFresPower);
            }
            yield return null;
        }

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        crawlerMovement.canMove = true;
    }

    private IEnumerator FadeIn()
    {
        float currentAlpha = 0f;
        float currentFresPower = 0f;
        
        while (currentAlpha < 1)
        {
            currentAlpha += Time.deltaTime * fadeSpeed;
            currentFresPower -= Time.deltaTime * fadeSpeed *12;
            if(currentFresPower < 1.2f)
            {
                currentFresPower = 1.2f;
            }   
            foreach (Material mat in materials)
            {
                mat.SetFloat(ALPHA_PROPERTY, currentAlpha);
                mat.SetFloat(FRESPOWER_PROPERTY, currentFresPower);
            }
            yield return null;
        }

        // Restore original alpha values
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat(ALPHA_PROPERTY, originalAlpha[i]);
        }

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
    }

    public override void TakeDamage(float damage, WeaponType killedBy, float stunTime = 0)
    {
        // Reveal when taking damage
        RevealFromStealth();
        base.TakeDamage(damage, killedBy, stunTime);
    }

    public override void Spawn()
    {
        base.Spawn();
        isStealthed = false;
        stealthTimer = 0; // Can stealth immediately when spawning
        
        // Ensure materials are visible
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetFloat(ALPHA_PROPERTY, originalAlpha[i]);
        }
    }

    public override void Die(WeaponType weapon)
    {
        // Ensure visible when dying
        RevealFromStealth();
        base.Die(weapon);
        PlayerSavedData.instance._gameStats.totalElites++;
        if (PlayerSavedData.instance._gameStats.totalElites == 1)
        {
            PlayerAchievements.instance.SetAchievement("ELITE_1");
        }
        if (PlayerSavedData.instance._gameStats.totalElites == 5)
        {
            PlayerAchievements.instance.SetAchievement("ELITE_5");
        }
        if (PlayerSavedData.instance._gameStats.totalElites == 10)
        {
            PlayerAchievements.instance.SetAchievement("ELITE_10");
        }
    }
}
