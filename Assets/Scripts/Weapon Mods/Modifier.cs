using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModifierType
{
    Damage,
    FireRate,
    Range,
    Accuracy,
    FuelRate
}

[System.Serializable]
public struct Modifier
{ 
    public ModifierType modType;
    public float modValue;
}
