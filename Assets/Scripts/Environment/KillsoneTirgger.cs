using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillsoneTirgger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print(other.name + " has entered the Killzone trigger");
        if (other.GetComponent<TargetHealth>() != null)
        {
            other.GetComponent<TargetHealth>().TakeDamage(1000);
        }

    }
}
