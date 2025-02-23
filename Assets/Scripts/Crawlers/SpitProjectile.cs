using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class SpitProjectile : MonoBehaviour
{
    public float _damage;
    public float speed;
    public float explosionRadius = 10f;
    public float explosionForce = 1000f;
    public LayerMask layerMask;
    public ParticleSystem explosionEffect;
    private Rigidbody _rigidbody;
    private Collider _collider;
    public Vector3 targetLocation;
    public float aimSpeed = 5f;
    public bool inflight;
    public GameObject trailEffect;
    private bool aiming;
    public float aimDealay = 0.2f;

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
        aiming = false;
        Invoke("DelayedAiming", aimDealay);
        trailEffect.GetComponent<ParticleSystem>().Play();
    }

    private void DelayedAiming()
    {
        aiming = true;
        _collider.enabled = true;
    }

    public void Reflected(Vector3 reflectPos)
    {
        Vector3 dir = reflectPos - transform.position;
        dir.y =0;
        targetLocation = reflectPos - dir.normalized * 2;
        targetLocation.y = 0;
        aiming = false;
        transform.forward = -transform.forward;
        Invoke("DelayedAiming", aimDealay * 0.3f);
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
        transform.forward = Vector3.Lerp(transform.forward, targetLocation - transform.position, Time.deltaTime * aimSpeed);

    }

    private void OnTriggerEnter(Collider other)
    {
        print("Hit " + other.name);
        Explode();

    }

    private void Explode()
    {
        inflight = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                print("Explode hit " + rb.name);
                Vector3 direction = collider.transform.position - transform.position;
                rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
                if (rb.GetComponent<TargetHealth>() != null)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    float damage = Mathf.Clamp(_damage * (1.2f - (distance / explosionRadius)), 0, _damage);
                    rb.GetComponent<TargetHealth>().TakeDamage(damage, WeaponType.Cralwer);
                }
            }
        }
        explosionEffect.Play();
        trailEffect.SetActive(false);
        _collider.enabled = false;  
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        
    }


}
