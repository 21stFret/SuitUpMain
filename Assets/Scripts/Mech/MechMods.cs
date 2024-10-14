using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MechMods : MonoBehaviour
{
    public List<RunMod> runMods = new List<RunMod>();
    public RunUpgradeManager runUpgradeManager; 

    public void EquipRunMod(RunMod runMod)
    {
        runMods.Add(runMod);
    }

    public void UnequipRunMod(RunMod runMod)
    {
        runMods.Remove(runMod);
    }


}
