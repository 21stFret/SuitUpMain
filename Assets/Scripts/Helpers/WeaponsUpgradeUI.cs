using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponsUpgradeUI : MonoBehaviour
{
    public WeaponsManager weaponsManager;
    public WeaponsHanger weaponsHanger;
    public TMP_Text damage, speed, range, level, cost;
    public TMP_Text Udamage, Uspeed, Urange, Ulevel;
    public TMP_Text weaponName;
    private int index = 0;
    public MechWeapon currentWeapon;
    public bool isMainWeapon;
    public Transform weaponParent;

    public GameObject upgradeEffect;
    public AudioSource upgradeAudio;

    public Button mainWeaponButton;
    public Button altWeaponButton;
    public StatsUI statsUI;

    private bool lockUpgradebutton; 
    public GameObject lockedPanel;
    public GameObject cantAffordPanel;

    private void OnEnable()
    {
        isMainWeapon = true;
        weaponsManager = WeaponsManager.instance;
        LoadWeapon(0);
    }

    public void SetMainWeaponBool(bool value)
    {
        isMainWeapon = value;
        index = 0;
        LoadWeapon(index);
        if(value)
        {
            mainWeaponButton.targetGraphic.color = mainWeaponButton.colors.pressedColor;
            altWeaponButton.targetGraphic.color = altWeaponButton.colors.normalColor;
        }
        else
        {
            mainWeaponButton.targetGraphic.color = mainWeaponButton.colors.normalColor;
            altWeaponButton.targetGraphic.color = altWeaponButton.colors.pressedColor;
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


        if(currentWeapon.weaponData.unlocked)
        {
            ShowLockedPanel(true);
        }
    }

    public void ShowLockedPanel(bool value)
    {
        //lockedPanel.SetActive(value);
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
        if(currentWeapon.weaponData.level == 2)
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
        weaponsManager.UpdateWeaponData();
        StartCoroutine(PlayUpgradeEffect());
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
        if (info._cost[weaponsManager._altWeapons[index].weaponData.level] > PlayerSavedData.instance._playerCash)
        {
            cantAffordPanel.GetComponent<DoTweenFade>().PlayTween();
            print("Not enough cash");
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
        level.text = (weaponLevel + 1).ToString();
        cost.text = "$"+info._cost[weaponLevel].ToString();
        if(weaponLevel == 2)
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
}
