using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSavedData : MonoBehaviour
{
    public static PlayerSavedData instance;
    public float _BGMVolume;
    public float _SFXVolume;
    public int _playerLevel;
    public WeaponData[] _mainWeaponData;
    public WeaponData[] _altWeaponData;
    public int _playerCash;
    public int _killCount;
    public int _waveScore;
    public Vector2 playerLoadout;
    public bool _firstLoad;

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

    private void Update()
    {
        // Testing Only
        /*
        if(Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ResetAllData();
        }
        */
    }

    public void UpdateKillCount(int count)
    {
        _killCount += count;
        if (_killCount > _waveScore)
        {
            _waveScore = _killCount;
            PlayerPrefs.SetInt("HighScore", _waveScore);
        }
    }

    public void UpdatePlayerCash(int amount)
    {
        _playerCash += amount;
    }

    public void UpdatePlayerLevel(int level)
    {
        _playerLevel = level;
    }

    public void UpdateMainWeaponData(WeaponData weaponData, int index)
    {
        _mainWeaponData[index] = weaponData;
    }

    public void UpdateAltWeaponData(WeaponData weaponData, int index)
    {
        _altWeaponData[index] = weaponData;
    }

    public void UpdatePlayerLoadout(Vector2 loadout)
    {
        playerLoadout = loadout;
    }

    public void ResetAllData()
    {
        _firstLoad = true;
        _BGMVolume = 0.5f;
        _SFXVolume = 0.5f;
        _playerLevel = 0;
        _playerCash = 0;
        _killCount = 0;
        _waveScore = 0;
        CreateWeaponData();
        playerLoadout = new Vector2(0, 0);
        SavePlayerData();
    }

    public void CreateWeaponData()
    {
        _mainWeaponData = new WeaponData[2];
        for (int i = 0; i < _mainWeaponData.Length; i++)
        {
            WeaponData weaponData = new WeaponData();
            _mainWeaponData[i] = weaponData;
            _mainWeaponData[i].weaponIndex = i;
            _mainWeaponData[i].unlocked = false;
            _mainWeaponData[i].level = 0;
            _mainWeaponData[i].exp = 0;
            _mainWeaponData[i].mainWeapon = true;
        }
        _altWeaponData = new WeaponData[2];
        for (int i = 0; i < _altWeaponData.Length; i++)
        {
            WeaponData weaponData = new WeaponData();
            _altWeaponData[i] = weaponData;
            _altWeaponData[i].weaponIndex = i;
            _altWeaponData[i].unlocked = false;
            _altWeaponData[i].level = 0;
            _altWeaponData[i].exp = 0;
            _altWeaponData[i].mainWeapon = false;
        }
        _mainWeaponData[0].unlocked = true;
        _altWeaponData[0].unlocked = true;
    }

    public void SavePlayerData()
    {
        // Create a new instance of the SaveData class
        SaveData saveData = new SaveData();

        // Assign the values from the PlayerSavedData instance to the SaveData instance
        saveData.BGMVolume = _BGMVolume;
        saveData.SFXVolume = _SFXVolume;
        saveData.playerLevel = _playerLevel;
        saveData.playerCash = _playerCash;
        saveData.killCount = _killCount;
        saveData.highScore = _waveScore;
        saveData.mainWeaponData = _mainWeaponData;
        saveData.altWeaponData = _altWeaponData;
        saveData.playerLoadout = playerLoadout;
        saveData.firstLoad = _firstLoad;

        // Convert the SaveData instance to JSON
        string jsonData = JsonUtility.ToJson(saveData);

        // Save the JSON data to a file
        System.IO.File.WriteAllText("saveData.json", jsonData);

        print("Saved Data Complete" + jsonData);
    }

    public void LoadPlayerData()
    {
        // Check if the save file exists
        if (System.IO.File.Exists("saveData.json"))
        {
            // Read the JSON data from the file
            string jsonData = System.IO.File.ReadAllText("saveData.json");

            // Convert the JSON data to a SaveData instance
            SaveData saveData = JsonUtility.FromJson<SaveData>(jsonData);

            // Assign the values from the SaveData instance to the PlayerSavedData instance
            _BGMVolume = saveData.BGMVolume;
            _SFXVolume = saveData.SFXVolume;
            _playerLevel = saveData.playerLevel;
            _playerCash = saveData.playerCash;
            _killCount = saveData.killCount;
            _waveScore = saveData.highScore;
            _mainWeaponData = saveData.mainWeaponData;
            _altWeaponData = saveData.altWeaponData;
            playerLoadout = saveData.playerLoadout;
            _firstLoad = saveData.firstLoad;

            print("Loaded Data Complete" + jsonData);
        }
        else
        {
            print("No data found to load");
            ResetAllData();
        }
    }
}

public class SaveData
{
    public float BGMVolume;
    public float SFXVolume;
    public int playerLevel;
    public WeaponData[] mainWeaponData;
    public WeaponData[] altWeaponData;
    public int playerCash;
    public int killCount;
    public int highScore;
    public Vector2 playerLoadout;
    public bool firstLoad;
}
