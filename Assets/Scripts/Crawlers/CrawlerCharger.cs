using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerCharger : Crawler
{
    public ParticleSystem chargeEffect;
    public float chargeDistance;
    public float chargeTime;
    public float chargeRadius;
    public float chargeForce;
    public float chargeDamage;
    public LayerMask chargeLayerMask;
    public LayerMask chargeLookLayerMask;
    public LayerMask normalLookLayerMask;
    public float chargeTimer;
    public float chargeCooldown;
    public bool charging;
    public float chargeSpeed;
    public List<Collider> collidersHit;

    public bool CheckCanCharge()
    {
        if (crawlerMovement.distanceToTarget < chargeDistance)
        {
            chargeTimer += Time.deltaTime;
            if (chargeTimer > chargeCooldown)
            {
                chargeTimer = 0;
                return true;
            }
        }
        return false;
    }


    public void Charging()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, chargeRadius, chargeLayerMask);
        foreach (Collider collider in colliders)
        {
            if (collidersHit.Contains(collider))
            {
                continue;
            }

            if (collider.CompareTag("Enemy"))
            {
                continue;
            }

            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = collider.transform.position - transform.position;
                rb.AddForce(direction.normalized * chargeForce, ForceMode.Impulse);
            }

            collidersHit.Add(collider);

            TargetHealth targetHealth = collider.GetComponent<TargetHealth>();
            if (targetHealth == null)
            {
                continue;
            }

            targetHealth.TakeDamage(chargeDamage, WeaponType.Crawler);

            if (collider.CompareTag("Player"))
            {
                _crawlerBehavior.TransitionToState(typeof(PursuitState));
            }

        }
    }

    public IEnumerator Charge()
    {
        charging = true;
        var cachedSpeed = crawlerMovement.speedFinal;
        var cachedSteer = crawlerMovement.steerSpeed;
        var cachedLook = crawlerMovement.lookSpeed;
        crawlerMovement.speedFinal = chargeSpeed;
        crawlerMovement.steerSpeed = cachedSteer / 2;
        crawlerMovement.lookSpeed = cachedLook / 2;
        animator.speed = 2;
        chargeEffect.Play();
        yield return new WaitForSeconds(chargeTime);
        charging = false;
        animator.speed = 1;
        crawlerMovement.speedFinal = cachedSpeed;
        crawlerMovement.steerSpeed = cachedSteer;
        crawlerMovement.lookSpeed = cachedLook;
        chargeEffect.Stop();
    }

    public override void Die(WeaponType weapon)
    {
        base.Die(weapon);
        PlayerSavedData.instance._gameStats.totalElites++;
        if (PlayerAchievements.instance == null)
        {
            return;
        }
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
