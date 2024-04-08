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

    public override void Attack()
    {
        base.Attack();

        if(crawlerMovement.distanceToTarget< crawlerMovement.stoppingDistance + smashDistance)
        {
            smashTimer += Time.deltaTime;
            if (smashTimer > smashCooldown)
            {
                animator.SetTrigger("Smash Attack");
                smashTimer = 0;
            }
        }
    }

    public void Smash()
    {
        smashEffect.transform.position = smashLocation.position;
        smashEffect.Play();
        Collider[] colliders = Physics.OverlapSphere(smashLocation.position, smashRadius, smashLayerMask);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = collider.transform.position - smashLocation.position;
                rb.AddForce(direction.normalized * smashForce, ForceMode.Impulse);
                if(rb.GetComponent<TargetHealth>() != null)
                {
                    float dam = (attackDamage *2) * Vector3.Distance(smashLocation.position, collider.transform.position) / smashRadius;
                    rb.GetComponent<TargetHealth>().TakeDamage(dam, WeaponType.Cralwer);
                }
            }
        }
    }
}
