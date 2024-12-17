using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PauseModUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseModPanel;
    public Transform modContainer; // Parent transform where mod entries will be spawned
    public GameObject modEntryPrefab; // Prefab for each mod entry
    
    [Header("Dependencies")]
    public RunUpgradeManager runUpgradeManager;

    private List<GameObject> spawnedModEntries = new List<GameObject>();

    private void Start()
    {
        // Ensure the panel is hidden at start
        if (pauseModPanel != null)
            pauseModPanel.SetActive(false);
    }

    public void ShowPauseMods()
    {
        // Clear any existing mod entries
        ClearModEntries();

        if (pauseModPanel != null)
            pauseModPanel.SetActive(true);

        // Display all equipped mods
        foreach (RunMod mod in runUpgradeManager.currentEquipedMods)
        {
            CreateModEntry(mod);
        }
    }

    public void HidePauseMods()
    {
        if (pauseModPanel != null)
            pauseModPanel.SetActive(false);
    }

    private void CreateModEntry(RunMod mod)
    {
        GameObject entry = Instantiate(modEntryPrefab, modContainer);
        spawnedModEntries.Add(entry);

        // Get references to UI components (adjust these based on your prefab structure)
        Image modIcon = entry.GetComponentInChildren<Image>();
        TextMeshProUGUI modNameText = entry.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI modDescriptionText = entry.transform.Find("Description")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI modCategoryText = entry.transform.Find("Category")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI rarityText = entry.transform.Find("Rarity")?.GetComponent<TextMeshProUGUI>();

        // Set the mod information
        if (modIcon != null && mod.sprite != null)
            modIcon.sprite = mod.sprite;

        if (modNameText != null)
            modNameText.text = mod.modName;

        if (modDescriptionText != null)
        {
            string description = mod.modDescription;
            // Add modifier information
            foreach (Modifier modifier in mod.modifiers)
            {
                description += $"\n• {modifier.statType}: {modifier.statValue:F2}";
            }
            modDescriptionText.text = description;
        }

        if (modCategoryText != null)
            modCategoryText.text = $"Category: {mod.modCategory}";

        if (rarityText != null)
        {
            string rarityString = mod.rarity switch
            {
                0 => "Common",
                1 => "Rare",
                2 => "Epic",
                _ => "Unknown"
            };
            rarityText.text = $"Rarity: {rarityString}";
        }
    }

    private void ClearModEntries()
    {
        foreach (GameObject entry in spawnedModEntries)
        {
            Destroy(entry);
        }
        spawnedModEntries.Clear();
    }
}
