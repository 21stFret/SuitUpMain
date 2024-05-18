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
    public Transform chargeTarget;
    public LayerMask chargeLayerMask;
    private float chargeTimer;
    public float chargeCooldown;
    public bool charging;
    public float chargeSpeed;
    private List<GameObject> chargeTargets = new List<GameObject>();

    public override void CheckDistance()
    {
        if (charging)
        {
            Charging();
            return;
        }

        if (crawlerMovement.distanceToTarget < chargeDistance)
        {

            chargeTimer += Time.deltaTime;
            if (chargeTimer > chargeCooldown)
            {
                //animator.SetTrigger("charge Attack");
                chargeTimer = 0;
                chargeTarget = target;
                StartCoroutine(Charge());
            }
        }

        if (charging)
        {
            return;
        }

        base.CheckDistance();

    }

    private void Charging()
    {
        var variForce = chargeForce;
        Collider[] colliders = Physics.OverlapSphere(transform.position, chargeRadius, chargeLayerMask);
        foreach (Collider collider in colliders)
        {
            if (chargeTargets.Contains(collider.gameObject))
            {
                continue;
            }
            TargetHealth targetHealth = collider.GetComponent<TargetHealth>();
            if (targetHealth == null)
            {
                continue;
            }
            float dam = attackDamage * 2;
            if (collider.gameObject.tag == "Player")
            {
                targetHealth.TakeDamage(dam, WeaponType.Cralwer);
            }
            else
            {
                variForce = chargeForce / 8;
            }
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = collider.transform.position - transform.position;
                rb.AddForce(direction.normalized * variForce, ForceMode.Impulse);
            }
            chargeTargets.Add(collider.gameObject);
        }
    }

    public IEnumerator Charge()
    {
        charging = true;
        var cachedSpeed = crawlerMovement.speedFinal;
        var cachedSteer= crawlerMovement.steerSpeed;
        var cachedLook = crawlerMovement.lookSpeed;
        crawlerMovement.speedFinal = chargeSpeed;
        crawlerMovement.steerSpeed = cachedSteer/2;
        crawlerMovement.lookSpeed = cachedLook/2;
        animator.speed = 2;
        chargeEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(chargeTime);
        charging = false;
        animator.speed = 1;
        crawlerMovement.speedFinal = cachedSpeed;
        crawlerMovement.steerSpeed = cachedSteer;
        crawlerMovement.lookSpeed = cachedLook;
        chargeEffect.gameObject.SetActive(false);
        chargeTargets.Clear();
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
}
