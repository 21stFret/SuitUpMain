using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroneOptionShopUI : MonoBehaviour
{
    public TMP_Text droneOptionText;
    public GameObject iconObjectParent;
    public int index;
    public Button hoverButton;
    public GameObject lockIcon;
    public GameObject _3DIcon;

    public void UpdateAbilityInfo(DroneAbility droneAbility)
    {
        gameObject.SetActive(true);
        droneOptionText.text = droneAbility.abilityName;
        lockIcon.SetActive(!droneAbility.unlocked);
        iconObjectParent.SetActive(droneAbility.unlocked);
        // Update other UI elements based on the selected ability
    }

    public void UpdateEquipSlot(DroneAbility droneAbility, bool isLocked)
    {
        gameObject.SetActive(true);
        if (droneAbility != null)
        {
            droneOptionText.text = isLocked ? "Locked" : droneAbility.abilityName;
        }
        else
        {
            droneOptionText.text = "Empty Slot";
        }
        lockIcon.SetActive(isLocked);
        iconObjectParent.SetActive(!isLocked);

        // Update other UI elements based on the selected equip slot
    }

}
