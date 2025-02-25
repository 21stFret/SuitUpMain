using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ModButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_Text modName;
    public TMP_Text modDescription;
    public TMP_Text modRarity;
    public TMP_Text modType;
    public ModStats[] modStats;
    public Image modImage;
    public Image modRarityImage;
    private bool isModEquipped = false;
    public ModUI modUI;
    public RunMod currentlyEquipedMod;

    public void OnPointerEnter(PointerEventData eventData)
    {
         if (isModEquipped)
        {
            modUI.SetupText(currentlyEquipedMod);
            modUI.SetPopupPosition(transform.localPosition);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        modUI.replacePanel.SetActive(false);
    }

    public void CheckIfModEquipped(ModCategory mod)
    { 
        if(mod == ModCategory.STATS)
        {
            isModEquipped = false;
            return;
        }
        isModEquipped = PlayerHasModEquipped(mod);
    }

    private bool PlayerHasModEquipped(ModCategory modType)
    {
        RunUpgradeManager runUpgradeManager = modUI.runUpgradeManager;
        foreach (var mod in runUpgradeManager.currentEquipedMods)
        {
            if (mod.modCategory == modType)
            {
                currentlyEquipedMod = mod;
                return true;
            }
        }
        return false; // Placeholder
    }
}


