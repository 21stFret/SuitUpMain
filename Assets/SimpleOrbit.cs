using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleOrbit : MonoBehaviour
{
    public Transform target; // The object to orbit around
    public float orbitDistance = 5f; // Distance from the target
    public float orbitSpeed = 50f; // Speed of orbiting
    public Vector3 orbitAxis = Vector3.up; // Axis to orbit around

    void Update()
    {
        if (target != null)
        {
            // Calculate the orbit position
            transform.RotateAround(target.position, orbitAxis, orbitSpeed * Time.deltaTime);
            Vector3 desiredPosition = (transform.position - target.position).normalized * orbitDistance + target.position;
            transform.position = desiredPosition;
        }
    }
}
