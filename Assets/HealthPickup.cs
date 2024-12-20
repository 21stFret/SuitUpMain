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
        canpickup = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canpickup) return;

        if (other.CompareTag("Player"))
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        bool meterFull = false;
        if (GameManager.instance == null)
        {
            return;
        }
        if(Drone)
        {
            if(BattleMech.instance.droneController.CanUseDrone())
            {
                meterFull = true;
            }
            else
            {
                BattleMech.instance.droneController.ChargeDroneOnHit(amount);
            }

        }
        else if(Fuel)
        {
            if(BattleMech.instance.weaponFuelManager.isFull())
            {
                meterFull = true;
            }
            else
            {
                BattleMech.instance.weaponFuelManager.RefillFuel(amount);
            }

        }
        else
        {

            if (BattleMech.instance.targetHealth.isFull())
            {
                meterFull = true;
            }
            else
            {
                BattleMech.instance.targetHealth.TakeDamage(-amount);
            }
            if (voidPickUp)
            {
                GameManager.instance.CompleteVoidRoom();
                meterFull = false;
            }

        }



        if(meterFull)
        {
            return;
        }
        RemovePickup();
    }

    private void RemovePickup()
    {
        canpickup = false;
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
