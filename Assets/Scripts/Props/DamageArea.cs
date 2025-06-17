using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    public float damageRadius = 2f;
    public float damageInterval = 1f;
    public float damageAmount = 10f;
    public float damageDuration = 0f;
    public WeaponType damageType = WeaponType.Flame; // Default to Fire, but can be changed
    public bool damageActive = true; // Control whether damage is being dealt
    public bool damageOnStart = false;

    public List<TargetHealth> targetsInRange = new List<TargetHealth>();
    private SphereCollider triggerCollider;

    public float radiusExtender=1;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        damageActive = false;
        triggerCollider = gameObject.GetComponent<SphereCollider>();
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
        adjustedRadius = Mathf.Clamp(adjustedRadius, 0.1f, 10f); // Ensure radius is within a reasonable range
        triggerCollider.radius = adjustedRadius;
        triggerCollider.isTrigger = true;

        if (damageOnStart)
        {
            EnableDamageArea();
        }
    }

    public void EnableDamageArea()
    {
        damageActive = true;
        triggerCollider.enabled = true;
        targetsInRange.Clear();
        var targets = Physics.OverlapSphere(transform.position, triggerCollider.radius +1);
        foreach (var target in targets)
        {
            TargetHealth targetHealth = target.GetComponent<TargetHealth>();
            if (targetHealth != null && !targetsInRange.Contains(targetHealth))
            {
                targetsInRange.Add(targetHealth);
            }
        }
        StartCoroutine(DealDamageRoutine());
        if (damageDuration > 0)
        {
            StartCoroutine(DisableDamageAreaRoutine());
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
        while (damageActive)
        {
            for (int i = targetsInRange.Count - 1; i >= 0; i--)
            {
                if (targetsInRange[i] != null && targetsInRange[i].gameObject.activeInHierarchy)
                {
                    targetsInRange[i].TakeDamage(damageAmount, damageType);
                }
                else
                {
                    targetsInRange.RemoveAt(i);
                }
            }
            yield return new WaitForSeconds(damageInterval);
        }
    }

    private IEnumerator DisableDamageAreaRoutine()
    {
        yield return new WaitForSeconds(damageDuration);
        damageActive = false;
        triggerCollider.enabled = false;
        targetsInRange.Clear();
    }

    // Public method to activate/deactivate damage
    public void SetDamageActive(bool active)
    {
        damageActive = active;
        triggerCollider.enabled = active;
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