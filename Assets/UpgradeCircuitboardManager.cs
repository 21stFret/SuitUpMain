using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [HideInInspector]
    public RunMod currentRunMod = null;
    public ChipSlotUI currentChipSlot;
    public GameObject firstSelectedChipSlot;
    public PauseModUI pauseModUI;
    public InputActionReference ineteractSlotAction;
    public ModEntry _modIcon;
    public Button CloseMenuButton;
    public bool readOnly;

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
        }
        ineteractSlotAction.action.Enable();
        ineteractSlotAction.action.performed += InteractWithSlot;
        CloseMenuButton.gameObject.SetActive(!readOnly);
        CloseMenuButton.enabled = true;
        if(currentRunMod != null)
        {
            CloseMenuButton.enabled = false;
        }
    }

    public void InteractWithSlot(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (currentChipSlot == null)
        {
            Debug.LogError("No chip slot selected. Please select a chip slot before interacting.");
            return;
        }
        currentChipSlot.InteractWithSlot();
    }

    public void SelectChipSlot(ChipSlotUI chipSlot)
    {
        if (currentChipSlot != null)
        {
            currentChipSlot.chipSlotHighlight.SetActive(false);
        }
        currentChipSlot = chipSlot;
        currentChipSlot.chipSlotHighlight.SetActive(true);
    }

    public void RemoveSelectedShipSlot()
    {
        if (currentChipSlot == null)
        {
            Debug.LogError("No chip slot selected to remove.");
            return;
        }
        currentChipSlot = null;
    }
}

[Serializable]
public class ComboLock
{
    public ModBuildType[] buildLocks;
    public bool isLocked;

    public void ResetLock()
    {
        isLocked = true;
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
            return true;
        }
        return false;
    }

    public void CreateRandomLock()
    {
        int randomModType = UnityEngine.Random.Range(0, 4);
        buildLocks = new ModBuildType[4];
        for (int i = 0; i < buildLocks.Length; i++)
        {
            buildLocks[i] = (ModBuildType)randomModType;
            randomModType = (randomModType + UnityEngine.Random.Range(0, 4)) % 4; // Ensure variety in mod types
        }
        ResetLock();
        isLocked = true;
    }

}
