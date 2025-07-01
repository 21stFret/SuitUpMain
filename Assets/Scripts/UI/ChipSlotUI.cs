using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ChipSlotUI : MonoBehaviour
{
    public UpgradeCircuitboardManager upgradeCircuitboardManager;
    public RunUpgradeManager runUpgradeManager;
    public Image connectorLineA, connectorLineB;
    public ModBuildType modBuildType;
    public RunMod currentRunMod = null;
    public Button chipButton;
    public PowerNode[] powerNode;
    public GameObject chipSlotHighlight;
    public bool doublePowerNode = false;
    public List<ChipSlotUI> linkedChipSlots;
    public bool isPowered = true;
    private ModEntry _modEntry;

    public void InteractWithSlot()
    {
        if (upgradeCircuitboardManager.readOnly)
        {
            Debug.LogError("Cannot interact with chip slot in read-only mode.");
            return;
        }
        if (currentRunMod != null)
            {
                if (upgradeCircuitboardManager.currentRunMod != null)
                {
                    Debug.LogError("Cannot place a new chip while one is already selected.");
                    return;
                }
                RemoveChip();
                return;
            }
        if (upgradeCircuitboardManager.currentRunMod == null)
        {
            print("No mod selected to place in chip slot.");
            return;
        }
        PlaceChip();
        if (upgradeCircuitboardManager.currentRunMod == null)
        {
            upgradeCircuitboardManager.CloseMenuButton.enabled = true;
        }
    }

    public void PlaceChip()
    {
        if (upgradeCircuitboardManager.currentRunMod == null && currentRunMod != null)
        {
            upgradeCircuitboardManager.currentRunMod = currentRunMod;
            currentRunMod = null;
            SetConnectorLinesActive(ModBuildType.UPGRADE);
            Debug.LogError("No mod selected to place in chip slot.");
            return;
        }
        currentRunMod = upgradeCircuitboardManager.currentRunMod;

        if (doublePowerNode)
        {
            foreach (var mod in currentRunMod.modifiers)
            {
                mod.statValue *= 2;
            }
        }

        if (isPowered)
        {
            SetModifiers(true);
        }
        
        SetConnectorLinesActive(currentRunMod.modBuildType);

        for (int i = 0; i < powerNode.Length; i++)
        {
            powerNode[i].chipSlots.Add(this);
            powerNode[i].TryLocks();
        }

        if (!doublePowerNode)
        {
            for (int i = 0; i < linkedChipSlots.Count; i++)
            {
                linkedChipSlots[i].linkedChipSlots.Add(this);
                linkedChipSlots[i].CheckisPowered();
            }
        }

        var modIcon = upgradeCircuitboardManager.pauseModUI.GetStatEntry(currentRunMod.ID);
        if (modIcon == null)
        {
            Debug.LogError("Mod icon not found for: " + currentRunMod.modName);
            return;
        }
        modIcon.SetupMod(currentRunMod);
        modIcon.transform.SetParent(transform.parent, false);
        modIcon.transform.position = transform.position;
        modIcon.transform.rotation = transform.rotation;
        modIcon.gameObject.SetActive(true);
        modIcon.chipSlotUI = this;
        _modEntry = modIcon;
        upgradeCircuitboardManager.currentRunMod = null;
        upgradeCircuitboardManager.CloseMenuButton.enabled = true;
        chipSlotHighlight.SetActive(false);
    }

    public void RemoveChip()
    {
        if (currentRunMod == null)
        {
            Debug.LogError("No chip to remove from slot.");
            return;
        }

        upgradeCircuitboardManager._modIcon = _modEntry;
        upgradeCircuitboardManager._modIcon.OnHoverExit();
        upgradeCircuitboardManager._modIcon.gameObject.SetActive(false);
        upgradeCircuitboardManager._modIcon.chipSlotUI = null;
        upgradeCircuitboardManager.currentRunMod = currentRunMod;
        upgradeCircuitboardManager.CloseMenuButton.enabled = false;
        _modEntry = null;

        if (isPowered)
        {
            SetModifiers(false);
        }
        if (doublePowerNode)
        {
            foreach (var mod in currentRunMod.modifiers)
            {
                mod.statValue /= 2;
            }
        }
        currentRunMod = null;
        SetConnectorLinesActive(ModBuildType.UPGRADE);
        if (!doublePowerNode)
        {
            for (int i = 0; i < linkedChipSlots.Count; i++)
            {
                linkedChipSlots[i].linkedChipSlots.Remove(this);
                linkedChipSlots[i].CheckisPowered();
            }
        }
        
        for (int i = 0; i < powerNode.Length; i++)
        {
            powerNode[i].chipSlots.Remove(this);
            powerNode[i].TryLocks();
        }

        chipSlotHighlight.SetActive(false);
    }

    private void CheckisPowered()
    {
        if (!doublePowerNode)
        {
            isPowered = true;
            return;
        }
        isPowered = false;
        if (linkedChipSlots.Count == 2)
        {
            isPowered = true;
        }
        var colors = GetComponent<Button>().colors;
        colors.normalColor = isPowered ? Color.white : Color.gray;
        GetComponent<Button>().colors = colors;
        if (currentRunMod != null)
        {
            if (!isPowered)
            {
                SetModifiers(false);
            }
            else
            {
                SetModifiers(true);
            }
        }


    }

    public void SelectChipSlot()
    {
        if (upgradeCircuitboardManager._modIcon != null)
        {
            upgradeCircuitboardManager._modIcon.OnHoverExit();
        }
        upgradeCircuitboardManager.SelectChipSlot(this);
        if (currentRunMod != null)
        {
            upgradeCircuitboardManager._modIcon = transform.parent.GetComponentInChildren<ModEntry>();
            if (upgradeCircuitboardManager._modIcon != null)
            {
                upgradeCircuitboardManager._modIcon.OnHoverEnter();
            }
        }
        if (InputTracker.instance.usingMouse)
        {
            InteractWithSlot();
        }
    }

    public void OnHoverEnter()
    {
        chipSlotHighlight.SetActive(true);
    }

    public void OnHoverExit()
    {
        chipSlotHighlight.SetActive(false);
        if (upgradeCircuitboardManager._modIcon != null)
        {
            upgradeCircuitboardManager._modIcon.OnHoverExit();
        }
        upgradeCircuitboardManager.SelectChipSlot(null);
    }

    private void SetModifiers(bool addingMod)
    {
        if( currentRunMod == null)
        {
            Debug.LogError("No mod to set modifiers for.");
            return;
        }
        if (addingMod)
        {
            runUpgradeManager.EnableModSelection(currentRunMod);
        }
        else
        {
            runUpgradeManager.DisableModSelection(currentRunMod);
        }
    }

    public void SetConnectorLinesActive(ModBuildType modCategory)
    {
        Color color = Color.white;
        switch (modCategory)
        {
            case ModBuildType.ASSAULT:
                color = Color.red;
                break;
            case ModBuildType.TECH:
                color = Color.cyan;
                break;
            case ModBuildType.TANK:
                color = Color.green;
                break;
            case ModBuildType.AGILITY:
                color = Color.yellow;
                break;
        }
        connectorLineA.color = color;
        connectorLineB.color = color;
        if (modCategory == ModBuildType.UPGRADE)
        {
            connectorLineA.color = Color.white;
            connectorLineB.color = Color.white;
            return;
        }
        modBuildType = modCategory;
    }
}
