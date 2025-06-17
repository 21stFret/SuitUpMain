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
            powerNode[i].TryLocks();
        }
        var modIcon = upgradeCircuitboardManager.pauseModUI.FindModEntry(currentRunMod);
        if (modIcon == null)
        {
            Debug.LogError("Mod icon not found for: " + currentRunMod.modName);
            return;
        }
        modIcon.transform.SetParent(transform.parent, false);
        modIcon.transform.position = transform.position;
        modIcon.gameObject.SetActive(true);
        upgradeCircuitboardManager.currentRunMod = null;
    }

    public void SelectChipSlot()
    {
        upgradeCircuitboardManager.currentChipSlot = this;
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
                color = Color.blue;
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
