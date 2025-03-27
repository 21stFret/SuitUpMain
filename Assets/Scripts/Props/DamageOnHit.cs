using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnHit : MonoBehaviour
{
    public float damageAmount = 10f;
    public float damageRadius = 2f;
    private SphereCollider triggerCollider;
    private CapsuleCollider _triggerCollider;
    public WeaponType damageType = WeaponType.Trap;

    void Start()
    {
        float parentScale;
        if(transform.parent == null)
        {
            parentScale = transform.localScale.x;
        }
        else
        {
            parentScale = transform.parent.localScale.x;
        }
        float adjustedRadius = damageRadius / parentScale;
        //Debug.Log("Adjusted Radius: " + adjustedRadius);
        triggerCollider = gameObject.GetComponent<SphereCollider>();
        if(triggerCollider != null)
        {
            triggerCollider.radius = adjustedRadius;
        }
        _triggerCollider = gameObject.GetComponent<CapsuleCollider>();
        if(_triggerCollider != null)
        {
            _triggerCollider.radius = adjustedRadius;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        TargetHealth targetHealth = other.GetComponent<TargetHealth>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(damageAmount, damageType);
        }
    }

    private void OnDrawGizmos()
    {
        // Store original gizmo color
        Color originalColor = Gizmos.color;

        // Set the color (you can adjust these values)
        Gizmos.color = Color.red;

        // Draw a wire sphere for the radius
        Gizmos.DrawWireSphere(transform.position, damageRadius);

        // Optional: Draw lines for cardinal directions
        Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * damageRadius);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * damageRadius);

        // Restore original color
        Gizmos.color = originalColor;
    }
}
