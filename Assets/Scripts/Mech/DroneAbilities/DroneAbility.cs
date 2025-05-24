using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DroneAbility
{
    public int droneAbilityID;
    public bool unlocked;
    public string abilityName;
    public string abilityDescription;
    public string abilityChargeDescription;
    public int[] chargeInts;
    public int cost;

}
