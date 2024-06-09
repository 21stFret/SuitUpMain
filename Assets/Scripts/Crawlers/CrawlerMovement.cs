using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CrawlerMovement : MonoBehaviour
{
    private Transform target;
    [SerializeField]
    private Vector3 destination;
    public float speedFinal;
    public float steerSpeed;
    public float lookSpeed;
    public float stoppingDistance;
    [SerializeField]
    private Vector3 direction;
    private Rigidbody rb;
    public int rayAmount;
    public float rayDistance;
    public LayerMask layerMask;
    public float distanceToTarget;
    public bool tracking = true;
    public float groundLevel;
    public Collider groundCollider;


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
        if(target != null)
        {
            distanceToTarget = Vector3.Distance(target.position, transform.position);
        }

        if (tracking)
        {
            if (target != null)
            {
                direction = target.position - transform.position;

            }
            else
            {
                direction = destination - transform.position;
            }
        }
        else
        {
            if(Vector3.Distance(destination, transform.position) > 1)
            {
                direction = destination - transform.position;
            }

        }
        RayCastSteering();
        Debug.DrawRay(transform.position, direction * 5, Color.blue);
        var dir = Vector3.Lerp(transform.forward, direction.normalized, Time.deltaTime * steerSpeed);
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, lookSpeed * Time.deltaTime);
        rb.MovePosition(transform.position + dir.normalized * speedFinal * Time.deltaTime);

    }

    private void RayCastSteering()
    {
        for (int i = 0; i < rayAmount; i++)
        {
            var z = i - rayAmount/2;
            Vector3 rayDirection = Quaternion.Euler(0, (90f / rayAmount) * z, 0) * transform.forward;
            var raycastPos = new Vector3(transform.position.x, transform.position.y, transform.position.z +0.5f);

            RaycastHit hit;
            if (Physics.Raycast(raycastPos, rayDirection, out hit, rayDistance, layerMask))
            {
                if(hit.collider == groundCollider)
                {                     
                    continue;
                }
                //print("hit " + hit.collider.name);
                Vector3 steerDirection = -(hit.point - transform.position);
                Debug.DrawRay(transform.position, steerDirection, Color.red);
                direction = steerDirection;
            }
            else
            {
                   Debug.DrawRay(transform.position, rayDirection * rayDistance, Color.green);
            }
        }
    }
}
