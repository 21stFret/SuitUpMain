using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    MWD_Increase_Percent,
    AWD_Increase_Percent,
    Time,
    Health,
    FireRate,
    FuelRate,
    Speed,
    Unique,
    AWD_Percent,
    MWD_Percent,
    Healing,
    Stun

}

[System.Serializable]
public class Modifier
{ 
    public StatType statType;
    public float statValue;
}
