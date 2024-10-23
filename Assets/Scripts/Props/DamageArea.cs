using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    public float damageRadius = 2f;
    public float damageInterval = 0.5f;
    public float damageAmount = 1f;
    public float damageDuration = 5f;
    public float stunTime = 0f;
    public WeaponType damageType = WeaponType.Flame; // Default to Fire, but can be changed
    public bool damageActive = true; // Control whether damage is being dealt

    private List<TargetHealth> targetsInRange = new List<TargetHealth>();
    private SphereCollider triggerCollider;
    public Color gizmoColor = Color.green;

    private void Start()
    {
        damageActive = false;

        triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.radius = damageRadius;
        triggerCollider.isTrigger = true;

        StartCoroutine(DealDamageRoutine());
    }

    public void EnableDamageArea()
    {
        damageActive = true;
        targetsInRange.Clear();
        var targets = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var target in targets)
        {
            TargetHealth targetHealth = target.GetComponent<TargetHealth>();
            if (targetHealth != null && !targetsInRange.Contains(targetHealth))
            {
                targetsInRange.Add(targetHealth);
            }
        }
        if(damageDuration > 0)
        {
            StartCoroutine(DisableDamageArea());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        TargetHealth targetHealth = other.GetComponent<TargetHealth>();
        if (targetHealth != null && !targetsInRange.Contains(targetHealth))
        {
            targetsInRange.Add(targetHealth);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TargetHealth targetHealth = other.GetComponent<TargetHealth>();
        if (targetHealth != null)
        {
            targetsInRange.Remove(targetHealth);
        }
    }

    private IEnumerator DealDamageRoutine()
    {
        while (true)
        {
            if (damageActive)
            {
                for (int i = targetsInRange.Count - 1; i >= 0; i--)
                {
                    if (targetsInRange[i] != null)
                    {
                        targetsInRange[i].TakeDamage(damageAmount, damageType,stunTime);
                    }
                    else
                    {
                        targetsInRange.RemoveAt(i);
                    }
                }
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }

    private IEnumerator DisableDamageArea()
    {
        yield return new WaitForSeconds(damageDuration);
        damageActive = false;
    }

    // Public method to activate/deactivate damage
    public void SetDamageActive(bool active)
    {
        damageActive = active;
    }

    // Public method to change damage type
    public void SetDamageType(WeaponType newType)
    {
        damageType = newType;
    }

    // Public method to change damage amount
    public void SetDamageAmount(float amount)
    {
        damageAmount = amount;
    }

    // Update the damage radius
    public void SetDamageRadius(float radius)
    {
        damageRadius = radius;
        if (triggerCollider != null)
        {
            triggerCollider.radius = damageRadius;
        }
    }

    private void OnDrawGizmos()
    {
        // Store original gizmo color
        Color originalColor = Gizmos.color;

        // Set the color (you can adjust these values)
        Gizmos.color = gizmoColor;

        // Draw a wire sphere for the radius
        Gizmos.DrawWireSphere(transform.position, damageRadius);

        // Optional: Draw lines for cardinal directions
        Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * damageRadius);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * damageRadius);

        // Restore original color
        Gizmos.color = originalColor;
    }
}