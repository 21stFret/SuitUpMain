using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    private TargetHealth targetHealth;
    public float damageAmount = 10f;
    public float range = 5f;

    void OnCollisionEnter(Collision collision)
    {
        targetHealth = gameObject.GetComponent<TargetHealth>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(10);
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        foreach (var hitCollider in hitColliders)
        {
            var targetHealth = hitCollider.GetComponent<TargetHealth>();
            if (targetHealth != null && targetHealth.alive)
            {
                targetHealth.TakeDamage(damageAmount);
            }
        }

    }
}
