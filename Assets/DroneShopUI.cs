using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class DroneShopUI : MonoBehaviour
{
    public DroneAbilityManager droneAbilityManager;
    public TMP_Text abilityNameText;
    public TMP_Text abilityDescriptionText;
    public TMP_Text abilityPriceText;
    public TMP_Text buttonText;
    public List<DroneOptionShopUI> droneShopUIs = new List<DroneOptionShopUI>();
    public List<DroneOptionShopUI> equipOptions = new List<DroneOptionShopUI>();
    public GameObject startingButton;
    public GameObject shopMenu;
    public GameObject cost;
    public GameObject artifactCost;
    public DoTweenFade fade;
    public VideoPlayer videoPlayer;
    public List<VideoClip> videoClips = new List<VideoClip>();
    public DroneOptionShopUI currentSelectedOption;
    public DroneOptionShopUI currentSelectedEquipSlot;
    private DroneAbility currentSelectedAbility = null;
    public InputActionReference inputUpgrade;
    private DroneControllerUI droneControllerUI;

    public void Init()
    {
        droneAbilityManager = DroneAbilityManager.instance;
        droneControllerUI = droneAbilityManager.droneControllerUI;
        inputUpgrade.action.Enable();
        inputUpgrade.action.performed += PurchaseAbility;


        SetupButtons();

        currentSelectedAbility = null;
        currentSelectedOption = null;
        currentSelectedEquipSlot = null;
    }

    private void SetupButtons()
    {
        for (int i = 0; i < droneShopUIs.Count; i++)
        {
            var droneShopUI = droneShopUIs[i];
            droneShopUI.gameObject.SetActive(false);
            droneShopUI.index = i;
            var _droneAbilities = droneAbilityManager._droneAbilities;
            if (i >= _droneAbilities.Count)
            {
                continue;
            }
            droneShopUI.UpdateAbilityInfo(_droneAbilities[i]);
            droneShopUI.hoverButton.onClick.AddListener(() => UpdateAbilityInfo(_droneAbilities[droneShopUI.index]));
            if (droneShopUI._3DIcon.transform.childCount > 0)
            {
                GameObject lastObj = droneShopUI._3DIcon.transform.GetChild(0).gameObject;
                Destroy(lastObj);
            }
            var obj = Instantiate(droneControllerUI.uiObjects[i], droneShopUI._3DIcon.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(true);     
        }

        for (int i = 0; i < equipOptions.Count; i++)
        {
            var equipOption = equipOptions[i];
            equipOption.index = i;
            equipOption.hoverButton.onClick.RemoveAllListeners();
            equipOption.hoverButton.onClick.AddListener(() => SelectEquipSlot(equipOption.index));
            var savedData = PlayerSavedData.instance.droneLO[i];
            if (savedData == -2)
            {
                equipOption.UpdateEquipSlot(droneAbilityManager._droneAbilities[0], true);
                continue;
            }
            if (savedData == -1)
            {
                equipOption.UpdateEquipSlot(null, false);
                continue;
            }
            if (savedData >= 0)
            {
                equipOption.UpdateEquipSlot(droneAbilityManager._droneAbilities[savedData], false);
            }

            if (equipOption._3DIcon.transform.childCount > 0)
            {
                GameObject lastObj = equipOption._3DIcon.transform.GetChild(0).gameObject;
                Destroy(lastObj);
            }
            var obj = Instantiate(droneControllerUI.uiObjects[savedData], equipOption._3DIcon.transform);
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(true);
        }
    }

    public void OpenShopMenu()
    {
        Init();
        shopMenu.SetActive(true);
        fade.FadeIn();
        BaseManager.instance.statsUI.gameObject.SetActive(true);
    }

    public void CloseShopMenu()
    {
        fade.FadeOut();
        inputUpgrade.action.performed -= PurchaseAbility;
        inputUpgrade.action.Disable();
        BaseManager.instance.statsUI.gameObject.SetActive(false);
        PlayerSavedData.instance.SavePlayerData();
        Invoke("SetMenuInactive", 0.5f);
    }

    private void SetMenuInactive()
    {
        shopMenu.SetActive(false);
    }

    public void UpdateAbilityInfo(DroneAbility ability, bool force = false)
    {
        currentSelectedOption = droneShopUIs[ability.droneAbilityID];
        currentSelectedEquipSlot = null;
        if (currentSelectedAbility != null && !force)
        {
            return;
        }
        abilityNameText.text = ability.abilityName;
        abilityDescriptionText.text = ability.abilityDescription + "\n" + ability.abilityChargeDescription;
        videoPlayer.clip = videoClips[ability.droneAbilityID];
        currentSelectedOption = droneShopUIs[ability.droneAbilityID];
        currentSelectedOption.hoverButton.Select();

        if (ability.unlocked)
        {
            cost.SetActive(false);
            artifactCost.SetActive(false);
            abilityPriceText.text = "";
            buttonText.text = "Equip";
            return;
        }
        cost.SetActive(true);
        artifactCost.SetActive(false);
        abilityPriceText.text = ability.cost.ToString("N0");
        buttonText.text = "Purchase";

    }

    public void UpdateEquipInfo()
    {
        if (currentSelectedEquipSlot == null || currentSelectedAbility != null)
        {
            return;
        }
        int savedData = PlayerSavedData.instance.droneLO[currentSelectedEquipSlot.index];
        if (savedData == -2)
        {
            abilityNameText.text = "Unlock New Slot";
            abilityDescriptionText.text = "Upgrade your drone to aquire another slot, enabling you to bring more abilities into battle.";
            abilityPriceText.text = GetUnlockedCost().ToString();
            videoPlayer.Stop();
            cost.SetActive(false);
            artifactCost.SetActive(true);
            buttonText.text = "Unlock";
        }
        else if (savedData == -1)
        {
            abilityNameText.text = "Empty Slot";
            abilityDescriptionText.text = "Equip an ability into this slot.";
            abilityPriceText.text = "";
            videoPlayer.Stop();
            cost.SetActive(false);
            artifactCost.SetActive(false);
            buttonText.text = "Unlock";
        }
        else
        {
            var ability = droneAbilityManager._droneAbilities[savedData];
            abilityNameText.text = ability.abilityName;
            abilityDescriptionText.text = ability.abilityDescription + "\n" + ability.abilityChargeDescription;
            videoPlayer.clip = videoClips[ability.droneAbilityID];
            cost.SetActive(false);
            artifactCost.SetActive(false);
            buttonText.text = "Equip";
            abilityPriceText.text = "";
        }
    }

    public void PurchaseAbility(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        Purchase();
    }

    public void PurchaseAbilityButton()
    {
        Purchase();
    }

    private void Purchase()
    {
        if (currentSelectedEquipSlot != null && currentSelectedEquipSlot.lockIcon.activeSelf)
        {
            PurchaseEquip();
            return;
        }
        if (currentSelectedAbility != null && currentSelectedEquipSlot != null)
        {
            if (currentSelectedEquipSlot.lockIcon.activeSelf)
            {
                return;
            }
            Equip();
            return;
        }
        if (currentSelectedOption != null)
        {
            var ability = droneAbilityManager._droneAbilities[currentSelectedOption.index];
            if (ability.unlocked)
            {
                if(currentSelectedAbility != null)
                {
                    HighlightEquipSlots(false);
                }

                currentSelectedAbility = ability;
                UpdateAbilityInfo(currentSelectedAbility, true);
                HighlightEquipSlots(true);
                return;
            }
            if (PlayerSavedData.instance._Cash >= ability.cost)
            {
                BaseManager.instance.statsUI.RemoveCash(ability.cost);
                AudioManager.instance.PlaySFX(SFX.Unlock);
                ability.unlocked = true;
                currentSelectedOption.UpdateAbilityInfo(ability);
                currentSelectedOption.lockIcon.SetActive(false);
                currentSelectedOption.iconObjectParent.SetActive(true);
                PlayerSavedData.instance._droneAb[currentSelectedOption.index] = 0;
                currentSelectedAbility = ability;
                UpdateAbilityInfo(currentSelectedAbility, true);
            }
            else
            {
                // Show not enough cash message
                AudioManager.instance.PlaySFX(SFX.Error);
            }
        }

    }

    private void PurchaseEquip()
    {
        if(currentSelectedAbility != null)
        {
            return;
        }
        if (currentSelectedEquipSlot == null)
        {
            return;
        }
        int unlockCost = GetUnlockedCost();
        if (PlayerSavedData.instance._Artifact >= unlockCost)
        {
            BaseManager.instance.statsUI.RemoveArtifact(unlockCost);
            AudioManager.instance.PlaySFX(SFX.Unlock);
            currentSelectedEquipSlot.lockIcon.SetActive(false);
            currentSelectedEquipSlot.iconObjectParent.SetActive(true);
            PlayerSavedData.instance.droneLO[currentSelectedEquipSlot.index] = -1;
            SetupButtons();
            currentSelectedEquipSlot = null;
        }
        else
        {
            // Show not enough artifact message
            AudioManager.instance.PlaySFX(SFX.Error);
        }
    }

    private int GetUnlockedCost()
    {
        int unlockCost = 0;
        for (int i = 0; i < equipOptions.Count; i++)
        {
            var equipOption = equipOptions[i];
            if (!equipOption.lockIcon.activeSelf)
            {
                unlockCost ++;
            }
        }
        return unlockCost;
    }

    public void SelectEquipSlot(int _index)
    {
        currentSelectedOption = null;
        currentSelectedEquipSlot = equipOptions[_index];
        UpdateEquipInfo();
        if(InputTracker.instance != null)
        {
            if(InputTracker.instance.usingMouse)
            {
                if (currentSelectedAbility != null && currentSelectedEquipSlot != null)
                {
                    Purchase();
                }
            }
        }
    }

    public void EquipSlot(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        Equip();
    }

    private void Equip()
    {
        if (currentSelectedAbility != null && currentSelectedEquipSlot != null)
        {
            HighlightEquipSlots(false);
            currentSelectedEquipSlot.UpdateEquipSlot(currentSelectedAbility, false);
            PlayerSavedData.instance.droneLO[currentSelectedEquipSlot.index] = currentSelectedAbility.droneAbilityID;
            currentSelectedAbility = null;
            SetupButtons();
        }
    }

    public void HighlightEquipSlots(bool on)
    {
        for (int i = 0; i < equipOptions.Count; i++)
        {
            var equipOption = equipOptions[i];
            var colors = equipOption.hoverButton.colors;
            Color color = equipOption.lockIcon.activeSelf ? Color.white * 0.3f : Color.white * 0.7f;
            if (!on)
            {
                color = Color.white * 0.3f;
            }
            colors.normalColor = color;
            equipOption.hoverButton.colors = colors;
        }
        if (currentSelectedAbility == null)
        {
            return;
        }
        var _colors = droneShopUIs[currentSelectedAbility.droneAbilityID].hoverButton.colors;
        Color _color = on ? Color.white * 0.9f : Color.white * 0.3f;
        _colors.normalColor = _color;
        droneShopUIs[currentSelectedAbility.droneAbilityID].hoverButton.colors = _colors;
    }

}
