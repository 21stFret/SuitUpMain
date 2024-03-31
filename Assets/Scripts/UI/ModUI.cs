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

    private void Awake()
    {
        instance = this;
    }

    public void OpenModUI(PickupType type)
    {
        modUI.SetActive(true);
        foreach (var button in modButtons)
        {
            foreach (var stat in button.modStats)
            {
                stat.gameObject.SetActive(false);
            }
            button.gameObject.SetActive(false);
        }
        if (type == PickupType.WeaponMod)
        {
            DisplayWeaponMods();
        }
        eventSystem.SetSelectedGameObject(modButtons[0].gameObject);
    }

    public void CloseModUI()
    {
        modUI.SetActive(false);
    }

    public void DisplayWeaponMods()
    {
        for (int i = 0; i < weaponModManager.mods.Count; i++)
        {
            var mod = weaponModManager.mods[i];
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
    }
}
