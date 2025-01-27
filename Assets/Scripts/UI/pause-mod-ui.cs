using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseModUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseModPanel;
    public GameObject infoPopup;
    [SerializeField] private TextMeshProUGUI modNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI rarityText;
    public List<ModStats> modStats;

    [Header("Dependencies")]
    public RunUpgradeManager runUpgradeManager;
    public Transform modEntryParent;
    public Transform statEntryParent;
    public PauseMenu pauseMenu;

    private List<ModEntry> modEntryPool = new List<ModEntry>();
    private List<ModEntry> statEntryPool = new List<ModEntry>();

    private void Awake()
    {
        for (int i = 0; i < modEntryParent.childCount; i++)
        {
            ModEntry entry = modEntryParent.GetChild(i).GetComponent<ModEntry>();
            modEntryPool.Add(entry);
            entry.pauseModUI = this;
        }

        for (int i = 0; i < statEntryParent.childCount; i++)
        {
            ModEntry entry = statEntryParent.GetChild(i).GetComponent<ModEntry>();
            statEntryPool.Add(entry);
            entry.pauseModUI = this;
        }

        modStats = new List<ModStats>();
        modStats.AddRange(infoPopup.GetComponentsInChildren<ModStats>(true));
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
        StartCoroutine(DelaySetSelected());
    }

    private IEnumerator DelaySetSelected()
    {
        yield return new WaitForSeconds(0.2f);
        print("Setting selected button");
        pauseMenu.eventSystem.SetSelectedGameObject(pauseMenu.modSelectedButton);
    }

    public void HidePauseMods()
    {
        if (pauseModPanel != null)
            pauseModPanel.SetActive(false);
    }

    private void OnDisable()
    {
    }

    public void SetupText(RunMod mod)
    {
        foreach (ModStats stat in modStats)
        {
            stat.gameObject.SetActive(false);
        }
        if (mod.modName == "")
        {
            modNameText.text = "Empty Slot";
            descriptionText.text = "Collect mods as you go!";
            rarityText.text = "";
            return;
        }

        if (modNameText != null)
            modNameText.text = mod.modName;

        if (descriptionText != null)
        {
            descriptionText.text = mod.modDescription;
        }

        if (categoryText != null)
            categoryText.text = $"Category: {mod.modCategory}";

        if (rarityText != null)
        {
            string rarityString = mod.rarity switch
            {
                0 => "Common",
                1 => "Rare",
                2 => "Epic",
                _ => "Unknown"
            };
            rarityText.text = rarityString;
        }
        if (rarityText != null)
        {
            Color rarityColor = mod.rarity switch
            {
                0 => Color.white,
                1 => Color.cyan,
                2 => Color.magenta,
                _ => Color.white
            };
            rarityText.color = rarityColor;
        }

        for (int i = 0; i < mod.modifiers.Count; i++)
        {
            ModStats stat = modStats[i];
            Modifier modifier = mod.modifiers[i];
            stat.modStat.text = ModUI.ReplaceUnderscoreWithSpace(modifier.statType.ToString());
            stat.modStatValue.text = modifier.statValue.ToString("F2");
            stat.gameObject.SetActive(true);
        }
    }

    public void SetPopupPosition(Vector3 position)
    {
        if (infoPopup == null)
        {
            return;
        }
        infoPopup.GetComponent<RectTransform>().localPosition = position;
        infoPopup.SetActive(true);
    }
}