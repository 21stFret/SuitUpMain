using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsManager : MonoBehaviour
{
    public static WeaponsManager instance;
    public GameObject weaponsHolder;
    public MechWeapon[] _mainWeapons;
    public MechWeapon[] _altWeapons;
    public int mainWeapon;
    public int altWeapon;
    private PlayerSavedData _playerSavedData;

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

    }

    public void SetMainWeaponIndex(int index)
    {
        mainWeapon = index;
        _playerSavedData.UpdateMainWeaponLoadout( index);
        _playerSavedData.SavePlayerData();
    }

    public void SetAltWeaponIndex(int index)
    {
        altWeapon = index;
        _playerSavedData.UpdateAltWeaponLoadout(index);
        _playerSavedData.SavePlayerData();
    }

    public void LoadWeaponsData(WeaponData[] mainWeapons, WeaponData[] altWeapons)
    {
        _playerSavedData = PlayerSavedData.instance;
        for (int i = 0; i < mainWeapons.Length; i++)
        {
            _mainWeapons[i].weaponData = mainWeapons[i];
        }
        for (int i = 0; i < altWeapons.Length; i++)
        {
            _altWeapons[i].weaponData = altWeapons[i];
        }

        mainWeapon = (int)_playerSavedData._playerLoadout.x;
        altWeapon = (int)_playerSavedData._playerLoadout.y;
    }

    public void UpdateWeaponData()
    {
        for (int i = 0; i < _mainWeapons.Length; i++)
        {
            _playerSavedData.UpdateMainWeaponData(_mainWeapons[i].weaponData, i);
        }
        for (int i = 0; i < _altWeapons.Length; i++)
        {
            _playerSavedData.UpdateAltWeaponData(_altWeapons[i].weaponData, i);
        }
        _playerSavedData.SavePlayerData();
    }

    public void GetWeaponsFromHolder(ConnectWeaponHolderToManager holder)
    {
        weaponsHolder = holder.gameObject;
        _mainWeapons = holder.mainWeapons;
        _altWeapons = holder.altWeapons;
    }
    
    public void UnlockWeapon(int index, bool mainWeapon)
    {
        var weapon = mainWeapon ? _mainWeapons[index] : _altWeapons[index];
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


