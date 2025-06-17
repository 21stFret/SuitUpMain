using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCircuitboardManager : MonoBehaviour
{
    public static UpgradeCircuitboardManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public RunUpgradeManager runUpgradeManager;
    public List<ChipSlotUI> slots = new List<ChipSlotUI>();
    public ComboLock[] comboLocks;
    public RunMod currentRunMod;
    public ChipSlotUI currentChipSlot;
    public GameObject firstSelectedChipSlot;
    public PauseModUI pauseModUI;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        runUpgradeManager = GameManager.instance.runUpgradeManager;
        foreach (var slot in slots)
        {
            slot.runUpgradeManager = runUpgradeManager;
            slot.upgradeCircuitboardManager = this;
            slot.currentRunMod = null;
            slot.chipButton.onClick.AddListener(() => slot.PlaceChip());
        }
    }

    public void UpdateModEntry(RunMod runMod)
    {
        if (pauseModUI != null)
        {
            pauseModUI.UpdateModEntry(runMod);
        }
    }

}

public class ComboLock
{
    public ModBuildType[] buildLocks;
    public bool isLocked;
    public bool isUnlocked;

    public void ResetLock()
    {
        isLocked = true;
        isUnlocked = false;
    }

    public bool TryUnlock(List<ModBuildType> modTypes)
    {
        int matchCount = 0;
        foreach (var type in modTypes)
        {
            if (Array.Exists(buildLocks, element => element == type))
            {
                matchCount++;
            }
        }
        if (matchCount >= buildLocks.Length)
        {
            isLocked = false;
            isUnlocked = true;
            return true;
        }
        return false;
    }
    
    
}
