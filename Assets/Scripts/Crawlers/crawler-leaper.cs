using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrawlerLeaper : Crawler
{
    public float leapDistance = 15f;        // Maximum distance to attempt a leap
    public float leapForce = 20f;           // Force of the leap
    public float leapCooldown = 3f;         // Time between leaps
    public float leapDuration = 1f;         // How long the leap lasts
    public float damageCheckFrequency = 0.1f; // How often to check for damage during leap
    public ParticleSystem leapEffect;       // Visual effect for leaping
    
    private float leapTimer;                // Track cooldown
    private bool isLeaping;                 // Track if currently in leap
    private bool hasDealtDamage;           // Prevent multiple hits in one leap

    public Material fangsMat;

    private Vector3 leapDirection;

    public override void Init()
    {
        base.Init();
        fangsMat = meshRenderer.materials[2];
    }

    /*
    public override void CheckDistance()
    {
        // Always count down leap timer
        leapTimer += Time.deltaTime;
        
        if (isLeaping)
        {
            return;
        }

        // Only attempt leap if in range AND cooldown is ready
        if (crawlerMovement.distanceToTarget < leapDistance && leapTimer > leapCooldown)
        {
            StartLeap();
            leapTimer = 0;
        }

        // Use normal attack behavior when very close
        if (!isLeaping)
        {
            base.CheckDistance();
        }
    }
    */

    private void StartLeap()
    {
        isLeaping = true;
        hasDealtDamage = false;
        crawlerMovement.enabled = false;    // Disable normal movement
        animator.SetTrigger("Leap");        // Trigger leap animation
    }

    public void Leap()
    {
        // Play leap effect
        if (leapEffect != null)
        {
            leapEffect.Play();
        }

        rb.AddForce(leapDirection * leapForce, ForceMode.Impulse);

        // Start leap duration timer
        StartCoroutine(LeapDuration());
        StartCoroutine(LeapDamage());
    }

    public void FlashFangs()
    {
        fangsMat.EnableKeyword("_EMISSION");
        Invoke("UnflashFangs", 2f);
        leapDirection = (target.position - transform.position).normalized;
        leapDirection.y = 0;    // Prevent vertical leap
        transform.forward = leapDirection;
    }

    public IEnumerator LeapDuration()
    {
        yield return new WaitForSeconds(leapDuration);
        isLeaping = false;
        rb.velocity = Vector3.zero;
        crawlerMovement.enabled = true;
    }

    private IEnumerator LeapDamage()
    {
        while (isLeaping)
        {
            CheckForDamage();
            yield return new WaitForSeconds(damageCheckFrequency);
        }
    }

    public void UnflashFangs()
    {
        fangsMat.DisableKeyword("_EMISSION");
    }

    private void CheckForDamage()
    {
        if (hasDealtDamage)
            return;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Player"))
            {
                var targetHealth = col.GetComponent<TargetHealth>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(attackDamage, WeaponType.Cralwer);
                    //hasDealtDamage = true;  // Prevent multiple hits
                    break;
                }
            }
        }
    }

    public override void Die(WeaponType weapon)
    {
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

    public override void Spawn()
    {
        base.Spawn();
        leapTimer = leapCooldown;  // Start ready to leap
        isLeaping = false;
        hasDealtDamage = false;
    }
}
