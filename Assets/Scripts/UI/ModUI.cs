using Micosmo.SensorToolkit.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModUI : MonoBehaviour
{
    public static ModUI instance;
    public GameObject modUI;
    public ModButton[] modButtons;
    public WeaponModManager weaponModManager;
    public EventSystem eventSystem;


    public void OpenModUI(PickupType type)
    {
        GameManager.instance.SwapPlayerInput("UI");
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
        if (type == PickupType.WeaponMod)
        {
            DisplayWeaponMods();
        }
        if (type == PickupType.Bonus)
        {
            DisplayBonusMods();
        }
        eventSystem.SetSelectedGameObject(modButtons[0].gameObject);

    }

    public void CloseModUI()
    {
        modUI.SetActive(false);
        GameManager.instance.SwapPlayerInput("Gameplay");
    }

    public void DisplayWeaponMods()
    {
        WeaponType weaponType = WeaponsManager.instance.currentAltWeapon.weaponType;
        int buttonIndex = 0;
        for(int i = 0; i < modButtons.Length; i++)
        {
            modButtons[i].gameObject.SetActive(false);
        }
        weaponModManager.LoadCurrentWeaponMods(weaponType);
        for (int i = 0; i < weaponModManager.currentMods.Count; i++)
        {
            DisplayWeaponMod(weaponModManager.currentMods[i], buttonIndex);
            buttonIndex++;
        }
    }

    private void DisplayWeaponMod(WeaponMod mod, int buttonIndex)
    {
        var button = modButtons[buttonIndex];
        button.gameObject.SetActive(true);
        button.modName.text = mod.modName;
        button.modImage.sprite = mod.sprite;
        button.modDescription.text = mod.modDescription;
        for (int j = 0; j < mod.modifiers.Count; j++)
        {
            var modifier = mod.modifiers[j];
            var stat = button.modStats[j];
            stat.gameObject.SetActive(true);
            stat.modStat.text = modifier.modType.ToString();
            string modValue = modifier.modValue.ToString();
            if (modifier.modValue < 0)
            {
                stat.modStatValue.color = Color.red;
            }
            else
            {
                stat.modStatValue.color = Color.green;
                modValue = "+" + modValue;
            }
            stat.modStatValue.text = modValue + "%";
        }
    }

    public void DisplayBonusMods()
    {
        /*
        for (int i = 0; i < weaponModManager.bonusMods.Count; i++)
        {
            var mod = weaponModManager.bonusMods[i];
            var button = modButtons[i];
            button.gameObject.SetActive(true);
            button.modName.text = mod.modName;
            button.modImage.sprite = mod.sprite;
            button.modDescription.text = mod.modDescription;
            for (int j = 0; j < mod.modifiers.Count; j++)
            {
                var modifier = mod.modifiers[j];
                var stat = button.modStats[j];
                stat.gameObject.SetActive(true);
                stat.modStat.text = modifier.modType.ToString();
                string modValue = modifier.modValue.ToString();
                if (modifier.modValue < 0)
                {
                    stat.modStatValue.color = Color.red;
                }
                else
                {
                    stat.modStatValue.color = Color.green;
                    modValue = "+" + modValue;
                }
                stat.modStatValue.text = modValue + "%";
            }
        
        }
        */
    }
}
