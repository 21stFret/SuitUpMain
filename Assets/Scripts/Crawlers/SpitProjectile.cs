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
    public GameObject explosionEffect;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Init(int damage)
    {
        gameObject.SetActive(true);
        _damage = damage;
    }

    private void Update()
    {
        _rigidbody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                print("Explode");
                Vector3 direction = collider.transform.position - transform.position;
                rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
                if (rb.GetComponent<TargetHealth>().mechHealth != null)
                {
                    rb.GetComponent<TargetHealth>().TakeDamage(_damage, null);
                }
            }
        }
        Instantiate(explosionEffect, transform.position, quaternion.identity);
        gameObject.SetActive(false);
    }


}
