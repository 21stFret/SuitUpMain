using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Profiling;
using Steamworks;

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
    public bool triggeredEasterEgg;
    public bool hasSeenThankYouPanel = false;
    public GameStats _gameStats         {get; private set;}
    public int highestDifficulty;
    public bool demoBuild;
    public int[] _droneAbilities;
    public int[] droneLoadOut;
    public bool triggeredCircuitTutorial = false;

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

    public bool HasCompletedEasyMode()
    {
        return highestDifficulty>0;
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

    public void UpdateDroneLoadout(int[] loadout)
    {
        droneLoadOut = loadout;
    }

    public void ResetAllData()
    {
        CreateData();
        LogManager.instance.ClearLogs();
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
        triggeredEasterEgg = false;
        highestDifficulty = 0;
        hasSeenThankYouPanel = false;
        triggeredCircuitTutorial = false;
        CreateWeaponData();
        CreateDroneAbilityData();
        _gameStats = new GameStats();
        SavePlayerData();
    }

    public void CreateWeaponData()
    {
        _mainWeaponData = new WeaponData[3];
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
        _playerLoadout = new Vector2(0, 0);
    }

    public void CreateDroneAbilityData()
    {
        _droneAbilities = new int[15];
        for (int i = 0; i < _droneAbilities.Length; i++)
        {
            _droneAbilities[i] = -1; // -1 means locked
        }
        _droneAbilities[0] = 0; // 0 means unlocked
        _droneAbilities[1] = 0;

        droneLoadOut = new int[5];
        for (int i = 0; i < droneLoadOut.Length; i++)
        {
            droneLoadOut[i] = -2;
        }
        droneLoadOut[0] = 0;
        droneLoadOut[1] = 1;
    }

    public void SavePlayerData()
    {
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
        saveData.triggeredEasterEgg = triggeredEasterEgg;
        saveData.highestDifficulty = highestDifficulty;
        saveData.hasSeenThankYouPanel = hasSeenThankYouPanel;
        saveData.droneAbilities = _droneAbilities;
        saveData.triggeredCircuitTutorial = triggeredCircuitTutorial;
        saveData.droneLoadOut = droneLoadOut;

        string jsonData = JsonUtility.ToJson(saveData, true);
        byte[] byteData = Encoding.UTF8.GetBytes(jsonData); // Changed from ASCII to UTF8

        if (SteamManager.Initialized)
        {
            string cloudFileName = "saveData.sav";

            // Check if we have enough cloud storage
            if (SteamRemoteStorage.GetQuota(out ulong totalBytes, out ulong availableBytes))
            {
                if ((ulong)byteData.Length <= availableBytes)
                {
                    bool success = SteamRemoteStorage.FileWrite(cloudFileName, byteData, byteData.Length);
                    if (success)
                    {
                        Debug.Log($"Successfully saved to Steam Cloud: {cloudFileName} ({byteData.Length} bytes)");
                        return; // Only return if Steam save was successful
                    }
                }
                else
                {
                    Debug.LogWarning($"Not enough Steam Cloud storage. Need: {byteData.Length}, Available: {availableBytes}");
                }
            }

            Debug.LogWarning("Falling back to local save");
        }

        // Local save as fallback
        SaveLocalFile(jsonData);
    }

    private void SaveLocalFile(string jsonData)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, "saveData.sav");
        
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        
        File.WriteAllText(fullPath, jsonData);
        Debug.Log($"Saved locally to: {fullPath}");
    }

    public void LoadPlayerData()
    {
        string jsonData = null;
        bool loadedSuccessfully = false;

        if (SteamManager.Initialized)
        {
            string cloudFileName = "saveData.sav";
            
            if (SteamRemoteStorage.FileExists(cloudFileName))
            {
                int fileSize = SteamRemoteStorage.GetFileSize(cloudFileName);
                if (fileSize > 0)
                {
                    byte[] data = new byte[fileSize];
                    int bytesRead = SteamRemoteStorage.FileRead(cloudFileName, data, fileSize);
                    
                    if (bytesRead == fileSize)
                    {
                        try
                        {
                            jsonData = Encoding.UTF8.GetString(data);
                            // Validate JSON before marking as successful
                            JsonUtility.FromJson<SaveData>(jsonData);
                            loadedSuccessfully = true;
                            Debug.Log($"Successfully loaded {bytesRead} bytes from Steam Cloud");
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Error parsing Steam Cloud save: {e.Message}");
                        }
                    }
                }
            }
        }

        // Fall back to local file if Steam load failed or Steam isn't initialized
        if (!loadedSuccessfully)
        {
            string localPath = Path.Combine(Application.persistentDataPath, "saveData.sav");
            if (File.Exists(localPath))
            {
                jsonData = File.ReadAllText(localPath);
                loadedSuccessfully = true;
                Debug.Log("Successfully loaded from local save");
            }
        }

        if (loadedSuccessfully)
        {
            try
            {
                bool corrupted = false;
                SaveData savedData = JsonUtility.FromJson<SaveData>(jsonData);
                _BGMVolume = savedData.BGMVolume;
                _SFXVolume = savedData.SFXVolume;
                _playerLevel = savedData.playerLevel;
                _Cash = savedData.playerCash;
                _Exp = savedData.playerExp;
                _Artifact = savedData.playerArtifact;
                if (savedData.mainWeaponData.Length == 0 || savedData.altWeaponData.Length == 0)
                {
                    CreateWeaponData(); // Ensure weapon data is initialized if missing
                    corrupted = true;
                }
                else
                {
                    _mainWeaponData = savedData.mainWeaponData;
                    _altWeaponData = savedData.altWeaponData;
                    _playerLoadout = savedData.playerLoadout;
                }
                _firstLoad = savedData.firstLoad;
                _gameStats = savedData.gameStats;
                triggeredEasterEgg = savedData.triggeredEasterEgg;
                highestDifficulty = savedData.highestDifficulty;
                hasSeenThankYouPanel = savedData.hasSeenThankYouPanel;
                triggeredCircuitTutorial = savedData.triggeredCircuitTutorial;
                if (savedData.droneAbilities.Length == 0)
                {
                    CreateDroneAbilityData(); // Ensure drone data is initialized if missing
                    corrupted = true;
                }
                else
                {
                    _droneAbilities = savedData.droneAbilities;
                    droneLoadOut = savedData.droneLoadOut;
                }

                if (corrupted)
                {
                    SavePlayerData();
                }
                else
                {
                    Debug.Log("Save data loaded successfully");
                }


                Debug.Log("Save data parsed successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error parsing save data: {e.Message}");
                CreateData();
            }
        }
        else
        {
            Debug.Log("No save data found, creating new data");
            CreateData();
        }
    }
}

public class SaveData
{
    public int playerLevel;
    public int playerCash;
    public int playerExp;
    public int playerArtifact;
    public GameStats gameStats;
    public WeaponData[] mainWeaponData;
    public WeaponData[] altWeaponData;
    public Vector2 playerLoadout;
    public bool firstLoad;
    public float BGMVolume;
    public float SFXVolume;
    public int highestDifficulty;
    public bool triggeredEasterEgg;
    public bool hasSeenThankYouPanel;
    public int[] droneAbilities;
    public int[] droneLoadOut;
    public bool triggeredCircuitTutorial;
}
