using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrawlerLeaper : Crawler
{
    [Header("Leap Settings")]
    public float leapDistance = 15f;
    public float leapForce = 20f;
    public float leapCooldown = 3f;
    public float leapDuration = 1f;
    public float damageCheckFrequency = 0.1f;
    public ParticleSystem leapEffect;
    
    private float leapTimer;
    private bool isLeaping;
    public bool IsLeaping => isLeaping;
    private bool hasDealtDamage;
    private Vector3 leapDirection;
    public Material fangsMat;

    public override void Init()
    {
        base.Init();
        fangsMat = GetComponentInChildren<SkinnedMeshRenderer>().materials[2];
    }

    public bool CheckCanLeap()
    {
        if (isLeaping)
            return false;

        if (leapTimer < leapCooldown)
            return false;

        if (target == null)
            return false;

        if (Vector3.Distance(target.position, transform.position) > leapDistance)
            return false;

        return true;
    }

    public  void Update()
    {
        if (isLeaping)
            return;

        if (leapTimer < leapCooldown)
        {
            leapTimer += Time.deltaTime;
        }
    }

    public void StartLeapPreparation()
    {
        isLeaping = true;
        animator.SetTrigger("Leap");
    }

    // Called by animation event
    public void FlashFangs()
    {
        fangsMat.EnableKeyword("_EMISSION");
    }

    // Called by animation event
    public void Leap()
    {

        hasDealtDamage = false;
        crawlerMovement.enabled = false;
        leapTimer = 0;

        if (leapEffect != null)
        {
            leapEffect.Play();
        }

        leapDirection = (target.position - transform.position).normalized;
        leapDirection.y = 0;    // Prevent vertical leap
        transform.forward = leapDirection;
        rb.AddForce(leapDirection * leapForce, ForceMode.Impulse);
        StartCoroutine(LeapDuration());
        StartCoroutine(LeapDamage());
    }


    public IEnumerator LeapDuration()
    {
        yield return new WaitForSeconds(leapDuration);
        isLeaping = false;
        rb.velocity = Vector3.zero;
        crawlerMovement.enabled = true;
        fangsMat.DisableKeyword("_EMISSION");
        leapEffect.Stop();
    }

    private IEnumerator LeapDamage()
    {
        while (isLeaping)
        {
            CheckForDamage();
            yield return new WaitForSeconds(damageCheckFrequency);
        }
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
                    targetHealth.TakeDamage(attackDamage*2, WeaponType.Cralwer);
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

    public override void Spawn(bool daddy = false)
    {
        base.Spawn();
        leapTimer = leapCooldown;
        isLeaping = false;
        hasDealtDamage = false;
    }
}
