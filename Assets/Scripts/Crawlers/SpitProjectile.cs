using System.Collections;
using System.Collections.Generic;
using FORGE3D;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class SpitProjectile : MonoBehaviour
{
    public float _damage;
    public float speed;
    public float explosionRadius = 10f;
    public float explosionForce = 1000f;
    public LayerMask targetLayer;
    public LayerMask reflectedLayer;
    public ParticleSystem explosionEffect;
    private Rigidbody _rigidbody;
    private Collider _collider;
    public Vector3 targetLocation;
    public float aimSpeed = 5f;
    public bool inflight;
    public GameObject trailEffect;
    private bool aiming;
    public float aimDelay = 0.2f;
    private bool isReflected;

    public void Init(float damage, Transform target)
    {
        trailEffect.SetActive(true);
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.constraints = RigidbodyConstraints.None;
        gameObject.SetActive(true);
        _damage = damage;
        targetLocation = target.position;
        transform.forward = Vector3.up;
        inflight = true;
        isReflected = false;
        aiming = false;
        Invoke("DelayedAiming", aimDelay);
        trailEffect.GetComponent<ParticleSystem>().Play();
    }

    private void DelayedAiming()
    {
        aiming = true;
        _collider.enabled = true;
    }

    public void Reflected(Vector3 reflectPos, float range)
    {
        Vector3 dir = reflectPos - transform.position;
        float distance = Vector3.Distance(reflectPos, transform.position);
        float t = 5 - Mathf.Clamp01(distance / range) * 5;
        t += 8f;
        dir.Normalize();
        targetLocation = transform.position - dir * t;
        targetLocation.y = 0;
        transform.forward = -dir;
        isReflected = true;
        Invoke("DelayedAiming", aimDelay*0.2f);
    }

    private void Update()
    {
        if(!inflight)
        {
            return;
        }
        _rigidbody.velocity = transform.forward * speed;

        if (!aiming)
        {
            return;
        }
        Vector3 dir = targetLocation - transform.position;
        transform.forward = Vector3.Lerp(transform.forward, dir.normalized, Time.deltaTime * aimSpeed);

    }

    private void OnTriggerEnter(Collider other)
    {
        print("Hit " + other.name);
        Explode();

    }

    private void Explode()
    {
        inflight = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, isReflected ? reflectedLayer : targetLayer);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = collider.transform.position - transform.position;
                rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
            }
            TargetHealth targetHealth = collider.GetComponent<TargetHealth>();
            if (targetHealth != null)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                float damage = Mathf.Clamp(_damage * (1.2f - (distance / explosionRadius)), 0, _damage);
                var weaponType = WeaponType.Crawler;
                if (isReflected)
                {
                    weaponType = WeaponType.AoE;
                }
                targetHealth.TakeDamage(damage, weaponType);
            }
        }
        explosionEffect.Play();
        trailEffect.SetActive(false);
        F3DAudioController.instance.SpitHit(transform.position);
        _collider.enabled = false;  
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        
    }


}
