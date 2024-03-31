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
    private Vector3 targetLocation;
    public float aimSpeed = 5f;
    private bool inflight;

    private void Awake()
    {

    }

    public void Init(int damage, Transform target)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(true);
        _damage = damage;
        targetLocation = target.position;
        transform.forward = Vector3.up;
        inflight = true;
    }

    private void Update()
    {
        if(!inflight)
        {
            return;
        }
        transform.forward = Vector3.Lerp(transform.forward, targetLocation - transform.position, Time.deltaTime * aimSpeed);
        _rigidbody.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
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
                print("Explode");
                Vector3 direction = collider.transform.position - transform.position;
                rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
                if (rb.GetComponent<TargetHealth>() != null)
                {
                    rb.GetComponent<TargetHealth>().TakeDamage(_damage, null);
                }
            }
        }
        //Instantiate(explosionEffect, transform.position, quaternion.identity);
        gameObject.SetActive(false);
    }


}
