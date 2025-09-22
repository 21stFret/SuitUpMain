using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentapideHead : Crawler
{
    public List<GameObject> bodySegments;
    public float segmentDistance;
    public float moveSpeed;
    public float rotationSpeed;
    public Transform centiTarget;
    public Transform aiTarget;
    public bool followingCentipede;
    public GameObject DeathEffect;
    public int spawnCount;
    public float explosionRadius = 10f;
    public float explosionForce = 1000f;
    public LayerMask layerMask;
    public GameObject eggs;
    
    // Path recording system
    private Queue<Vector3> pathPoints = new Queue<Vector3>();
    private Queue<Quaternion> pathRotations = new Queue<Quaternion>();
    private List<int> segmentPathIndices = new List<int>();

    public float damageInterval = 1f; // Time between damage applications
    private float damageTimer = 0f;

    private void Start()
    {
        // Initialize path indices for each segment
        for (int i = 0; i < bodySegments.Count; i++)
        {
            segmentPathIndices.Add(0);
        }
        Init();
        meshRenderer.enabled = true;
        _collider.enabled = true;
    }

    private void Update()
    {
        if(dead)
        {
            return;
        }
        // Record current head position and rotation
        RecordPathPoint();

        // Move body segments along recorded path
        MoveBodySegments();
        MoveToTargetPosition(centiTarget);

        // Handle damage application
        damageTimer += Time.deltaTime;
        if (damageTimer >= damageInterval)
        {
            damageTimer = 0f;
            DealDamageToTargetsInRange();
        }
    }

    private void DealDamageToTargetsInRange()
    {
        foreach (var segment in bodySegments)
        {
            Collider[] hitColliders = Physics.OverlapSphere(segment.transform.position, 1.5f, playerLayer);
            foreach (var hitCollider in hitColliders)
            {
                var targetHealth = hitCollider.GetComponent<TargetHealth>();
                if (targetHealth != null && targetHealth.alive)
                {
                    targetHealth.TakeDamage(10);
                }
                var rigidbody = hitCollider.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    Vector3 forceDirection = (hitCollider.transform.position - segment.transform.position).normalized;
                    rigidbody.AddForce(forceDirection * 5f, ForceMode.Impulse);
                }
            }
        }
    }

    public override void Die(WeaponType killedBy)
    {
        if (killedBy != WeaponType.Default)
        {
            Vector3 pos = transform.position;
            pos.y += 3;
            spawnCount = Random.Range(2, 5);
            crawlerSpawner.SpawnAtPoint(transform, spawnCount);
            ExplodeIfInRange();
        }
        base.Die(killedBy);
        eggs.SetActive(false);
        foreach (var segment in bodySegments)
        {
            if(segment != null)
            {
                var crawler = segment.GetComponent<Crawler>();
                if (crawler != null)
                {

                    crawler.enabled = true;
                    crawler.Init();
                    crawler.Spawn();
                    crawler.rb.useGravity = true;
                    // Exclude nothing layer
                    crawler._collider.excludeLayers = 0;
                }
            }
        }
    }

    private void ExplodeIfInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = collider.transform.position - transform.position;
                rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
                float attackDamageAfterRange = attackDamage * (1 - (Vector3.Distance(transform.position, collider.transform.position) / explosionRadius));
                if(rb.GetComponent<TargetHealth>() != null)
                {
                    rb.GetComponent<TargetHealth>().TakeDamage(attackDamageAfterRange, WeaponType.Crawler);
                }
            }
        }
    }

    private void RecordPathPoint()
    {
        // Only record if we've moved a minimum distance
        if (pathPoints.Count == 0 || Vector3.Distance(transform.position, pathPoints.ToArray()[pathPoints.Count - 1]) > 0.1f)
        {
            pathPoints.Enqueue(transform.position);
            pathRotations.Enqueue(transform.rotation);

            // Limit path length to prevent memory issues
            if (pathPoints.Count > bodySegments.Count * 50)
            {
                pathPoints.Dequeue();
                pathRotations.Dequeue();
            }
        }
    }

    private void MoveBodySegments()
    {
        Vector3[] pathArray = pathPoints.ToArray();
        Quaternion[] rotationArray = pathRotations.ToArray();
        
        for (int i = 0; i < bodySegments.Count; i++)
        {
            // Calculate desired distance behind head
            float desiredDistance = segmentDistance * (i + 1);
            
            // Find the point in our path that's closest to this distance
            int targetIndex = FindPathIndexAtDistance(pathArray, desiredDistance);
            
            if (targetIndex >= 0 && targetIndex < pathArray.Length)
            {
                // Smoothly move to the path point
                bodySegments[i].transform.position = Vector3.Lerp(
                    bodySegments[i].transform.position,
                    pathArray[targetIndex],
                    Time.deltaTime * moveSpeed
                );
                
                // Smoothly rotate to match the path rotation
                bodySegments[i].transform.rotation = Quaternion.Lerp(
                    bodySegments[i].transform.rotation,
                    rotationArray[targetIndex],
                    Time.deltaTime * rotationSpeed
                );
            }
        }
    }

    private int FindPathIndexAtDistance(Vector3[] pathArray, float targetDistance)
    {
        if (pathArray.Length < 2) return 0;
        
        float accumulatedDistance = 0f;
        
        // Start from the most recent path point (end of array) and work backwards
        for (int i = pathArray.Length - 1; i > 0; i--)
        {
            float segmentLength = Vector3.Distance(pathArray[i], pathArray[i - 1]);
            accumulatedDistance += segmentLength;
            
            if (accumulatedDistance >= targetDistance)
            {
                return i - 1; // Return the index where we exceeded the target distance
            }
        }
        
        // If we don't have enough path recorded, return the oldest point
        return 0;
    }

    private void MoveToTargetPosition(Transform target)
    {
        if (!followingCentipede)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(target.position - transform.position), rotationSpeed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 targetPosition = target.position - target.forward * segmentDistance;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            Vector3 lookDirection = (target.position - transform.position).normalized;
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
