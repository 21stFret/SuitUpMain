using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public TMP_Text weaponFuelText;
    public Image fuelBar;
    public Image fuelImage;

    public void UpdateWeaponUI(float fuel)
    {
        weaponFuelText.text = fuel.ToString("0") + "%";
        fuelBar.fillAmount = fuel / 100;
    }

    public void SetFuelImage(Sprite sprite)
    {
        fuelImage.sprite = sprite;  
    }
}
