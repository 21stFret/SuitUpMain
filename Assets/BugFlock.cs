using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class BugFlock : MonoBehaviour
{
    public float cohesionWeight = 1.0f;
    public float alignmentWeight = 1.0f;
    public float separationWeight = 1.5f;
    public float avoidanceWeight = 2.0f;
    public float targetWeight = 1.0f;

    public float neighborRadius = 3.0f;
    public float separationRadius = 1.0f;
    public float avoidanceRadius = 2.0f;
    public float maxSpeed = 5.0f;
    public float maxForce = 10.0f;

    public Transform target;
    public LayerMask layerMask;
    private Rigidbody rb;

    public float multiplier = 1.0f; //
    public Vector3 multipliedForce;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 cohesion = Cohesion() * cohesionWeight;
        Vector3 alignment = Alignment() * alignmentWeight;
        Vector3 separation = Separation() * separationWeight;
        Vector3 avoidance = Avoidance() * avoidanceWeight;
        Vector3 targetSeeking = SeekTarget() * targetWeight;

        Vector3 flockingForce = cohesion + alignment + separation + avoidance + targetSeeking;
        multipliedForce = flockingForce * multiplier;
        Vector3 acceleration = Vector3.ClampMagnitude(flockingForce, maxForce) / rb.mass;
        rb.velocity = Vector3.ClampMagnitude(rb.velocity + acceleration * Time.fixedDeltaTime, maxSpeed);
    }

    Vector3 Cohesion()
    {
        Vector3 centerOfMass = Vector3.zero;
        int neighborCount = 0;

        foreach (BugFlock neighbor in GetNeighbors())
        {
            centerOfMass += neighbor.transform.position;
            neighborCount++;
        }

        if (neighborCount == 0)
            return Vector3.zero;

        centerOfMass /= neighborCount;
        return (centerOfMass - transform.position).normalized;
    }

    Vector3 Alignment()
    {
        Vector3 averageVelocity = Vector3.zero;
        int neighborCount = 0;

        foreach (BugFlock neighbor in GetNeighbors())
        {
            averageVelocity += neighbor.rb.velocity;
            neighborCount++;
        }

        if (neighborCount == 0)
            return Vector3.zero;

        averageVelocity /= neighborCount;
        return (averageVelocity - rb.velocity).normalized;
    }

    Vector3 Separation()
    {
        Vector3 repulsion = Vector3.zero;
        int neighborCount = 0;

        foreach (BugFlock neighbor in GetNeighbors())
        {
            float distance = Vector3.Distance(transform.position, neighbor.transform.position);
            if (distance < separationRadius)
            {
                repulsion += (transform.position - neighbor.transform.position) / distance;
                neighborCount++;
            }
        }

        if (neighborCount == 0)
            return Vector3.zero;

        return repulsion.normalized;
    }

    Vector3 Avoidance()
    {
        Vector3 avoidanceForce = Vector3.zero;
        Collider[] obstacles = Physics.OverlapSphere(transform.position, avoidanceRadius, layerMask);

        foreach (Collider obstacle in obstacles)
        {
            if (!obstacle.CompareTag("Enemy"))
            {
                Vector3 directionToObstacle = transform.position - obstacle.ClosestPoint(transform.position);
                avoidanceForce += directionToObstacle.normalized / directionToObstacle.magnitude;
            }
        }

        return avoidanceForce.normalized;
    }

    Vector3 SeekTarget()
    {
        if (target == null)
            return Vector3.zero;

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        return directionToTarget;
    }

    List<BugFlock> GetNeighbors()
    {
        List<BugFlock> neighbors = new List<BugFlock>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, neighborRadius);

        foreach (Collider collider in colliders)
        {
            BugFlock neighbor = collider.GetComponent<BugFlock>();
            if (neighbor != null && neighbor != this)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }
}
