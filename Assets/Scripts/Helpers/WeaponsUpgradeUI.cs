using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class WeaponsUpgradeUI : MonoBehaviour
{
    public WeaponsManager weaponsManager;
    public WeaponsHanger weaponsHanger;
    public TMP_Text damage, speed, range, level, cost;
    public TMP_Text Udamage, Uspeed, Urange, Ulevel;
    public TMP_Text weaponName;
    public TMP_Text weaponDescription;
    private int index = 0;
    public MechWeapon currentWeapon;
    private int currentWeaponIndex;
    public bool isMainWeapon;
    public Transform weaponParent;

    public GameObject upgradeEffect;
    public AudioSource upgradeAudio;

    public TMP_Text mainWeaponButton;
    public TMP_Text altWeaponButton;
    public StatsUI statsUI;

    private bool lockUpgradebutton; 
    public GameObject lockedPanel;
    public GameObject cantAffordPanel;

    public TMP_Text UpgradeButtonText;
    public TMP_Text UnlockAmount;

    private int MaxLevel;

    private void OnEnable()
    {
        isMainWeapon = true;
        weaponsManager = WeaponsManager.instance;
        SetMainWeaponBool(true);
        MaxLevel = weaponsManager._mainWeapons[0].baseWeaponInfo._damage.Length;
    }

    public void SetMainWeaponBool(bool value)
    {
        isMainWeapon = value;
        index = 0;
        LoadWeapon(index);
        var color = mainWeaponButton.color;
        color.a = 0.2f;
        altWeaponButton.color = color;
        color.a = 1f;
        mainWeaponButton.color = color;
    }

    public void ToggleMainWeapon()
    {
        isMainWeapon = !isMainWeapon;
        index = 0;
        LoadWeapon(index);
        
        if(!isMainWeapon)
        {
            var color = mainWeaponButton.color;
            color.a = 0.2f;
            mainWeaponButton.color = color;
            color.a = 1f;
            altWeaponButton.color = color;
        }
        else
        {
            var color = mainWeaponButton.color;
            color.a = 0.2f;
            altWeaponButton.color = color;
            color.a = 1f;
            mainWeaponButton.color = color;
        }
    }

    public void LoadWeapon(int _index)
    {
        if(currentWeapon != null)
        {
            if (isMainWeapon)
            {
                weaponsHanger.SetMainWeaponPositionToSlot(currentWeapon);
            }
            else
            {
                weaponsHanger.SetAltWeaponPositionToSlot(currentWeapon);
            }
        }

        if (isMainWeapon)
        {
            currentWeapon = weaponsManager._mainWeapons[_index];
            UpdateUI(currentWeapon.baseWeaponInfo, currentWeapon.weaponData.level);
        }
        else
        {
            currentWeapon = weaponsManager._altWeapons[_index];
            UpdateUI(currentWeapon.baseWeaponInfo, currentWeapon.weaponData.level);
        }
        currentWeapon.transform.position = weaponParent.position;
        currentWeapon.transform.rotation = weaponParent.rotation;
        currentWeapon.transform.SetParent(weaponParent);
        currentWeapon.transform.localScale = new Vector3(60, 60, 60);
        currentWeaponIndex = _index;
        ShowLockedPanel(!currentWeapon.weaponData.unlocked);
    }

    public void ShowLockedPanel(bool value)
    {
        UnlockAmount.text = currentWeapon.baseWeaponInfo._unlockCost.ToString();
        lockedPanel.SetActive(value);
        if(value)
        {
            UpgradeButtonText.text = "UNLOCK";
        }
        else
        {
            UpgradeButtonText.text = "UPGRADE";
        }
    }

    public void NextWeapon()
    {
        index++;
        int maxIndex = isMainWeapon? weaponsManager._mainWeapons.Length : weaponsManager._altWeapons.Length;
        if (index > maxIndex - 1)
        {
            index = 0;
        }
        LoadWeapon(index);
    }

    public void PreviousWeapon()
    {
        index--;
        if (index < 0)
        {
            index = isMainWeapon ? weaponsManager._mainWeapons.Length - 1 : weaponsManager._altWeapons.Length - 1;
        }
        LoadWeapon(index);
    }

    public void UpgradeWeapon()
    {
        StartCoroutine(pauseInput());
        if (!currentWeapon.weaponData.unlocked)
        {
            if (CheckUnlockCost(currentWeapon.baseWeaponInfo._unlockCost))
            {
                statsUI.RemoveArtifact(currentWeapon.baseWeaponInfo._unlockCost);
                weaponsManager.UnlockWeapon(currentWeaponIndex, isMainWeapon);
                ShowLockedPanel(false);
            }
            return;
        }

        if(currentWeapon.weaponData.level == MaxLevel)
        {
            return;
        }
        
        if(!CheckCost(currentWeapon.baseWeaponInfo))
        {
            return;
        }
        if(lockUpgradebutton)
        {
            return;
        }
        lockUpgradebutton = true;
        statsUI.RemoveCash(currentWeapon.baseWeaponInfo._cost[currentWeapon.weaponData.level]);
        currentWeapon.weaponData.level++;
        PlayerSavedData.instance._gameStats.totalUpgrades++;
        if(PlayerSavedData.instance._gameStats.totalUpgrades == 1)
        {
            PlayerAchievements.instance.SetAchievement("UPGRADE_1");
        }
        if(currentWeapon.weaponData.level == MaxLevel)
        {
            PlayerAchievements.instance.SetAchievement("UPGRADE_MAX_1");
            CheckAllMaxed();
        }
        weaponsManager.UpdateWeaponData();
        StartCoroutine(PlayUpgradeEffect());
    }

    private void CheckAllMaxed()
    {
        int checkAmount = 0;
        int doubleCheck = 0;
        for (int i = 0; i < weaponsManager._mainWeapons.Length; i++)
        {
            doubleCheck++;
            if (weaponsManager._mainWeapons[i].weaponData.level == MaxLevel)
            {
                checkAmount++;
            }
        }
        for (int i = 0; i < weaponsManager._altWeapons.Length; i++)
        {
            doubleCheck++;
            if (weaponsManager._altWeapons[i].weaponData.level == MaxLevel)
            {
                checkAmount++;
            }
        }
        if (checkAmount == doubleCheck)
        {
            PlayerAchievements.instance.SetAchievement("UPGRADE_ALL_MAX_");
        }
    }

    private IEnumerator PlayUpgradeEffect()
    {
        currentWeapon.GetComponentInChildren<Animator>().SetTrigger("Upgrade");
        upgradeAudio.Play();
        upgradeEffect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        LoadWeapon(index);
        upgradeEffect.SetActive(false);
        lockUpgradebutton = false;
    }

    public bool CheckCost(BaseWeaponInfo info)
    {
        if (info._cost[currentWeapon.weaponData.level] > PlayerSavedData.instance._Cash)
        {
            cantAffordPanel.GetComponent<DoTweenFade>().PlayTween();
            print("Not enough cash");
            return false;
        }
        return true;
    }

    public bool CheckUnlockCost(int cost)
    {
        if (cost > PlayerSavedData.instance._Artifact)
        {
            cantAffordPanel.GetComponent<DoTweenFade>().PlayTween();
            print("Not enough tech");
            return false;
        }
        return true;
    }

    public void UpdateUI(BaseWeaponInfo info, int weaponLevel)
    {
        damage.text = info._damage[weaponLevel].ToString();
        speed.text = info._fireRate[weaponLevel].ToString();
        range.text = info._range[weaponLevel].ToString();
        weaponName.text = info.weaponName;
        weaponDescription.text = info.weaponDescription;
        level.text = (weaponLevel + 1).ToString();
        cost.text = "$"+info._cost[weaponLevel].ToString();
        if(weaponLevel +1 == info._damage.Length)
        {
            Udamage.text = " Max";
            Uspeed.text = " Max";
            Urange.text = " Max";
            Ulevel.text = " Max";
            cost.text = "Max";
            return;
        }
        Udamage.text = info._damage[weaponLevel + 1].ToString();
        Uspeed.text = info._fireRate[weaponLevel + 1].ToString();
        Urange.text = info._range[weaponLevel + 1].ToString();
        Ulevel.text = (weaponLevel + 2).ToString();
    }

    public void ShowUpgradeStats()
    {
        damage.text += " + 1";
    }

    public IEnumerator pauseInput()
    {
        MainMenu.instance.PauseInput(true);
        yield return new WaitForSeconds(1.5f);
        MainMenu.instance.PauseInput(false);
    }
}
