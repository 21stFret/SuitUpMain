using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadOutPanel : MonoBehaviour
{
    public WeaponsManager weaponsManager;
    public WeaponsHanger weaponsHanger;
    public MechLoader mechLoader;
    public GameObject mainWeapon;
    public TMP_Text mainWeaponName;
    public GameObject altWeapon;
    public TMP_Text altWeaponName;
    public GameObject armor;
    public TMP_Text armorName;
    public int currentMainIndex;
    public int currentAltIndex;
    public int currentArmorIndex;

    public TMP_Text DifficultyButtonText;
    public TMP_Text DifficultyDecription;
    public Image DifficultyButtonImage;
    public Sprite[] DifficultyButtonSprites;
    public int currentDifficulty;
    public int maxDifficulty;
    public bool playLocked;
    public GameObject firstSelectedButton;

    public void OnEnable()
    {
        int difficulty = PlayerSavedData.instance.highestDifficulty;
        currentDifficulty = difficulty-1;
        ScrollDifficulties(true);
        weaponsManager = WeaponsManager.instance;
        currentMainIndex = weaponsManager.mainWeapon;
        currentAltIndex = weaponsManager.altWeapon;
        ChangeWeaponImage();
        mainWeaponName.text = weaponsManager._assaultWeapons[currentMainIndex].baseWeaponInfo.weaponName;
        altWeaponName.text = weaponsManager._techWeapons[currentAltIndex].baseWeaponInfo.weaponName;
        //armorName.text = "Armor " + currentArmorIndex;
        armorName.text = "Coming soon!";
        InputTracker.instance.eventSystem.SetSelectedGameObject(firstSelectedButton);
        BattleMech.instance.myCharacterController.ToggleCanMove(false);
    }

    public void CloseMenu()
    {
        mechLoader.Init();
        gameObject.SetActive(false);
        BattleMech.instance.myCharacterController.ToggleCanMove(true);
        PlayerSavedData.instance.SavePlayerData();
    }

    private void ChangeWeaponImage()
    {
        weaponsHanger.SetWeaponToLoadoutMenu(weaponsManager._assaultWeapons[currentMainIndex], mainWeapon.transform);
        weaponsHanger.SetWeaponToLoadoutMenu(weaponsManager._techWeapons[currentAltIndex], altWeapon.transform);
    }

    public void NextMainWeapon()
    {
        AudioManager.instance.PlaySFX(SFX.Select);
        weaponsHanger.SetMainWeaponPositionToSlot(weaponsManager._assaultWeapons[currentMainIndex]);
        currentMainIndex++;
        if (currentMainIndex >= weaponsManager._assaultWeapons.Length)
        {
            currentMainIndex = 0;
        }
        if (!weaponsManager._assaultWeapons[currentMainIndex].weaponData.unlocked)
        {
            NextMainWeapon();
            return;
        }
        weaponsManager.SetMainWeaponIndex(currentMainIndex);
        mainWeaponName.text = weaponsManager._assaultWeapons[currentMainIndex].baseWeaponInfo.weaponName;
        ChangeWeaponImage();
    }

    public void NextAltWeapon()
    {
        AudioManager.instance.PlaySFX(SFX.Select);
        weaponsHanger.SetAltWeaponPositionToSlot(weaponsManager._techWeapons[currentAltIndex]);
        currentAltIndex++;
        if (currentAltIndex >= weaponsManager._techWeapons.Length)
        {
            currentAltIndex = 0;
        }
        if (!weaponsManager._techWeapons[currentAltIndex].weaponData.unlocked)
        {
            NextAltWeapon();
            return;
        }
        weaponsManager.SetAltWeaponIndex(currentAltIndex);
        altWeaponName.text = weaponsManager._techWeapons[currentAltIndex].baseWeaponInfo.weaponName;
        ChangeWeaponImage();
    }

    public void NextArmor()
    {
        currentArmorIndex++;
        if (currentArmorIndex >= 3)
        {
            currentArmorIndex = 0;
        }
        ChangeWeaponImage();
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


        DifficultyButtonImage.sprite = DifficultyButtonSprites[currentDifficulty];

        if(PlayerSavedData.instance.highestDifficulty < currentDifficulty)
        {
            DifficultyButtonText.text = "Locked";
            DifficultyDecription.text = "Complete the previous difficulty to unlock!";
            playLocked = true;
            return;
        }
        playLocked = false;

        SetupGame.instance.diffiulty = (Difficulty)currentDifficulty;

        switch (currentDifficulty)
        {
            case 0:
                DifficultyButtonText.text = "Easy";
                DifficultyDecription.text = "A small invasion force of weak bugs.\r\nGo get em!";
                break;
            case 1:
                DifficultyButtonText.text = "Normal";
                DifficultyDecription.text = "A sizeable horde of a mix of bugs. \r\nGood luck out there!";
                break;
            case 2:
                DifficultyButtonText.text = "Hard";
                DifficultyDecription.text = "A challenging horde of every bug. \r\n Make it back alive!";
                break;
            case 3:
                DifficultyButtonText.text = "Insane";
                DifficultyDecription.text = "A terrifying horde of elite bugs. \r\n Unleash chaos!";
                break;
        }
    }
}
