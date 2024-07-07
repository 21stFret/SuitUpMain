using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillsoneTirgger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print("Killzone trigger enter");
        other.GetComponent<Crawler>().Die(WeaponType.Default);
    }
}
