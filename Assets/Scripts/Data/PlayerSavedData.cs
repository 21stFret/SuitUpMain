using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSavedData : MonoBehaviour
{
    public static PlayerSavedData instance;
    public float _BGMVolume             {get; private set;} 
    public float _SFXVolume             {get; private set;} 
    public int _playerLevel             {get; private set;} 
    public WeaponData[] _mainWeaponData {get; private set;} 
    public WeaponData[] _altWeaponData  {get; private set;} 
    public int _Cash                    {get; private set;} 
    public int _Exp                     {get; private set;} 
    public int _Artifact                {get; private set;} 
    public Vector2 _playerLoadout       {get; private set;} 
    public bool _firstLoad              {get; private set;} 
    public GameStats _gameStats         {get; private set;} 

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

    public void UpdatePlayerCash(int amount)
    {
        _Cash += amount;
    }

    public void UpdatePlayerArtifact(int amount)
    {
        _Artifact += amount;
    }

    public void UpdatePlayerLevel(int level)
    {
        _playerLevel = level;
    }

    public void UpdatePlayerExp(int exp)
    {
        _Exp += exp;
        CheckLevel();
    }

    private void CheckLevel()
    {
        var requiredExp = 100 + (20* _playerLevel);
        if(_Exp >= requiredExp)
        {
            _playerLevel++;
        }
    }

    public void UpdateMainWeaponData(WeaponData weaponData, int index)
    {
        _mainWeaponData[index] = weaponData;
    }

    public void UpdateAltWeaponData(WeaponData weaponData, int index)
    {
        _altWeaponData[index] = weaponData;
    }

    public void UpdateMainWeaponLoadout(int mainWeapon)
    {
        Vector2 loadout = new Vector2(mainWeapon, _playerLoadout.y);
        _playerLoadout = loadout;
    }

    public void UpdateAltWeaponLoadout(int altWeapon)
    {
        Vector2 loadout = new Vector2(_playerLoadout.x, altWeapon);
        _playerLoadout = loadout;
    }

    public void UpdateFirstLoad(bool firstLoad)
    {
        _firstLoad = firstLoad;
    }

    public void UpdateBGMVolume(float volume)
    {
        _BGMVolume = volume;
    }

    public void UpdateSFXVolume(float volume)
    {
        _SFXVolume = volume;
    }

    public void ResetAllData()
    {
        CreateData();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void CreateData()
    {
        _firstLoad = true;
        _BGMVolume = 0.5f;
        _SFXVolume = 0.5f;
        _playerLevel = 0;
        _Cash = 0;
        _Exp = 0;
        _Artifact = 0;
        CreateWeaponData();
        _playerLoadout = new Vector2(0, 0);
        _gameStats = new GameStats();
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
            _mainWeaponData[i].mainWeapon = true;
        }
        _altWeaponData = new WeaponData[3];
        for (int i = 0; i < _altWeaponData.Length; i++)
        {
            WeaponData weaponData = new WeaponData();
            _altWeaponData[i] = weaponData;
            _altWeaponData[i].weaponIndex = i;
            _altWeaponData[i].unlocked = false;
            _altWeaponData[i].level = 0;
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
        saveData.playerCash = _Cash;
        saveData.playerExp = _Exp;
        saveData.playerArtifact = _Artifact;
        saveData.mainWeaponData = _mainWeaponData;
        saveData.altWeaponData = _altWeaponData;
        saveData.playerLoadout = _playerLoadout;
        saveData.firstLoad = _firstLoad;
        saveData.gameStats = _gameStats;

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
            _Cash = saveData.playerCash;
            _Exp = saveData.playerExp;
            _Artifact = saveData.playerArtifact;
            _mainWeaponData = saveData.mainWeaponData;
            _altWeaponData = saveData.altWeaponData;
            _playerLoadout = saveData.playerLoadout;
            _firstLoad = saveData.firstLoad;
            _gameStats = saveData.gameStats;

            print("Loaded Data Complete" + jsonData);
        }
        else
        {
            print("No data found to load");
            CreateData();
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
    public int playerExp;
    public int playerArtifact;
    public Vector2 playerLoadout;
    public bool firstLoad;
    public GameStats gameStats;
}
