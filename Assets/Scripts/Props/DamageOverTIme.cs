using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTIme : MonoBehaviour
{
    public float damage = 1;
    public float damageTime = 1;
    public WeaponType weaponType;
    public LayerMask layerMask;
    private List<TargetHealth> targets = new List<TargetHealth>();

    private void OnEnable()
    {
        targets.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius, layerMask);
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<TargetHealth>() == null)
            {
                continue;
            }
            targets.Add(collider.GetComponent<TargetHealth>());
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(enabled == false)
        {
            return;
        }
        if(other.GetComponent<TargetHealth>() == null)
        {
            return;
        }

        targets.Add(other.GetComponent<TargetHealth>());
        StartCoroutine(DamageTarget());
    }

    private void OnTriggerExit(Collider other)
    {
        targets.Remove(other.GetComponent<TargetHealth>());
    }

    private IEnumerator DamageTarget()
    {
        while (targets.Count>0)
        {
            yield return new WaitForSeconds(damageTime);
            foreach (TargetHealth health in targets)
            {
                health.TakeDamage(damage, weaponType);
            }

        }
    }
}
