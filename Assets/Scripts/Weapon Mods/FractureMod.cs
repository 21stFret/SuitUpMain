using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureMod : WeaponMod
{
    public override void Init()
    {
        base.Init();
        CryoController gun = baseWeapon as CryoController;
    }
}
