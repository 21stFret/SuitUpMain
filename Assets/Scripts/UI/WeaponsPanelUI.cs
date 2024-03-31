using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsPanelUI : MonoBehaviour
{
    public WeaponsManager weaponsManager;

    public Image mainImage;
    public Image altImage;

    public Sprite[] mainWeaponSprites;
    public Sprite[] altWeaponSprites;
    public int currentMainIndex;
    public int currentAltIndex;

    public void OnEnable()
    {
        weaponsManager = WeaponsManager.instance;
        currentMainIndex = weaponsManager.mainWeapon;
        currentAltIndex = weaponsManager.altWeapon;
        ChangeWeaponImage();
    }

    private void ChangeWeaponImage()
    {
        if (currentMainIndex >= 0) { mainImage.sprite = mainWeaponSprites[currentMainIndex]; }

        if (currentAltIndex >= 0) { altImage.sprite = altWeaponSprites[currentAltIndex]; }

    }

    public void NextMainWeapon()
    {
        currentMainIndex++;
        if (currentMainIndex >= weaponsManager._mainWeapons.Length)
        {
            currentMainIndex = 0;
        }
        weaponsManager.SetMainWeaponIndex(currentMainIndex);
        ChangeWeaponImage();
    }

    public void NextAltWeapon()
    {
        currentAltIndex++;
        if (currentAltIndex >= weaponsManager._altWeapons.Length)
        {
            currentAltIndex = 0;
        }
        weaponsManager.SetAltWeaponIndex(currentAltIndex);
        ChangeWeaponImage();
    }

}
