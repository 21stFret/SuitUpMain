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
    public GameObject statInfoPanel;
    public List<StatInfoUI> statInfoUI;
    public GameObject rootNodeHighlight;

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
        if (currentRunMod != null)
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
            //Debug.LogError("No chip slot selected. Please select a chip slot before interacting.");
            return;
        }
        currentChipSlot.InteractWithSlot();
    }

    public void SelectChipSlot(ChipSlotUI chipSlot)
    {
        currentChipSlot = chipSlot;
    }

    public void OnCloseCuircuitBoard()
    {
        if (currentChipSlot != null)
        {
            currentChipSlot.chipSlotHighlight.SetActive(false);
        }
        if (_modIcon != null)
        {
            _modIcon.OnHoverExit();
        }
        currentChipSlot = null;
        _modIcon = null;
        CloseMenuButton.enabled = true;
    }

    public void ShowCurrentStats()
    {
        statInfoPanel.SetActive(true);
        rootNodeHighlight.SetActive(true);
        var allCurrentMultipliers = runUpgradeManager.GetAllCurrentMultipliers();
        
        foreach (var statInfo in statInfoUI)
        {
            statInfo.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < allCurrentMultipliers.Count && i < statInfoUI.Count; i++)
        {
            StatInfoUI statInfo = statInfoUI[i];
            if (statInfo == null)
            {
                Debug.LogError("StatInfoUI is null for index: " + i);
                continue;
            }
            
            float statValue = allCurrentMultipliers[(StatType)i];
            
            // Skip if value is 0, NaN, or infinity
            if (statValue == 0 || float.IsNaN(statValue) || float.IsInfinity(statValue))
            {
                continue;
            }
            
            statInfo.gameObject.SetActive(true);
            statInfo.UpdateStatValue(statValue * 100); // Assuming you want to show percentage
            statInfo.UpdateStatName(((StatType)i).ToString().Replace("_", " "));
        }
    }

    public void HideCurrentStats()
    {
        statInfoPanel.SetActive(false);
        rootNodeHighlight.SetActive(false);
    }
}

[Serializable]
public class ComboLock
{
    public List<ModBuildType> buildLocks;
    public bool isLocked;

    public void ResetLock()
    {
        isLocked = true;
    }

    public bool TryUnlock(List<ModBuildType> modTypes)
    {
        // Check if both lists have the same count first
        if (buildLocks.Count != modTypes.Count)
            return false;
        
        // Create sorted copies and compare
        var sortedRequired = new List<ModBuildType>(buildLocks);
        var sortedProvided = new List<ModBuildType>(modTypes);
        
        sortedRequired.Sort();
        sortedProvided.Sort();
        
        // Compare sorted lists element by element
        for (int i = 0; i < sortedRequired.Count; i++)
        {
            if (sortedRequired[i] != sortedProvided[i])
                return false;
        }
        
        isLocked = false;
        return true;
    }

    public void CreateRandomLock()
    {
        int randomModType = UnityEngine.Random.Range(0, 4);
        buildLocks.Clear();
        for (int i = 0; i < 4; i++)
        {
            buildLocks.Add((ModBuildType)randomModType);
            randomModType = (randomModType + UnityEngine.Random.Range(0, 4)) % 4; // Ensure variety in mod types
        }
        ResetLock();
        isLocked = true;
    }
    public List<ModBuildType> GetRequiredCombination()
    {
        return buildLocks; // Or whatever your field is called
    }


}
