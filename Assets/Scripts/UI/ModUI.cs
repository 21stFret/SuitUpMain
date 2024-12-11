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

    public void OpenModUI(ModBuildType type)
    {
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

    }

    private void SetBuildImages(ModBuildType type)
    {
        BG.material.color = pickup.pickupColor;
        buildImage.sprite = buildImages[(int)type];
        buildImage2.sprite = buildImages[(int)type];
        buildName.text = type.ToString();
    }

    public void CloseModUI()
    {
        modUI.SetActive(false);
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
        for (int j = 0; j < mod.modifiers.Count; j++)
        {
            var modifier = mod.modifiers[j];
            var stat = button.modStats[j];
            stat.gameObject.SetActive(true);
            stat.modStat.text = modifier.statType.ToString();
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
                return;
            }
            stat.modStatValue.text += "%";
        }
        
    }
}
