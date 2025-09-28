using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOrbit : MonoBehaviour
{
    public Transform target; // The object to orbit around
    public float orbitDistance = 5f; // Distance from the target
    public float orbitSpeed = 50f; // Speed of orbiting
    public Vector3 orbitAxis = Vector3.up; // Axis to orbit around
    private float currentAngle; // Current angle in the orbit
    private Vector3 initialDirection; // Initial direction from target to this object
    private bool orbitDirectionClockwise = true; // Orbit direction
    private bool isInitialized = false;

    void Start()
    {
        InitializeOrbit();
    }

    void Update()
    {
        if (target != null && isInitialized)
        {
            // Update the angle based on orbit speed
            currentAngle += orbitSpeed * Time.deltaTime;

            // Calculate the new position based on the current angle
            Quaternion rotation = Quaternion.AngleAxis(currentAngle, orbitAxis);
            Vector3 rotatedDirection = rotation * initialDirection;
            if (!orbitDirectionClockwise)
            {
                rotatedDirection = -rotatedDirection;
            }
            Vector3 desiredPosition = target.position + rotatedDirection * orbitDistance;
            desiredPosition.y = target.position.y; // Keep the y position aligned with the target
            transform.position = desiredPosition;

        }
    }

    private void InitializeOrbit()
    {
        if (target != null)
        {
            // Calculate initial direction and set up orbit parameters
            initialDirection = (transform.position - target.position).normalized;
            currentAngle = 0f;
            isInitialized = true;
        }
    }

    public void ForcePositionInFrontofPlayer()
    {
        if (target != null)
        {
            // Get direction from player to current position
            Vector3 directionFromPlayer = (transform.position - target.position).normalized;
            // Flip the direction to get the opposite side
            Vector3 oppositeDirection = -directionFromPlayer;
            // Position on the opposite side at the same distance
            Vector3 desiredPosition = target.position + oppositeDirection * orbitDistance;
            desiredPosition.y = target.position.y; // Keep the y position aligned with the target
            transform.position = desiredPosition;
            // Reinitialize the orbit from this new position
            orbitDirectionClockwise = !orbitDirectionClockwise; // Reverse orbit direction
            InitializeOrbit();
        }
    }
}
