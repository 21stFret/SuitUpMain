using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerBomber : Crawler
{
    public GameObject[] bombsacks;

    public float explosionRadius = 10f;
    public float explosionForce = 1000f;
    public LayerMask layerMask;

    public float distanceToGrow;
    public float growSize;
    public float explodeSize;
    public float explosionDelay = 0.5f;
    private float explosionTImer;

    public AudioClip splatSound;

    [Header("Shake Settings")]
    [SerializeField] private float shakeIntensity = 0.2f;
    [SerializeField] private float shakeFrequency = 30f;
    private Vector3[] originalPositions;

    public override void Die(WeaponType killedBy)
    {
        overrideDeathNoise = true;
        deathNoise.clip = splatSound;
        ExplodeIfInRange();
        base.Die(killedBy);
        if(bombsacks == null || originalPositions == null)
        {
            return;
        }
        for (int i = 0; i < bombsacks.Length; i++)
        {
            if (bombsacks[i] != null && originalPositions[i] != null)
            {
                bombsacks[i].transform.localPosition = originalPositions[i];
            }
        }
    }

    private void ExplodeIfInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = collider.transform.position - transform.position;
                rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
                float attackDamageAfterRange = attackDamage * (1 - (Vector3.Distance(transform.position, collider.transform.position) / explosionRadius));
                if(rb.GetComponent<TargetHealth>() != null)
                {
                    rb.GetComponent<TargetHealth>().TakeDamage(attackDamageAfterRange, WeaponType.Crawler);
                }
            }
        }
    }

    void Update()
    {
        if(triggeredAttack)
        {
            explosionTImer += Time.deltaTime;
            foreach (var item in bombsacks)
            {
                item.transform.localScale = Vector3.Lerp(item.transform.localScale, Vector3.one*explodeSize, explosionTImer/explosionDelay);
            }
            return;
        }
        if(crawlerMovement!=null)
        {
            if(crawlerMovement.distanceToTarget <= distanceToGrow)
            {
                foreach (var item in bombsacks)
                {
                    item.transform.localScale = Vector3.Lerp(item.transform.localScale, Vector3.one*growSize, 1 - crawlerMovement.distanceToTarget / distanceToGrow);
                }
            }
            else
            {
                foreach (var item in bombsacks)
                {
                    item.transform.localScale = Vector3.Lerp(item.transform.localScale, Vector3.one, Time.deltaTime * 2);
                }
            }
        }
    }

    public override void Attack()
    {
        triggeredAttack = true;
        StartCoroutine(Explode());
        _crawlerBehavior.TransitionToState(typeof(BombState));
    }

    private IEnumerator Explode()
    {
        float elapsedTime = 0f;
        deathNoise.clip = deathSounds[Random.Range(0, deathSounds.Length)];
        deathNoise.Play();

        while (elapsedTime < explosionDelay)
        {
            for (int i = 0; i < bombsacks.Length; i++)
            {
                if (bombsacks[i] != null)
                {
                    // Create random offset based on sine wave for smooth shaking
                    Vector3 offset = new Vector3(
                        Mathf.Sin(Time.time * shakeFrequency + i) * shakeIntensity,
                        Mathf.Sin(Time.time * shakeFrequency + i + 1) * shakeIntensity,
                        Mathf.Sin(Time.time * shakeFrequency + i + 2) * shakeIntensity
                    );

                    bombsacks[i].transform.localPosition = originalPositions[i] + offset;
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Die(WeaponType.Crawler);
    }

    public override void Spawn(bool daddy = false)
    {
        base.Spawn(daddy);
        originalPositions = new Vector3[bombsacks.Length];
        for (int i = 0; i < bombsacks.Length; i++)
        {
            originalPositions[i] = bombsacks[i].transform.localPosition;
        }
        DeathBlood.transform.SetParent(transform);
    }

}
