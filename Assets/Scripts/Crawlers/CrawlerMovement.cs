using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class CrawlerMovement : MonoBehaviour
{
    [SerializeField]
    private Transform movementTarget;
    [SerializeField] private Vector3 destination;
    public float speedFinal;
    public float steerSpeed = 2f;
    public float lookSpeed = 5f;
    public float stoppingDistance = 1f;
    public bool canMove = true;
    [SerializeField] private Vector3 direction;
    private Rigidbody rb;
    public int rayAmount = 5;
    public float rayDistance = 3f;
    public LayerMask SteeringRaycast;
    public float distanceToTarget;
    public bool tracking = true;

    // Swarm behavior parameters
    public float separationWeight = 1.5f;
    public float swarmRadius = 5f;
    public float desiredSeparation = 2f; // Desired distance between crawlers

    private List<CrawlerMovement> nearbySwarmMembers = new List<CrawlerMovement>();

    public ParticleSystem icedEffect;
    private bool isSlowed;
    public float slowedDuration = 2f;
    public float slowedAmount = 0.5f;

    public Crawler m_crawler;
    private bool isGrounded;

    [SerializeField] private float obstacleAvoidanceWeight = 2f;

    public float wanderRadius = 10f;

    private void MoveCrawler()
    {

        // 1. Calculate base direction to target
        Vector3 targetDirection = (destination - transform.position).normalized;
        direction = targetDirection;

        // 2. Check for obstacles
        Vector3 avoidanceDirection = RayCastSteering();

        if (avoidanceDirection != Vector3.zero)
        {
            direction = (targetDirection + avoidanceDirection * obstacleAvoidanceWeight).normalized;
        }
        else
        {
            if (!m_crawler.triggeredAttack)
            {
                Vector3 separationForce = CalculateSeparationForce();
                direction = (direction + separationForce * separationWeight).normalized;
            }        
        }

        // Visualization
        Debug.DrawRay(transform.position, targetDirection * 5, Color.blue);
        if (avoidanceDirection != Vector3.zero)
        {
            Debug.DrawRay(transform.position, avoidanceDirection * 3, Color.yellow);
        }

        if (tracking)
        {
            direction = (movementTarget.position - transform.position).normalized;
        }


        if (!canMove)
        {
            return;
        }

        // Apply smooth rotation
        var dir = Vector3.Lerp(transform.forward, direction, Time.deltaTime * steerSpeed);
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, lookSpeed * Time.deltaTime);


        float speed = speedFinal;
        if (isSlowed)
        {
            speed *= slowedAmount;
        }

        rb.AddForce(dir.normalized * speed, ForceMode.Force);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);
    }

    private Vector3 RayCastSteering()
    {
        Vector3 avoidanceDirection = Vector3.zero;
        bool obstacleDetected = false;
        var raycastPos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

        for (int i = 0; i < rayAmount; i++)
        {
            float _rayDistance = rayDistance;
            var z = i - rayAmount / 2;
            float rayAngle = (140f / rayAmount) * z;
            Vector3 rayDirection = Quaternion.Euler(0, rayAngle, 0) * transform.forward;

            RaycastHit _hit;
            if (Physics.Raycast(raycastPos, rayDirection, out _hit, _rayDistance, SteeringRaycast))
            {
                obstacleDetected = true;

                // Get avoidance direction based on which side of the crawler the hit occurred
                Vector3 avoidDir;
                if (z < 0)  // Left side rays
                {
                    avoidDir = Vector3.Cross(_hit.normal, Vector3.up);
                }
                else  // Right side rays
                {
                    avoidDir = Vector3.Cross(Vector3.up, _hit.normal);
                }

                // Weight based on distance and center rays
                float weight = 1f - (_hit.distance / _rayDistance);
                if (Mathf.Abs(z) <= 1) weight *= 1.5f;

                avoidanceDirection += avoidDir * weight;

                Debug.DrawRay(raycastPos, rayDirection * _hit.distance, Color.red);
            }
            else
            {
                Debug.DrawRay(raycastPos, rayDirection * _rayDistance, Color.green);
            }
        }

        return obstacleDetected ? avoidanceDirection.normalized : Vector3.zero;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        distanceToTarget = Mathf.Infinity;
    }

    public void SetTarget(Transform transform, bool hivemindlock = false)
    {
        movementTarget = transform;
        m_crawler.target = transform;
        SetDestination(movementTarget.position);
        if(hivemindlock)
        {
            return;
        }
        UpdateNearbySwarmMembers();
        foreach (CrawlerMovement crawler in nearbySwarmMembers)
        {
            crawler.SetTarget(movementTarget, true);
            //print("Set target for: " + crawler.name);
        }
    }

    public void SetDestination(Vector3 position)
    {
        destination = position;
        distanceToTarget = Vector3.Distance(destination, transform.position);

    }

    private void FixedUpdate()
    {
        UpdateNearbySwarmMembers();
        if(destination == Vector3.zero)
        {
            return;
        }

        distanceToTarget = Vector3.Distance(destination, transform.position);

        CheckGround();
        if(!isGrounded)
        {
            return;
        }
        MoveCrawler();

    }

    private void CheckGround()
    {
        RaycastHit hit;
        Vector3 raycastPos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        if (Physics.Raycast(raycastPos, Vector3.down, out hit, 1f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void SetRaycastSteering(LayerMask layerMask)
    {
        SteeringRaycast = layerMask;
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

    private Vector3 CalculateSeparationForce()
    {
        Vector3 separation = Vector3.zero;

        foreach (CrawlerMovement crawler in nearbySwarmMembers)
        {
            Vector3 diff = transform.position - crawler.transform.position;
            float distance = diff.magnitude;

            if (distance < desiredSeparation)
            {
                separation += diff.normalized / distance; // Inverse proportional to distance
            }
        }

        return separation;
    }


    public void ApplySlow(float amount)
    {
        slowedAmount = amount;
        isSlowed = true;
        StartCoroutine(WaitForSlow());
    }

    private IEnumerator WaitForSlow()
    {
        yield return new WaitForSeconds(slowedDuration);
        isSlowed = false;
    }
}