using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CrawlerMovement : MonoBehaviour
{
    public Transform target;
    public Vector3 destination;
    public float speed;
    public float steerSpeed;
    public float stoppingDistance;
    public Vector3 direction;
    private Rigidbody rb;
    public int rayAmount;
    public float rayDistance;
    public LayerMask layerMask;
    public float distanceToTarget;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        distanceToTarget = Mathf.Infinity;
    }

    public void SetTarget(Transform transform)
    {
        target = transform;
    }

    public void SetDestination(Vector3 position)
    {
        destination = position;
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            direction = target.position - transform.position;
            distanceToTarget = Vector3.Distance(target.position, transform.position);
        }
        else
        {
            direction = destination - transform.position;
        }
        RayCastSteering();
        Debug.DrawRay(transform.position, direction * 5, Color.blue);
        var dir = Vector3.Lerp(transform.forward, direction.normalized, Time.deltaTime * steerSpeed);
        transform.forward = Vector3.Lerp(transform.forward, direction.normalized, Time.deltaTime * steerSpeed);
        rb.MovePosition(transform.position + dir.normalized * speed * Time.deltaTime);

    }

    private void RayCastSteering()
    {
        for (int i = 0; i < rayAmount; i++)
        {
            var z = i - rayAmount/2;
            Vector3 rayDirection = Quaternion.Euler(0, (90f / rayAmount) * z, 0) * transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDirection, out hit, rayDistance, layerMask))
            {
                //Vector3 steerDirection = Vector3.Reflect(rayDirection, hit.normal);
                Vector3 steerDirection = -(hit.point - transform.position);
                Debug.DrawRay(transform.position, steerDirection, Color.red);
                // Steer away from the raycast hit
                direction = steerDirection;
            }
            else
            {
                   Debug.DrawRay(transform.position, rayDirection * rayDistance, Color.green);
            }
        }
    }
}
