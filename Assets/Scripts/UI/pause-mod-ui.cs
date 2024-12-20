using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;

public class PauseModUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseModPanel;

    [Header("Dependencies")]
    public RunUpgradeManager runUpgradeManager;

    public List<ModEntry> modEntryPool = new List<ModEntry>();
    public List<ModEntry> statEntryPool = new List<ModEntry>();

    private void Start()
    {
        if (pauseModPanel != null)
            pauseModPanel.SetActive(false);
    }

    public void ShowPauseMods()
    {
        // Deactivate all currently active entries
        for (int i = 0; i < statEntryPool.Count; i++)
        {
            ModEntry entry = statEntryPool[i];
            entry.gameObject.SetActive(false);
        }

        if (pauseModPanel != null)
            pauseModPanel.SetActive(true);

        // Display all equipped mods
        for (int i = 0; i < runUpgradeManager.currentEquipedMods.Count; i++)
        {
            RunMod mod = runUpgradeManager.currentEquipedMods[i];
            if(mod.modCategory == ModCategory.STATS)
            {
                ModEntry stat = statEntryPool[i];
                stat.gameObject.SetActive(true);
                stat.SetupMod(mod);
                continue;
            }
            ModEntry entry = modEntryPool[(int)mod.modCategory];
            entry.SetupMod(mod);
        }
    }

    public void HidePauseMods()
    {
        if (pauseModPanel != null)
            pauseModPanel.SetActive(false);
    }

    private void OnDisable()
    {
    }
}