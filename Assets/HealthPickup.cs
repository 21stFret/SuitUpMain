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

    private BattleMech battleMech;

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
        if (battleMech == null)
        {
            battleMech = BattleMech.instance;
        }
        bool meterFull = false;
        if (GameManager.instance == null)
        {
            return;
        }
        if(Drone)
        {
            if(battleMech.droneController.airDropTimer.charges==3)
            {
                meterFull = true;
            }
            else
            {
                battleMech.droneController.ChargeDroneOnHit(amount);
            }

        }
        else if(Fuel)
        {
            if(battleMech.weaponFuelManager.isFull())
            {
                meterFull = true;
            }
            else
            {
                battleMech.weaponFuelManager.RefillFuel(amount);
            }

        }
        else
        {
            if (voidPickUp)
            {
                GameManager.instance.CompleteVoidRoom();
                meterFull = false;
                // 25% of max health
                float amount = battleMech.targetHealth.maxHealth * 0.25f;
                battleMech.RepairArmour(amount);
            }
            else
            {
                if (battleMech.targetHealth.isFull())
                {
                    meterFull = true;
                }
                else
                {
                    battleMech.RepairArmour(amount);
                }
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
