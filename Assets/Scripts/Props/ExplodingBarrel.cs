using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : Prop
{
    public float explosionForce;
    public float explosionRadius;
    public float upwardsModifier;
    public float damage;
    public ParticleSystem explosionEffect;
    public LayerMask layerMask;
    public GameObject prefab;
    public float fuseTime;
    public int fuseCount;
    public GameObject explosionRadiusPrefab;
    public Light warningLight;
    public bool isFuseActive;
    public float flashTime;
    public AudioClip[] audioClips;
    public AudioSource explosionSound;
    public AudioClip warningNoise;

    public override void Die()
    {
        isFuseActive = true;
    }

    private void Update()
    {
        if (isFuseActive)
        {
            TimerDelay();
        }
    }

    private void Explode()
    {
        isFuseActive = false;
        GetComponent<Collider>().enabled = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
        foreach (Collider collider in colliders)
        {
            TargetHealth targetHealth = collider.GetComponent<TargetHealth>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage, WeaponType.AoE);
            }
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
        }
        explosionSound.clip = audioClips[Random.Range(0, audioClips.Length)];
        explosionSound.Play();
        explosionEffect.Play();
        prefab.SetActive(false);
        
    }

    public void TimerDelay()
    {
        
        fuseTime -= Time.deltaTime;
        if(fuseCount>1)
        {
            if (fuseTime < flashTime)
            {
                warningLight.enabled = true;
                explosionRadiusPrefab.SetActive(true);
                if (!explosionSound.isPlaying)
                {
                    explosionSound.clip = warningNoise;
                    explosionSound.Play();
                }
            }
            else
            {
                warningLight.enabled = false;
                explosionRadiusPrefab.SetActive(false);
            }
        }
        else
        {
            warningLight.enabled = false;
            explosionRadiusPrefab.SetActive(false);
        }
        if (fuseTime <= 0)
        {
            fuseCount--;
            if (fuseCount <= 0)
            {
                Explode();
                return;
            }
            else
            {
                fuseTime = 1;
            }
        }
    }
}
