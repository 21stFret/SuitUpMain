using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillsoneTirgger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<Crawler>().Die(WeaponType.Default);
        }
    }
}
