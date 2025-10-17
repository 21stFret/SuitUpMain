using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillsoneTirgger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //print($"{other} entered Killzone trigger");

        Crawler crawler = other.GetComponent<Crawler>();
        if(crawler)
        {
            crawler.Die(WeaponType.Default);
        }
    }
}
