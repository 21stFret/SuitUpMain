using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    Assault_Damage,
    Tech_Damage,
    Seconds,
    Health,
    Fire_Rate,
    Fuel_Tank,
    Speed,
    Unique,
    Assault,
    Tech,
    Heals,
    Stun_Time,
    Charge_Rate,
    Dash_Cooldown,
    Pulse_Range,

}

[System.Serializable]
public class Modifier
{ 
    public StatType statType;
    public float statValue;
}
