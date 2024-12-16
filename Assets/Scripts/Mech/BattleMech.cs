using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMech : MonoBehaviour
{
    public static BattleMech instance;
    public StatMultiplierManager statMultiplierManager;
    public MYCharacterController myCharacterController;
    public WeaponController weaponController;
    public WeaponFuelManager weaponFuelManager;
    public TargetHealth targetHealth;
    public DroneController droneController;
    public bool isDead;

    private void Awake()
    {
        instance = this;
        isDead = false;
        statMultiplierManager = GetComponent<StatMultiplierManager>();
        myCharacterController = GetComponent<MYCharacterController>();
        weaponController = GetComponent<WeaponController>();
        weaponFuelManager = GetComponent<WeaponFuelManager>();
        targetHealth = GetComponent<TargetHealth>();
    }

    public void OnDie()
    {
        isDead = true;
        myCharacterController.Die();
        if(GameManager.instance != null)
        {
            GameManager.instance.EndGameCall(false);
        }
        gameObject.layer = 1;
    }

    public void RepairArmour()
    {
        targetHealth.TakeDamage(-50, WeaponType.Default);
        print("Repaired Amrour");
    }

}
