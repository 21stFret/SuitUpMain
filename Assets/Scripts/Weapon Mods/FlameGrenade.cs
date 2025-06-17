using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameGrenade : Grenade
{
    public override void Init(float _damage, float _range)
    {
        base.Init(_damage, _range);
        weaponType = WeaponType.AoE;
    }
}