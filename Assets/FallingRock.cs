using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    public TargetHealth targetHealth;
    public Crate crate;
    public float damageAmount = 10f;
    public float damageRange = 5f;
    public float force;


    void OnCollisionEnter(Collision collision)
    {
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(10);
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRange);
        foreach (var hitCollider in hitColliders)
        {
            var targetHealth = hitCollider.GetComponent<TargetHealth>();
            if (targetHealth != null && targetHealth.alive)
            {
                targetHealth.TakeDamage(damageAmount);
            }
        }

    }

    public void Fall(Vector3 position)
    {
        crate.RefreshProp();
        transform.position = position;
        crate.transform.position = position;
        crate.rb.AddForce(Vector3.down * force, ForceMode.Impulse);
        crate.rb.AddTorque(Random.insideUnitSphere * force, ForceMode.Impulse);
    }
}
