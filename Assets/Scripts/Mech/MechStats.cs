using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechStats : MonoBehaviour
{
    public static MechStats instance;
    public float healthmultiplier { get; private set; }
    public float speedMultiplier { get; private set; }
    public float damageMultiplier { get; private set; }
    public float fuelMulitplier { get; private set; }
    private MechHealth mechHealth;

    private void Awake()
    {
        instance = this;
        mechHealth = GetComponent<MechHealth>();
        ResetStats();
    }

    public void ResetStats()
    {
        healthmultiplier = 1;
        speedMultiplier = 1;
        damageMultiplier = 1;
        fuelMulitplier = 1;
    }

    public void ApplyStats(ModType type, float value)
    {
        switch(type)
        {
            case ModType.Health:
                healthmultiplier += value / 100;
                mechHealth.targetHealth.maxHealth = mechHealth.targetHealth.maxHealth * healthmultiplier;
                mechHealth.targetHealth.TakeDamage(-mechHealth.targetHealth.maxHealth / 10);
                break;
            case ModType.BaseDamage:
                damageMultiplier += value / 100;
                break;
            case ModType.Speed:
                speedMultiplier += value / 100;
                break;
            case ModType.FuelRate:
                fuelMulitplier += value / 100;
                break;
        }
    }
}
