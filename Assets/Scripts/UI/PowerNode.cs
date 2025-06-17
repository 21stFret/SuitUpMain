using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerNode : MonoBehaviour
{
    public ModCategory modCategory;
    public List<RunMod> runMods = new List<RunMod>();
    public List<ChipSlotUI> chipSlots = new List<ChipSlotUI>();
    public List<ComboLock> comboLocks = new List<ComboLock>();
    public bool empowered = false;
    public RunMod currentRunMod;

    public void TryLocks()
    {
        List<ModBuildType> modTypes = new List<ModBuildType>();
        foreach (var chip in chipSlots)
        {
            modTypes.Add(chip.modBuildType);
        }

        for (int i = 0; i < comboLocks.Count; i++)
        {
            var lockItem = comboLocks[i];
            if (lockItem.TryUnlock(modTypes))
            {
                // Handle the case when the lock is successfully unlocked
                Debug.Log("Lock unlocked!");
                if(currentRunMod != null)
                {
                    if (currentRunMod == runMods[i])
                    {
                        return; // Already empowered with this mod
                    }
                }

                empowered = true;
                GameManager.instance.runUpgradeManager.EnableModSelection(runMods[i]);
                currentRunMod = runMods[i];
                return;
            }
            else
            {
                // Handle the case when the lock is still locked
                Debug.Log("Lock still locked.");
            }
            if (empowered)
            {
                GameManager.instance.runUpgradeManager.DisableModSelection(currentRunMod);
            }
            empowered = false;
        }
    }
}
