using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class SpitProjectile : MonoBehaviour
{
    public int _damage;
    public float speed;
    public float explosionRadius = 10f;
    public float explosionForce = 1000f;
    public LayerMask layerMask;
    public ParticleSystem explosionEffect;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private Vector3 targetLocation;
    public float aimSpeed = 5f;
    private bool inflight;
    public GameObject trailEffect;
    private bool aiming;
    public float aimDealay = 0.2f;

    public void Init(int damage, Transform target)
    {
        trailEffect.SetActive(true);
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _collider.enabled = true;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.constraints = RigidbodyConstraints.None;
        gameObject.SetActive(true);
        _damage = damage;
        targetLocation = target.position;
        transform.forward = Vector3.up;
        inflight = true;
        aiming = false;
        Invoke("DelayedAiming", aimDealay);
    }

    private void DelayedAiming()
    {
        aiming = true;
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
                    rb.GetComponent<TargetHealth>().TakeDamage(_damage, WeaponType.Cralwer);
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
