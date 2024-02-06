using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WeaponsUpgradeUI : MonoBehaviour
{
    public GameObject[] weapons;
    public TMP_Text damage, speed, range;
    public TMP_Text weaponName;

    public void UpdateUI(int index)
    {
        damage.text = weapons[index].GetComponent<MechWeapon>().damage.ToString();
        speed.text = weapons[index].GetComponent<MechWeapon>().speed.ToString();
        range.text = weapons[index].GetComponent<MechWeapon>().range.ToString();
        weaponName.text = weapons[index].GetComponent<MechWeapon>().weaponName;
    }

    public void ShowUpgradeStats()
    {
        damage.text += " + 1";
    }
}
