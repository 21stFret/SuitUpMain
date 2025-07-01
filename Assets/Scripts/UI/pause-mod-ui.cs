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
    public Transform statEntryParent;
    public PauseMenu pauseMenu;

    public List<ModEntry> modEntryPool = new List<ModEntry>();
    public List<ModEntry> statEntryPool = new List<ModEntry>();
    public List<ModEntry> placedEntries = new List<ModEntry>();

    private void Awake()
    {
        for (int i = 0; i < modEntryPool.Count; i++)
        {
            modEntryPool[i].pauseModUI = this;
        }

        for (int i = 0; i < statEntryParent.childCount; i++)
        {
            ModEntry entry = statEntryParent.GetChild(i).GetComponent<ModEntry>();
            statEntryPool.Add(entry);
            entry.pauseModUI = this;
            entry._mod = new RunMod(); // Initialize _mod to avoid null reference issues
        }

        modStats = new List<ModStats>();
        modStats.AddRange(infoPopup.GetComponentsInChildren<ModStats>(true));
    }

    public void ShowPauseMods()
    {
        ModUI.instance.OpenCircuitBoard(true);
    }

    public ModEntry GetStatEntry(int modID)
    {
        for (int i = 0; i < statEntryPool.Count; i++)
        {
            ModEntry entry = statEntryPool[i];
            if(entry._mod.ID == modID)
            {
                entry.gameObject.SetActive(true);
                return entry;
            }
        }
        
        for(int i = 0; i < statEntryPool.Count; i++)
        {
            ModEntry entry = statEntryPool[i];
            if (entry._mod.ID == -1)
            {
                entry.gameObject.SetActive(true);
                return entry;
            }
        }
        return null;
    }

    public ModEntry GetModEntry(ModCategory category)
    {
        foreach (ModEntry entry in modEntryPool)
        {
            if (entry._mod.modCategory == category)
            {
                entry.gameObject.SetActive(true);
                return entry;
            }
        }
        return null;
    }

    private IEnumerator DelaySetSelected()
    {
        yield return new WaitForSeconds(0.2f);
        print("Setting selected button");
        pauseMenu.eventSystem.SetSelectedGameObject(pauseMenu.modSelectedButton);
    }

    public void HidePauseMods()
    {
        ModUI.instance.CloseCircuitBoardPauseMenu();
        if (pauseModPanel != null)
            pauseModPanel.SetActive(false);
        if (infoPopup != null)
            infoPopup.SetActive(false);
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
            modNameText.text = "Unpowered Node";
            descriptionText.text = "Power up this Node to unlock its potential!";
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
            if (modifier.statValue >= 0)
            {
                stat.modStatValue.color = Color.green;
            }
            else
            {
                stat.modStatValue.color = Color.red;
            }
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