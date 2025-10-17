using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class CrawlerMovement : MonoBehaviour
{
    [SerializeField]
    public Transform movementTarget;
    public Vector3 destination;
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
    public float wanderRadius = 10f;
    public bool canRotate = true;
    private Vector3 currentDirection;
    
    private void Start()
    {
        currentDirection = transform.forward;
    }
    private void MoveCrawler()
    {
        // 1. Calculate base direction to target
        Vector3 targetDirection = (destination - transform.position).normalized;
        Vector3 desiredDirection = targetDirection;
        
        // Get raycast start position (slightly elevated)
        var raycastPos = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        
        // 2. First check if there's a clear path to target
        bool clearPathToTarget = true;
        //float distanceToDestination = Vector3.Distance(transform.position, destination);
        float distanceToDestination = 5f;
        
        RaycastHit directHit;
        if (Physics.Raycast(raycastPos, targetDirection, out directHit, distanceToDestination, SteeringRaycast))
        {
            // Something is between us and the target
            clearPathToTarget = false;
            Debug.DrawRay(raycastPos, targetDirection * directHit.distance, Color.red);
        }
        else
        {
            // Clear path to target
            Debug.DrawRay(raycastPos, targetDirection * distanceToDestination, Color.blue);
        }
        
        // 3. If path is not clear, check for avoidance directions
        Vector3 avoidanceDirection = Vector3.zero;
        if (!clearPathToTarget)
        {
            avoidanceDirection = RayCastSteering();
        }

        // 4. Determine desired direction based on conditions
        if (!clearPathToTarget && avoidanceDirection != Vector3.zero)
        {
            // Use pure avoidance when obstacle detected and no clear path
            desiredDirection = avoidanceDirection;
            Debug.DrawRay(transform.position, avoidanceDirection * 3, Color.yellow);
        }
        // Otherwise, move directly toward target
        else if (tracking && movementTarget != null)
        {
            desiredDirection = (movementTarget.position - transform.position).normalized;
        }

        // 5. Smoothly lerp the current direction
        currentDirection = Vector3.Lerp(currentDirection, desiredDirection, Time.deltaTime * steerSpeed);
        direction = currentDirection;

        // 6. Handle rotation
        if (!canRotate)
        {
            direction = transform.forward;
        }
        else
        {
            // Apply smooth rotation
            Vector3 rotationDirection = currentDirection;
            rotationDirection.y = 0;
            if (rotationDirection == Vector3.zero)
            {
                rotationDirection = transform.forward;
            }
            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
        }

        if (!canMove) return;

        // 7. Apply movement forces
        float speed = speedFinal;
        if (isSlowed)
        {
            speed *= slowedAmount;
        }

        // 8. Apply separation ONLY if not avoiding obstacles and not attacking
        if (clearPathToTarget && !m_crawler.triggeredAttack)
        {
            Vector3 separationForce = CalculateSeparationForce();
            // Apply separation force to the current direction
            currentDirection = Vector3.Lerp(currentDirection, 
                (currentDirection + separationForce * separationWeight).normalized, 
                Time.deltaTime * steerSpeed);
        }

        Debug.DrawRay(transform.position, currentDirection * 3, Color.magenta);
        rb.AddForce(currentDirection * speed, ForceMode.Force);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);
    }

    public void ChargeMovement(float _speed)
    {
        if (movementTarget == null) return;
        // Apply force towards predicted position
        rb.AddForce(transform.forward * _speed * Time.deltaTime, ForceMode.Acceleration);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, _speed);
    }

    private Vector3 RayCastSteering()
    {
        Vector3 avoidanceDirection = Vector3.zero;
        bool obstacleDetected = false;
        int blockedRays = 0;
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
                float weight = Mathf.Pow(1f - (_hit.distance / _rayDistance), 2f); // Square for stronger close-range avoidance
                if (Mathf.Abs(z) <= 1) weight *= 2f; // Increased center ray weight

                avoidanceDirection += avoidDir * weight;

                Debug.DrawRay(raycastPos, rayDirection * _hit.distance, Color.red);
            }
            else
            {
                Debug.DrawRay(raycastPos, rayDirection * _rayDistance, Color.green);
            }
        }

            // If all rays are blocked, turn around
        if (blockedRays >= rayAmount-1)
        {
            return -transform.forward;
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
        //print("Setting movement target for: " + name + " to " + transform.name);
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

    public void SetDestination(Vector3 position, bool forceRotation=false)
    {
        destination = position;
        //print("Setting destination for: " + name + " to " + position);
        distanceToTarget = Vector3.Distance(destination, transform.position);

        if(forceRotation)
        {
            direction = (destination - transform.position).normalized;
            Vector3 dir = direction;
            dir.y = 0;
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = rot;
        }
    }

    private void FixedUpdate()
    {
        UpdateNearbySwarmMembers();
        if(destination == Vector3.zero)
        {
            //Debug.LogWarning($"[{gameObject.name}] Destination is zero in FixedUpdate! MovementTarget: {(movementTarget != null ? movementTarget.name : "null")}");
            return;
        }

        distanceToTarget = Vector3.Distance(destination, transform.position);

        CheckGround();
        if(!isGrounded)
        {
            return;
        }
        MoveCrawler();
        if(movementTarget == null)
        {
            return;
        }

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
            if (crawler != null)
            {
                if (crawler == this) continue; // Skip self
                if (crawler.m_crawler !=null)
                {
                    nearbySwarmMembers.Add(crawler);
                }
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


    public void ApplySlow(float amount, bool always = false)
    {
        slowedAmount = amount;
        isSlowed = slowedAmount > 0;
        if(!always)
        {
            StartCoroutine(WaitForSlow());
        }
    }

    private IEnumerator WaitForSlow()
    {
        yield return new WaitForSeconds(slowedDuration);
        isSlowed = false;
    }
}