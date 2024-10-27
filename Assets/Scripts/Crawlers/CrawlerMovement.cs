using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CrawlerMovement : MonoBehaviour
{
    private Transform target;
    [SerializeField] private Vector3 destination;
    public float speedFinal = 5f;
    public float steerSpeed = 2f;
    public float lookSpeed = 5f;
    public float stoppingDistance = 1f;
    [SerializeField] private Vector3 direction;
    private Rigidbody rb;
    public int rayAmount = 5;
    public float rayDistance = 3f;
    public LayerMask layerMask;
    public float distanceToTarget;
    public bool tracking = true;
    public float groundLevel;
    public Collider groundCollider;

    // Swarm behavior parameters
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1.0f;
    public float cohesionWeight = 1.0f;
    public float swarmRadius = 5f;
    public float desiredSeparation = 2f; // Desired distance between crawlers

    private List<CrawlerMovement> nearbySwarmMembers = new List<CrawlerMovement>();

    public ParticleSystem icedEffect;
    private bool isSlowed;
    public float slowedDuration = 2f;
    public float slowedAmount = 0.5f;

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
        UpdateNearbySwarmMembers();

        if (target != null)
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
            if (Vector3.Distance(destination, transform.position) > 1)
            {
                direction = destination - transform.position;
            }
        }

        Vector3 swarmForce = CalculateSwarmForce();
        direction += swarmForce;

        RayCastSteering();
        Debug.DrawRay(transform.position, direction * 5, Color.blue);

        var dir = Vector3.Lerp(transform.forward, direction.normalized, Time.deltaTime * steerSpeed);
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, lookSpeed * Time.deltaTime);
        float speed = speedFinal;
        if(isSlowed)
        {
            speed *= slowedAmount;
        }
        rb.MovePosition(transform.position + dir.normalized * speed * Time.deltaTime);
    }

    private void UpdateNearbySwarmMembers()
    {
        nearbySwarmMembers.Clear();
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, swarmRadius);
        foreach (Collider col in nearbyColliders)
        {
            CrawlerMovement crawler = col.GetComponent<CrawlerMovement>();
            if (crawler != null && crawler != this)
            {
                nearbySwarmMembers.Add(crawler);
            }
        }
    }

    private Vector3 CalculateSwarmForce()
    {
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;
        int count = 0;

        foreach (CrawlerMovement crawler in nearbySwarmMembers)
        {
            Vector3 diff = transform.position - crawler.transform.position;
            float distance = diff.magnitude;

            // Enhanced separation
            if (distance < desiredSeparation)
            {
                separation += diff.normalized / distance; // Inverse proportional to distance
            }

            // Alignment and Cohesion
            if (distance < swarmRadius)
            {
                alignment += crawler.rb.velocity;
                cohesion += crawler.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            alignment /= count;
            cohesion /= count;
            cohesion = (cohesion - transform.position).normalized;
        }

        // Apply weights
        separation *= separationWeight;
        alignment *= alignmentWeight;
        cohesion *= cohesionWeight;

        return separation + alignment + cohesion;
    }

    private void RayCastSteering()
    {
        for (int i = 0; i < rayAmount; i++)
        {
            var z = i - rayAmount / 2;
            Vector3 rayDirection = Quaternion.Euler(0, (90f / rayAmount) * z, 0) * transform.forward;
            var raycastPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f);

            RaycastHit hit;
            if (Physics.Raycast(raycastPos, rayDirection, out hit, rayDistance, layerMask))
            {
                if (hit.collider == groundCollider)
                {
                    continue;
                }
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

    public void ApplySlow()
    {
        isSlowed = true;
        StartCoroutine(WaitForSlow());
    }

    private IEnumerator WaitForSlow()
    {
        yield return new WaitForSeconds(slowedDuration);
        isSlowed = false;
    }
}