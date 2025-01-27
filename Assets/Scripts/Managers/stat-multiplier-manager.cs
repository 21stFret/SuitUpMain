using System;
using System.Collections.Generic;
using UnityEngine;

public class StatMultiplierManager : MonoBehaviour
{
    [Serializable]
    public class Stat
    {
        public StatType type;
        public float baseValue;
        public float currentValue;
        private List<float> multipliers = new List<float>();

        public void AddMultiplier(float percentageIncrease)
        {
            float multiplier = percentageIncrease / 100f;
            multipliers.Add(multiplier);
            UpdateCurrentValue();
        }

        public void RemoveMultiplier(float percentageIncrease)
        {
            float multiplier = percentageIncrease / 100f;
            multipliers.Remove(multiplier);
            UpdateCurrentValue();
        }

        public void ClearMultipliers()
        {
            multipliers.Clear();
            UpdateCurrentValue();
        }

        private void UpdateCurrentValue()
        {
            // this method accumlates the current value by multiplying the base value by each multiplier
            /*
            currentValue = baseValue;
            foreach (float multiplier in multipliers)
            {
                currentValue *= (1 + multiplier);
            }
            */

            //This method adds the mulitpleis up and then applyies it to the base value
            currentValue = baseValue;
            float totalMultiplier = 1;
            foreach (float multiplier in multipliers)
            {
                totalMultiplier += multiplier;
            }
            currentValue *= totalMultiplier;
        }

        public void SetBaseValue(float value)
        {
            baseValue = value;
            UpdateCurrentValue();
        }
    }

    [SerializeField] private List<Stat> stats = new List<Stat>();
    private Dictionary<StatType, Stat> statDictionary = new Dictionary<StatType, Stat>();

    public void LoadBaseValues(Dictionary<StatType, float> baseValues)
    {
        foreach (var kvp in baseValues)
        {
            if (statDictionary.TryGetValue(kvp.Key, out Stat stat))
            {
                stat.SetBaseValue(kvp.Value);
            }
            else
            {
                // If the stat doesn't exist, create a new one
                Stat newStat = new Stat { type = kvp.Key, baseValue = kvp.Value };
                stats.Add(newStat);
                statDictionary[kvp.Key] = newStat;
                newStat.SetBaseValue(kvp.Value);
            }
        }
    }

    public Dictionary<StatType, float> GetAllCurrentValues()
    {
        Dictionary<StatType, float> currentValues = new Dictionary<StatType, float>();
        foreach (var stat in stats)
        {
            currentValues[stat.type] = stat.currentValue;
        }
        return currentValues;
    }

    public void AddMultiplier(StatType statType, float percentageIncrease)
    {
        if (statDictionary.TryGetValue(statType, out Stat stat))
        {
            stat.AddMultiplier(percentageIncrease);
            SetStat(statType, percentageIncrease);

        }
        else
        {
            Debug.LogWarning($"Stat '{statType}' not found.");
        }
    }

    public void RemoveMultiplier(StatType statType, float percentageIncrease)
    {
        if (statDictionary.TryGetValue(statType, out Stat stat))
        {
            stat.RemoveMultiplier(percentageIncrease);
            float value = GetCurrentValue(statType);
            SetStat(statType, value);
        }
        else
        {
            Debug.LogWarning($"Stat '{statType}' not found.");
        }
    }

    private void SetStat(StatType statType, float percentageIncrease)
    {
        switch (statType)
        {
            case StatType.Assault_Damage:
                // DOne in targetHealth
                Debug.Log($"MWD Increased by {percentageIncrease}");
                break;
            case StatType.Tech_Damage:
                // DOne in targetHealth
                Debug.Log($"AWD Increased by {percentageIncrease}");
                break;
            case StatType.Health:
                Debug.Log("Health");
                // DOne in targetHealth
                BattleMech.instance.targetHealth.SetNewMaxHealth();
                break;
            case StatType.Fuel_Tank:
                // DOne in WeaponFuel Manager
                Debug.Log($"Fuel Rate Increased by {percentageIncrease}");
                BattleMech.instance.weaponFuelManager.SetBonus();
                break;
            case StatType.Fire_Rate:
                Debug.Log($"Fire Rate Increased by {percentageIncrease}");
                // DOne in weaponController
                BattleMech.instance.weaponController.SetFireRate();
                break;
            case StatType.Speed:
                // DOne in MYCharacterController
                Debug.Log($"Speed Increased by {percentageIncrease}");
                BattleMech.instance.myCharacterController.SetBonusSpeed();
                break;
            case StatType.Dash_Cooldown:
                // DOne in MYCharacterController
                Debug.Log($"Speed Increased by {percentageIncrease}");
                BattleMech.instance.myCharacterController.SetDashCooldown();
                break;
            case StatType.Charge_Rate:
                Debug.Log($"Charge Rate Increased by {percentageIncrease}");
                BattleMech.instance.droneController.airDropTimer.SetChargeRate(GetCurrentValue(StatType.Charge_Rate));
                break;
            default:
                Debug.LogWarning($"Stat '{statType}' not found.");
                break;
        }
    }

    public void ClearMultipliers(StatType statType)
    {
        if (statDictionary.TryGetValue(statType, out Stat stat))
        {
            stat.ClearMultipliers();
        }
        else
        {
            Debug.LogWarning($"Stat '{statType}' not found.");
        }
    }

    public float GetCurrentValue(StatType statType)
    {
        if (statDictionary.TryGetValue(statType, out Stat stat))
        {
            return stat.currentValue;
        }
        else
        {
            Debug.LogWarning($"Stat '{statType}' not found.");
            return 0f;
        }
    }

    public float GetCurrentMultiplier(StatType statType)
    {
        if (statDictionary.TryGetValue(statType, out Stat stat))
        {
            return stat.currentValue / stat.baseValue;
        }
        else
        {
            Debug.LogWarning($"Stat '{statType}' not found.");
            return 0f;
        }
    }

    public float GetBaseValue(StatType statType)
    {
        if (statDictionary.TryGetValue(statType, out Stat stat))
        {
            return stat.baseValue;
        }
        else
        {
            Debug.LogWarning($"Stat '{statType}' not found.");
            return 0f;
        }
    }
}