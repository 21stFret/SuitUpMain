using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform target; // The target to move towards
    public float speed = 5f; // Movement speed
    public float detectionDistance = 5f; // Distance to detect obstacles
    public float avoidanceStrength = 10f; // Strength of avoidance maneuver
    public Collider obstacle;

    private int side = 0; // Side to avoid obstacle
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = Random.Range(10f, 14f);
    }

    void FixedUpdate()
    {
        MoveTowardsTarget();
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        RaycastHit hit;

        // Check if there's an obstacle directly in the direction of movement
        if (Physics.Raycast(transform.position, direction, out hit, detectionDistance))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                if (obstacle != hit.collider)
                {
                    obstacle = hit.collider;
                    side = Random.Range(0, 2);
                }
                // Calculate a direction to avoid the obstacle
                Vector3 avoidanceDirection = Vector3.zero;

                if (side == 0)
                {
                    avoidanceDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
                    direction += avoidanceDirection * avoidanceStrength;
                    direction.Normalize();
                }
                else
                {
                    avoidanceDirection = Vector3.Cross(Vector3.up, hit.normal).normalized;
                    direction += avoidanceDirection * avoidanceStrength;
                    direction.Normalize();
                }
                direction += avoidanceDirection * avoidanceStrength;
                direction.Normalize();
            }
        }

        // Move the enemy
        Vector3 movement = direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Rotate the enemy to face the direction of movement
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, toRotation, Time.fixedDeltaTime * speed);
        }
    }
}
