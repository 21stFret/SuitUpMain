using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadOutPanel : MonoBehaviour
{
    public WeaponsManager weaponsManager;

    public Image mainImage;
    public TMP_Text mainWeaponName;
    public Image altImage;
    public TMP_Text altWeaponName;
    public Image armorImage;
    public TMP_Text armorName;

    public Sprite[] armorSprites;
    public int currentMainIndex;
    public int currentAltIndex;
    public int currentArmorIndex;

    public TMP_Text DifficultyButtonText;
    public Image DifficultyButtonImage;
    public Sprite[] DifficultyButtonSprites;
    public int currentDifficulty;
    public int maxDifficulty = 2;

    public void OnEnable()
    {
        weaponsManager = WeaponsManager.instance;
        currentMainIndex = weaponsManager.mainWeapon;
        currentAltIndex = weaponsManager.altWeapon;
        ChangeWeaponImage();
        mainWeaponName.text = weaponsManager._mainWeapons[currentMainIndex].baseWeaponInfo.weaponName;
        altWeaponName.text = weaponsManager._altWeapons[currentAltIndex].baseWeaponInfo.weaponName;
        //armorName.text = "Armor " + currentArmorIndex;
        armorName.text = "Coming soon!";
    }

    private void ChangeWeaponImage()
    {
        mainImage.sprite = weaponsManager._mainWeapons[currentMainIndex].weaponSprite;
        altImage.sprite = weaponsManager._altWeapons[currentAltIndex].weaponSprite;
        armorImage.sprite = armorSprites[currentArmorIndex];
    }

    public void NextMainWeapon()
    {
        currentMainIndex++;
        if (currentMainIndex >= weaponsManager._mainWeapons.Length)
        {
            currentMainIndex = 0;
        }
        if (!weaponsManager._mainWeapons[currentMainIndex].weaponData.unlocked)
        {
            NextMainWeapon();
            return;
        }
        weaponsManager.SetMainWeaponIndex(currentMainIndex);
        mainWeaponName.text = weaponsManager._mainWeapons[currentMainIndex].baseWeaponInfo.weaponName;
        ChangeWeaponImage();
    }

    public void NextAltWeapon()
    {
        currentAltIndex++;
        if (currentAltIndex >= weaponsManager._altWeapons.Length)
        {
            currentAltIndex = 0;
        }
        if (!weaponsManager._altWeapons[currentAltIndex].weaponData.unlocked)
        {
            NextAltWeapon();
            return;
        }
        weaponsManager.SetAltWeaponIndex(currentAltIndex);
        altWeaponName.text = weaponsManager._altWeapons[currentAltIndex].baseWeaponInfo.weaponName;
        ChangeWeaponImage();
    }

    public void NextArmor()
    {
        currentArmorIndex++;
        if (currentArmorIndex >= armorSprites.Length)
        {
            currentArmorIndex = 0;
        }
        armorImage.sprite = armorSprites[currentArmorIndex];
        armorName.text = "Coming soon!";
    }

    public void ScrollDifficulties(bool up)
    {
        if(up)
        {
            currentDifficulty++;
            if (currentDifficulty > maxDifficulty)
            {
                currentDifficulty = 0;
            }
        }
        else
        {
            currentDifficulty--;
            if (currentDifficulty < 0)
            {
                currentDifficulty = maxDifficulty;
            }
        }

        SetupGame.instance.diffiulty = currentDifficulty;
        DifficultyButtonImage.sprite = DifficultyButtonSprites[currentDifficulty];

        switch (currentDifficulty)
        {
            case 0:
                DifficultyButtonText.text = "Easy";
                break;
            case 1:
                DifficultyButtonText.text = "Normal";
                break;
            case 2:
                DifficultyButtonText.text = "Hard";
                break;
        }
    }
}
