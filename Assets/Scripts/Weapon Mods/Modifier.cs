using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModType
{
    Damage,
    DamagePercent,
    Health,
    FireRate,
    Range,
    FuelRate,
    Speed,
    Unique
}

[System.Serializable]
public class Modifier
{ 
    public ModType modType;
    public float modValue;
}
