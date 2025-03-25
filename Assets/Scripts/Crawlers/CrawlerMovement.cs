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

    [SerializeField] private float wallStickPrevention = 2f;
    [SerializeField] private float wallCheckDistance = 1.5f;

    [Header("Stuck Detection")]
    [SerializeField] private float stuckCheckInterval = 0.5f;
    [SerializeField] private float stuckThreshold = 0.1f;
    [SerializeField] private int maxStuckChecks = 3;
    [SerializeField] private float unstuckForce = 50f;
    [SerializeField] private float upwardForce = 20f;

    private Vector3 lastPosition;
    private int stuckCounter;
    private float lastStuckCheck;

    public bool canRotate = true;

    private Vector3 currentDirection; 
    
    private void Start()
    {
        lastPosition = transform.position;
        currentDirection = transform.forward;
    }

    private void MoveCrawler()
    {
        // 1. Calculate base direction to target
        Vector3 targetDirection = (destination - transform.position).normalized;
        Vector3 desiredDirection = targetDirection;

        // 2. Check for obstacles with raycast steering
        Vector3 avoidanceDirection = RayCastSteering();

        // Visualization for target direction
        Debug.DrawRay(transform.position, targetDirection * 5, Color.blue);

        // 3. Determine desired direction - prioritize obstacle avoidance
        if (avoidanceDirection != Vector3.zero)
        {
            // When obstacle detected, use pure avoidance direction
            desiredDirection = avoidanceDirection;
            Debug.DrawRay(transform.position, avoidanceDirection * 3, Color.yellow);
        }
        // Otherwise, move toward target if tracking
        else if (tracking && movementTarget != null)
        {
            desiredDirection = (movementTarget.position - transform.position).normalized;
        }

        // 4. Smoothly lerp the current direction
        currentDirection = Vector3.Lerp(currentDirection, desiredDirection, Time.deltaTime * steerSpeed);
        direction = currentDirection;

        // 5. Handle rotation
        if (!canRotate)
        {
            direction = transform.forward;
        }
        else
        {
            // Apply smooth rotation
            Vector3 rotationDirection = currentDirection;
            rotationDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
        }

        if (!canMove) return;

        // 6. Apply movement forces
        float speed = speedFinal;
        if (isSlowed)
        {
            speed *= slowedAmount;
        }

        // 7. Apply separation ONLY if not avoiding obstacles and not attacking
        if (avoidanceDirection == Vector3.zero && !m_crawler.triggeredAttack)
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
        rb.AddForce(transform.forward * _speed, ForceMode.Acceleration);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, _speed);
    }

    private void CheckIfStuck()
    {
        float distanceMoved = Vector3.Distance(lastPosition, transform.position);
        lastPosition = transform.position;
        lastStuckCheck = 0;

        if (distanceMoved < stuckThreshold)
        {
            stuckCounter++;
            if (stuckCounter >= maxStuckChecks)
            {
                ApplyUnstuckForce();
                stuckCounter = 0;
            }
        }
        else
        {
            stuckCounter = 0;
        }
    }

    private void ApplyUnstuckForce()
    {
        // Get direction to center of map (assuming 0,0,0 is center)
        Vector3 directionToCenter = (Vector3.zero - transform.position).normalized;
        
        // Add upward component
        Vector3 unstuckDirection = (directionToCenter + Vector3.up).normalized;
        
        // Apply the force
        rb.velocity = Vector3.zero; // Clear current velocity
        rb.AddForce(unstuckDirection * unstuckForce + Vector3.up * upwardForce, ForceMode.Impulse);
        

        Debug.Log($"Unstuck force applied to {gameObject.name}");
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

    private Vector3 CheckWallSticking()
    {
        Vector3 rightCheck = transform.right;
        Vector3 leftCheck = -transform.right;
        Vector3 avoidForce = Vector3.zero;
        
        // Check right wall
        if (Physics.Raycast(transform.position, rightCheck, wallCheckDistance, SteeringRaycast))
        {
            avoidForce += -rightCheck;
        }
        
        // Check left wall
        if (Physics.Raycast(transform.position, leftCheck, wallCheckDistance, SteeringRaycast))
        {
            avoidForce += -leftCheck;
        }

        if (avoidForce != Vector3.zero)
        {
            // Add a small upward force to help break away from walls
            avoidForce += transform.forward;
            Debug.DrawRay(transform.position, avoidForce, Color.magenta);
        }

        return avoidForce.normalized;
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

    public void SetDestination(Vector3 position, bool forceRotation=false)
    {
        destination = position;
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
        lastStuckCheck += Time.deltaTime;
        if (lastStuckCheck >= stuckCheckInterval)
        {
            CheckIfStuck();
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