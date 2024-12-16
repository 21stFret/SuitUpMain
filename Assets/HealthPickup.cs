using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    bool canpickup = true;
    public float amount = 10f;
    public bool voidPickUp = false;
    private Collider col;
    public  GameObject obj;
    public bool Fuel;
    public bool Drone;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

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
        if(Drone)
        {
            BattleMech.instance.droneController.ChargeDroneOnHit(amount);
        }
        else if(Fuel)
        {
            BattleMech.instance.weaponFuelManager.RefillFuel(amount);
        }
        else
        {
            BattleMech.instance.targetHealth.TakeDamage(-amount);
        }

        if (voidPickUp)
        {
            GameManager.instance.CompleteVoidRoom();
        }
    }

    private void RemovePickup()
    {
        col.enabled = false;
        obj.SetActive(false);
    }

    public void ResetPickup()
    {
        canpickup = true;
        col.enabled = true;
        obj.SetActive(true);
    }
}
