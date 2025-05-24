using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    public static WeaponsManager instance;
    public GameObject weaponsHolder;
    public MechWeapon[] _assaultWeapons;
    public MechWeapon[] _techWeapons;
    public int mainWeapon;
    public int altWeapon;
    private PlayerSavedData _playerSavedData;
    private WeaponBaseDataReader weaponBaseDataReader;
    public MechWeapon currentMainWeapon { get { return _assaultWeapons[mainWeapon]; } }
    public MechWeapon currentAltWeapon { get { return _techWeapons[altWeapon]; } }

    private void Awake()
    {
        // Create a singleton instance
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy the duplicate instance
            Destroy(gameObject);
        }
        weaponBaseDataReader = GetComponent<WeaponBaseDataReader>();
    }

    public void SetMainWeaponIndex(int index)
    {
        mainWeapon = index;
        _playerSavedData.UpdateMainWeaponLoadout(index);
    }

    public void SetAltWeaponIndex(int index)
    {
        altWeapon = index;
        _playerSavedData.UpdateAltWeaponLoadout(index);
    }

    public void LoadWeaponsData(WeaponData[] mainWeapons, WeaponData[] altWeapons)
    {
        _playerSavedData = PlayerSavedData.instance;
        for (int i = 0; i < mainWeapons.Length; i++)
        {
            _assaultWeapons[i].weaponData = mainWeapons[i];
        }
        for (int i = 0; i < altWeapons.Length; i++)
        {
            _techWeapons[i].weaponData = altWeapons[i];
        }

        mainWeapon = (int)_playerSavedData._playerLoadout.x;
        altWeapon = (int)_playerSavedData._playerLoadout.y;
        if (weaponBaseDataReader == null)
        {
            weaponBaseDataReader = GetComponent<WeaponBaseDataReader>();
        }
        if (weaponBaseDataReader != null)
        {
            LoadBaseData();
        }
    }

    private void LoadBaseData()
    {
        weaponBaseDataReader.LoadFromExcell(this);
    }

    public void UpdateWeaponData()
    {
        for (int i = 0; i < _assaultWeapons.Length; i++)
        {
            _playerSavedData.UpdateMainWeaponData(_assaultWeapons[i].weaponData, i);
        }
        for (int i = 0; i < _techWeapons.Length; i++)
        {
            _playerSavedData.UpdateAltWeaponData(_techWeapons[i].weaponData, i);
        }
        _playerSavedData.SavePlayerData();
    }

    public void GetWeaponsFromHolder(ConnectWeaponHolderToManager holder)
    {
        weaponsHolder = holder.gameObject;
        _assaultWeapons = holder.mainWeapons;
        _techWeapons = holder.altWeapons;
    }

    public void UnlockWeapon(int index, bool mainWeapon)
    {
        var weapon = mainWeapon ? _assaultWeapons[index] : _techWeapons[index];
        weapon.weaponData.unlocked = true;
        UpdateWeaponData();
    }
}

[System.Serializable]
public class WeaponData
{
    public int weaponIndex;
    public bool mainWeapon;
    public bool unlocked;
    public int level;
}