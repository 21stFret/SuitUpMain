using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectWeaponHolderToManager : MonoBehaviour
{
    public MechWeapon[] mainWeapons;
    public MechWeapon[] altWeapons;

    void Start()
    {
        WeaponsManager.instance.GetWeaponsFromHolder(this);
    }

}
