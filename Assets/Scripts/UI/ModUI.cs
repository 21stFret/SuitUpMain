using Micosmo.SensorToolkit.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Reflection;
using TMPro;

public class ModUI : MonoBehaviour
{
    public static ModUI instance;
    public GameObject modUI;
    public Image BG;
    public Image buildImage, buildImage2;
    public TMP_Text buildName;
    public Sprite[] buildImages;
    public ModButton[] modButtons;
    public WeaponModManager weaponModManager;
    public EventSystem eventSystem;
    public RunUpgradeManager runUpgradeManager;
    public Pickup pickup;
    public TMP_Text RerollCost;
    [Header("Replace Info")]
    public GameObject replacePanel;
    public List<ModStats> modStats;
    public TMP_Text modNameText;
    public TMP_Text descriptionText;
    public TMP_Text rarityText;


    public void OpenModUI(ModBuildType type)
    {
        modStats = new List<ModStats>();
        modStats.AddRange(replacePanel.GetComponentsInChildren<ModStats>(true));
        for (int i = 0; i < modButtons.Length; i++)
        {
            modButtons[i].modUI = this;
        }
        GameUI.instance.droneController.CloseMenu();
        GameManager.instance.SwapPlayerInput("UI");
        SetBuildImages(type);
        modUI.SetActive(true);
        for (int i = 0; i < modButtons.Length; i++)
        {
            var button = modButtons[i];
            for (int j = 0; j < button.modStats.Length; j++)
            {
                var stat = button.modStats[j];
                stat.gameObject.SetActive(false);
            }
            button.gameObject.SetActive(false);
        }
        DisplayAllMods();
        eventSystem.SetSelectedGameObject(modButtons[0].gameObject);
        SetRerollCost(runUpgradeManager.RerollCost);
        CashCollector.instance.SetUI();
    }

    public void SetRerollCost(int cost)
    {
        RerollCost.text = "-" + cost.ToString();
    }

    public void SHowHiddenMenu()
    {
        GameManager.instance.SwapPlayerInput("UI");
        modUI.SetActive(true);
        eventSystem.SetSelectedGameObject(modButtons[0].gameObject);
    }

    private void SetBuildImages(ModBuildType type)
    {
        //BG.material.color = pickup.pickupColor;
        buildImage.sprite = buildImages[(int)type];
        buildImage2.sprite = buildImages[(int)type];
        buildName.text = type.ToString();
    }

    public void CloseModUI()
    {
        modUI.SetActive(false);
        CashCollector.instance.HideUI();
        GameManager.instance.SwapPlayerInput("Gameplay");
    }

    public void DisplayAllMods()
    {
        WeaponType weaponType = WeaponsManager.instance.currentAltWeapon.weaponType;
        int buttonIndex = 0;
        weaponModManager.LoadCurrentWeaponMods(weaponType);

        for (int i = 0; i < runUpgradeManager.listMods.Count; i++)
        {
            DisplayMod(runUpgradeManager.listMods[i], buttonIndex);
            buttonIndex++;
        }
    }

    private void DisplayMod(RunMod mod, int buttonIndex)
    {
        var button = modButtons[buttonIndex];
        button.gameObject.SetActive(true);
        button.modName.text = mod.modName;
        button.modImage.sprite = mod.sprite;
        button.modDescription.text = mod.modDescription;
        button.modType.text = mod.modCategory.ToString();
        button.CheckIfModEquipped(mod.modCategory);
        string rarity = "";
        Color color = Color.white;
        switch(mod.rarity)
        {
            case 0:
                rarity = "Basic";
                break;
            case 1:
                rarity = "Rare";
                color = Color.cyan;
                break;
            case 2:
                rarity = "Epic";
                color = Color.magenta;
                break;

        }
        button.modRarity.text = rarity;
        button.modRarity.color = color;
        button.modRarityImage.color = color;
        for (int j = 0; j < mod.modifiers.Count; j++)
        {
            var modifier = mod.modifiers[j];
            var stat = button.modStats[j];
            stat.gameObject.SetActive(true);
            stat.modStat.text = ReplaceUnderscoreWithSpace(modifier.statType.ToString());
            string modValue = modifier.statValue.ToString();
            if (modifier.statValue < 0)
            {
                stat.modStatValue.color = Color.red;
            }
            else
            {
                stat.modStatValue.color = Color.green;
                modValue = "+" + modValue;
            }

            stat.modStatValue.text = modValue;

            if (modifier.statType == StatType.Unique)
            {
                continue;
            }
            stat.modStatValue.text += "%";
        }
        
    }

    public static string ReplaceUnderscoreWithSpace(string input)
    {
        return input.Replace("_", " ");
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
        if (replacePanel == null)
        {
            return;
        }
        position.x -= 750;
        position.y -= 200;
        replacePanel.GetComponent<RectTransform>().localPosition = position;
        replacePanel.SetActive(true);
    }
}
