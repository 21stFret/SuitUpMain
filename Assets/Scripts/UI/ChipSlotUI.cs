using System.Collections;
using UnityEngine.UI;
using UnityEngine;

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


    public void InteractWithSlot()
    {
        if (currentRunMod != null)
        {
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
        runUpgradeManager.EnableModSelection(currentRunMod);
        SetConnectorLinesActive(currentRunMod.modBuildType);
        for (int i = 0; i < powerNode.Length; i++)
        {
            powerNode[i].chipSlots.Add(this);
            powerNode[i].TryLocks();
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
        upgradeCircuitboardManager.currentRunMod = null;
        upgradeCircuitboardManager.CloseMenuButton.enabled = true;
    }

    public void RemoveChip()
    {
        if (currentRunMod == null)
        {
            Debug.LogError("No chip to remove from slot.");
            return;
        }
        upgradeCircuitboardManager._modIcon = transform.parent.GetComponentInChildren<ModEntry>();
        upgradeCircuitboardManager._modIcon.chipSlotUI = null;
        upgradeCircuitboardManager.currentRunMod = currentRunMod;
        upgradeCircuitboardManager.CloseMenuButton.enabled = false;
        runUpgradeManager.DisableModSelection(currentRunMod);
        currentRunMod = null;
        SetConnectorLinesActive(ModBuildType.UPGRADE);
        for (int i = 0; i < powerNode.Length; i++)
        {
            powerNode[i].chipSlots.Remove(this);
            powerNode[i].TryLocks();
        }
        chipSlotHighlight.SetActive(false);
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
