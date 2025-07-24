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
    private Material fangsMat;
    private Material legsMat;

    public override void Init()
    {
        base.Init();
        legsMat = GetComponentInChildren<SkinnedMeshRenderer>().materials[1];
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
                    targetHealth.TakeDamage(attackDamage*2, WeaponType.Crawler);
                    //hasDealtDamage = true;  // Prevent multiple hits
                    break;
                }
            }
        }
    }

    public override void Die(WeaponType weapon)
    {
        base.Die(weapon);
        PlayerSavedData.instance._stats.totalElites++;
        if (PlayerSavedData.instance._stats.totalElites >= 5)
        {
            PlayerAchievements.instance.SetAchievement("ELITE_5");
        }
        if (PlayerSavedData.instance._stats.totalElites >= 20)
        {
            PlayerAchievements.instance.SetAchievement("ELITE_20");
        }
        if (PlayerSavedData.instance._stats.totalElites >= 100)
        {
            PlayerAchievements.instance.SetAchievement("ELITE_100");
        }
    }

    public override void Spawn(bool daddy = false)
    {
        base.Spawn();
        leapTimer = leapCooldown;
        isLeaping = false;
        hasDealtDamage = false;
    }

    public override void MakeElite(bool _becomeElite)
    {
        base.MakeElite(_becomeElite);
        Color color = Color.red;
        if (isElite)
        {
            color = Color.red;
            leapForce = 350;
        }
        else
        {
            color = Color.green;
            leapForce = 250;
        }
        fangsMat.SetColor("_EmissionColor", color * 2);
        fangsMat.DisableKeyword("_EMISSION");
        legsMat.SetColor("_BaseColor", color);
        var main = leapEffect.main;
        main.startColor = color;
    }
}
