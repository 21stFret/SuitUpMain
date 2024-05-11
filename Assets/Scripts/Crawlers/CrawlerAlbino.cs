using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float smashmovementDelay;
    private bool smashing;

    public override void CheckDistance()
    {
        if (crawlerMovement.distanceToTarget < smashDistance)
        {
            smashTimer += Time.deltaTime;
            if (smashTimer > smashCooldown)
            {
                animator.SetTrigger("Smash Attack");
                smashTimer = 0;
                StartCoroutine(SmashMovementDelay());
            }
        }
        if(smashing)
        {
            return;
        }

        base.CheckDistance();

    }

    public void Smash()
    {
        smashEffect.transform.position = smashLocation.position;
        smashEffect.Play();
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
            float dam = (attackDamage * 2) * ratio1;
            dam = Mathf.Clamp(dam, attackDamage, attackDamage * 2);
            targetHealth.TakeDamage(dam, WeaponType.Cralwer);
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = collider.transform.position - smashLocation.position;
                rb.AddForce(direction.normalized * smashForce, ForceMode.Impulse);
            }
        }

    }

    private IEnumerator SmashMovementDelay()
    {
        smashing = true;
        var cachedspeed = crawlerMovement.speed;
        crawlerMovement.speed = 0;
        yield return new WaitForSeconds(smashmovementDelay);
        crawlerMovement.speed = cachedspeed;
        smashing = false;
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
}
