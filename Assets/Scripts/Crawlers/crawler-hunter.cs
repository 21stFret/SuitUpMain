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
    
    public bool isStealthed;
    private float stealthTimer;
    private Material[] materials;
    private float[] originalAlpha;
    private static readonly string ALPHA_PROPERTY = "_Alpha";  // Your shader property name
    private static readonly string FRESPOWER_PROPERTY = "_FresnelPower";  
    private bool stealthAttacked;

    public LayerMask hidenLayer;
    public LayerMask visibleLayer;

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

    public bool CheckCanStealth()
    {
        return stealthTimer <= 0 && crawlerMovement.distanceToTarget >= stealthRange;
    }

    private void Update()
    {
        if (!isStealthed)
        {
            stealthTimer -= Time.deltaTime;
        }
    }

    public void EnterStealth()
    {
        if(isStealthed)
        {
            return;
        }
        crawlerMovement.ApplySlow(0.8f, true);
        isStealthed = true;
        stealthAttacked = false;
        gameObject.layer = (int)Mathf.Log(hidenLayer.value, 2);
        StartCoroutine(FadeOut());
        if (stealthEffect != null)
        {
            stealthEffect.Play();
            animator.SetTrigger("Spit");
        }
    }

    public void RevealFromStealth()
    {
        if (!isStealthed) return;
        crawlerMovement.ApplySlow(0, true);
        isStealthed = false;
        stealthTimer = stealthCooldown;
        gameObject.layer = (int)Mathf.Log(visibleLayer.value, 2);   
        StartCoroutine(FadeIn());
        if (revealEffect != null)
        {
            //revealEffect.Play();
        }
    }

    public override void Attack()
    {
        if(stealthAttacked)
        {
            base.Attack();
            return;
        }
        StealthAttack();
    }

    public void StealthAttack()
    {
        triggeredAttack = true;
        if (!isStealthed)
        {
            return;
        }
        stealthAttacked = true;
        // Reveal before attacking
        RevealFromStealth();
       
        // Trigger attack animation
        animator.SetTrigger("StealthAttack");

    }

    public void StealthAttackHit()
    {
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
                    targetHealth.TakeDamage(stealthAttackDamage, WeaponType.Crawler);
                }
            }

        }
        StartCoroutine(DealyedAttackTrigger());
    }

    private IEnumerator DealyedAttackTrigger()
    {
        yield return new WaitForSeconds(0.5f);
        triggeredAttack = false;
    }

    private IEnumerator FadeOut()
    {
        crawlerMovement.canMove = false;
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
        crawlerMovement.canMove = false;
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
        _crawlerBehavior.TransitionToState(typeof(FleeState));
    }

    public override void TakeDamage(float damage, WeaponType killedBy, float stunTime = 0, bool invincible = false)
    {
        // Reveal when taking damage
        RevealFromStealth();
        base.TakeDamage(damage, killedBy, stunTime);
    }

    public override void Spawn(bool daddy = false)
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
        if (PlayerSavedData.instance._gameStats.totalElites >= 5)
        {
            PlayerAchievements.instance.SetAchievement("ELITE_5");
        }
        if (PlayerSavedData.instance._gameStats.totalElites >= 20)
        {
            PlayerAchievements.instance.SetAchievement("ELITE_20");
        }
        if (PlayerSavedData.instance._gameStats.totalElites >= 100)
        {
            PlayerAchievements.instance.SetAchievement("ELITE_100");
        }
    }
}
