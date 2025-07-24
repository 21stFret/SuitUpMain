using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class WeaponsUpgradeUI : MonoBehaviour
{
    public WeaponsManager weaponsManager;
    public WeaponsHanger weaponsHanger;
    public TMP_Text level, cost;
    public TMP_Text  Ulevel;
    public List<WeaponInfoUI> weaponInfoUIs;
    public TMP_Text uniqueText;

    public TMP_Text weaponName;
    public TMP_Text weaponDescription;
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
    public GameObject techIcon;

    public TMP_Text UpgradeButtonText;
    public TMP_Text UnlockAmount;

    public GameObject firstSlected;

    private int MaxLevel;
    public GameObject arrows;


    public InputActionReference inputUpgrade;

    private PlayerSavedData _playerSavedData;
    bool inputBlocked;

    public void Init()
    {
        _playerSavedData = PlayerSavedData.instance;
        isMainWeapon = true;
        weaponsManager = WeaponsManager.instance;
        ToggleMainWeapon(true);
        MaxLevel = weaponsManager._assaultWeapons[0].baseWeaponInfo._damage.Length - 1;
        InputTracker.instance.OnInputChange += UpdateUIafterInputSwap;
        inputUpgrade.action.Enable();
        inputUpgrade.action.performed += UpgradeWeapon;
        LoadWeapon(0);
    }

    public void ToggleMenuOpen(bool value)
    {
        weaponParent.gameObject.SetActive(value);
        techIcon.SetActive(value);
        if (value)
        {
            UpdateUIafterInputSwap();
        }
        if(!value)
        {
            inputUpgrade.action.performed -= UpgradeWeapon;
            inputUpgrade.action.Disable();
            StartCoroutine(pauseInput(2f, true));
        }
    }

    private void UpdateUIafterInputSwap()
    {
        if (InputTracker.instance.usingMouse)
        {
            return;
        }
        EventSystem.current.SetSelectedGameObject(firstSlected);
    }

    public void ToggleMainWeapon(bool value)
    {
        isMainWeapon = value;

        if (!isMainWeapon)
        {
            var color = mainWeaponButton.color;
            color.a = 0.2f;
            mainWeaponButton.color = color;
            var color2 = altWeaponButton.color;
            color2.a = 1f;
            altWeaponButton.color = color2;
        }
        else
        {
            var color = altWeaponButton.color;
            color.a = 0.2f;
            altWeaponButton.color = color;
            var color2 = mainWeaponButton.color;
            color2.a = 1f;
            mainWeaponButton.color = color2;
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
            currentWeapon = weaponsManager._assaultWeapons[_index];
            UpdateUI(currentWeapon.baseWeaponInfo, currentWeapon.weaponData.level);
        }
        else
        {
            currentWeapon = weaponsManager._techWeapons[_index];
            UpdateUI(currentWeapon.baseWeaponInfo, currentWeapon.weaponData.level);
        }
        if (currentWeapon.name == "Chainsaw")
        {
            currentWeapon.GetComponentInChildren<Animator>().SetBool("Enabled", false);
        }
        currentWeapon.transform.position = weaponParent.position;
        currentWeapon.transform.rotation = weaponParent.rotation;
        currentWeapon.transform.SetParent(weaponParent);
        currentWeapon.transform.localScale = new Vector3(60, 60, 60);
        currentWeaponIndex = _index;
        ShowLockedPanel(currentWeapon.weaponData.unlocked == 0);
    }

    public void ShowLockedPanel(bool value)
    {
        UnlockAmount.text = currentWeapon.baseWeaponInfo._unlockCost.ToString();
        lockedPanel.SetActive(value);
        if(value)
        {
            UpgradeButtonText.text = "UNLOCK";
            cost.text = "N/A";
        }
        else
        {
            UpgradeButtonText.text = "UPGRADE";
        }
    }

    public void UpgradeWeaponButton()
    {
        ActualUpgrade();
    }

    public void UpgradeWeapon(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            ActualUpgrade();
        }
    }

    private void ActualUpgrade()
    {
        if (inputBlocked)
        {
            return;
        }
        StartCoroutine(pauseInput(1.1f));
        if (currentWeapon.weaponData.unlocked == 0)
        {
            if(_playerSavedData.demo)
            {
                string _text = "Locked in Demo";
                cantAffordPanel.GetComponentInChildren<TMP_Text>().text = _text;
                cantAffordPanel.GetComponent<DoTweenFade>().PlayTween();    
                return;
            }
            
            if (CheckUnlockCost(currentWeapon.baseWeaponInfo._unlockCost))
            {
                statsUI.RemoveArtifact(currentWeapon.baseWeaponInfo._unlockCost);
                weaponsManager.UnlockWeapon(currentWeaponIndex, isMainWeapon);
                ShowLockedPanel(false);
                AudioManager.instance.PlaySFX(SFX.Unlock);
            }
        }
        else
        {
            if (currentWeapon.weaponData.level == MaxLevel)
            {
                return;
            }
            if (!CheckCost(currentWeapon))
            {
                return;
            }
            if (lockUpgradebutton)
            {
                return;
            }
            lockUpgradebutton = true;
            statsUI.RemoveCash(currentWeapon.baseWeaponInfo._cost[currentWeapon.weaponData.level]);
            currentWeapon.weaponData.level++;
            _playerSavedData._stats.totalUpgrades++;
            if (PlayerAchievements.instance != null)
            {
                if (_playerSavedData._stats.totalUpgrades == 1)
                {
                    PlayerAchievements.instance.SetAchievement("UPGRADE_1");
                }
                if (currentWeapon.weaponData.level == MaxLevel)
                {
                    PlayerAchievements.instance.SetAchievement("UPGRADE_MAX_1");
                    CheckAllMaxed();
                }
            }
        }
        weaponsManager.UpdateWeaponData();
        StartCoroutine(PlayUpgradeEffect());
    }

    private void CheckAllMaxed()
    {
        int checkAmount = 0;
        int doubleCheck = 0;
        for (int i = 0; i < weaponsManager._assaultWeapons.Length; i++)
        {
            doubleCheck++;
            if (weaponsManager._assaultWeapons[i].weaponData.level == MaxLevel)
            {
                checkAmount++;
            }
        }
        for (int i = 0; i < weaponsManager._techWeapons.Length; i++)
        {
            doubleCheck++;
            if (weaponsManager._techWeapons[i].weaponData.level == MaxLevel)
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
        UpdateUI(currentWeapon.baseWeaponInfo, currentWeapon.weaponData.level);
        currentWeapon.GetComponentInChildren<Animator>().SetTrigger("Upgrade");
        upgradeAudio.Play();
        upgradeEffect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        upgradeEffect.SetActive(false);
        lockUpgradebutton = false;
    }

    public bool CheckCost(MechWeapon currentWeapon)
    {
        BaseWeaponInfo info = currentWeapon.baseWeaponInfo;
        if (info._cost[currentWeapon.weaponData.level] > _playerSavedData._Cash)
        {
            string _text = "Not Enough Credits";
            cantAffordPanel.GetComponentInChildren<TMP_Text>().text = _text;
            cantAffordPanel.GetComponent<DoTweenFade>().PlayTween();
            print("Not enough cash");
            return false;
        }
        if(currentWeapon.weaponData.level ==4 && _playerSavedData.demo)
        {
            string _text = "Demo Build Max Level Reached";
            cantAffordPanel.GetComponentInChildren<TMP_Text>().text = _text;
            cantAffordPanel.GetComponent<DoTweenFade>().PlayTween();
            print("Demo Build Max level reached");
            return false;
        }
        return true;
    }

    public bool CheckUnlockCost(int cost)
    {
        if (cost > _playerSavedData._Artifact)
        {
            cantAffordPanel.GetComponent<DoTweenFade>().PlayTween();
            print("Not enough tech");
            return false;
        }
        return true;
    }

    public void UpdateUI(BaseWeaponInfo info, int weaponLevel)
    {

        var itemlist = new List<float>
        {
            info._damage[weaponLevel],
            info._fireRate[weaponLevel],
            info._range[weaponLevel],
            info._uniqueValue[weaponLevel]
        };


        var itemlistPlus = new List<float>
        {
            info._damage[weaponLevel],
            info._fireRate[weaponLevel],
            info._range[weaponLevel],
            info._uniqueValue[weaponLevel]
        };

        if (weaponLevel +1 < info._damage.Length)
        {
            itemlistPlus = new List<float>
            {
                info._damage[weaponLevel + 1],
                info._fireRate[weaponLevel + 1],
                info._range[weaponLevel + 1],
                info._uniqueValue[weaponLevel + 1]
            };
        }

        level.gameObject.SetActive(true);
        for (int i = 0; i < weaponInfoUIs.Count; i++)
        {
            if (weaponLevel + 1 >= info._damage.Length)
            {
                weaponInfoUIs[i].amount.text = "";
                weaponInfoUIs[i].boostedlevel.text = itemlist[i].ToString();
                cost.text = "Max";
                continue;
            }

            weaponInfoUIs[i].amount.text = itemlist[i].ToString();
            if (currentWeapon.weaponData.unlocked == 0)
            {
                weaponInfoUIs[i].boostedlevel.text = "";
                continue;
            }

            weaponInfoUIs[i].boostedlevel.text = itemlistPlus[i].ToString();

        }
         
        switch(info.weaponName)
        {
            case "Minigun":
                uniqueText.text = "N/A";
                break;
            case "Shotgun":
            uniqueText.text = "Knockback";
                break;
            case "Plasma":
                uniqueText.text = "Pierce";
                break;
            case "Flamer":
                uniqueText.text = "N/A";
                break;
            case "Shocker":
                uniqueText.text = "Chain Amount";
                break;
            case "Cryo":
                uniqueText.text = "Freeze Time";
                break;
        }

        weaponName.text = info.weaponName;
        weaponDescription.text = info.weaponDescription;
        Ulevel.gameObject.SetActive(true);
        if (weaponLevel == MaxLevel)
        {
            level.gameObject.SetActive(false);
            arrows.SetActive(false);
            level.text = "";
            cost.text = "N/A";
            Ulevel.text = "Max";
            return;
        }
        if(currentWeapon.weaponData.unlocked == 0)
        {
            level.gameObject.SetActive(true);
            arrows.SetActive(false);
            level.text = "1";
            cost.text = "$"+info._unlockCost.ToString();
            Ulevel.gameObject.SetActive(false);
            Ulevel.text = "";
            return;
        }
        level.gameObject.SetActive(true);
        arrows.SetActive(true);
        level.text = (weaponLevel + 1).ToString();
        cost.text = "$"+info._cost[weaponLevel].ToString();
        Ulevel.text = (weaponLevel + 2).ToString();
    }

    public IEnumerator pauseInput(float time, bool close = false)
    {
        inputBlocked = true;
        MainMenu.instance.PauseInput(true);
        yield return new WaitForSeconds(time);
        MainMenu.instance.PauseInput(false);
        if (close)
        {
            gameObject.SetActive(false);
        }
        inputBlocked = false;
    }
}
