using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModType
{
    BaseDamage,
    BaseAltDamage,
    Time,
    Health,
    FireRate,
    FuelRate,
    Speed,
    Unique,
    AWD,
    MWD,
    Healing,
    Stun

}

[System.Serializable]
public class Modifier
{ 
    public ModType modType;
    public float modValue;
}
