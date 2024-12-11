using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    bool canpickup = true;
    public float healthAmount = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (!canpickup) return;

        if (other.CompareTag("Player"))
        {
            PickUp();
            RemovePickup();
        }
    }

    private void PickUp()
    {
        canpickup = false;
        if (GameManager.instance == null)
        {
            return;
        }
        BattleMech.instance.targetHealth.TakeDamage(-healthAmount);
    }

    private void RemovePickup()
    {
        Destroy(gameObject);
    }
}
