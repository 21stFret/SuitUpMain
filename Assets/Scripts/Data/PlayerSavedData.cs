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
    public float _BGMV             {get; private set;} 
    public float _SFXV             {get; private set;} 
    public WeaponData[] _mwData {get; private set;} 
    public WeaponData[] _awData  {get; private set;} 
    public int _Cash                    {get; private set;} 
    public byte _Artifact                {get; private set;} 
    public Vector2 _loadout       {get; private set;} 
    public byte _fplay              {get; private set;}
    public bool EastE;
    public bool tyPanel = false;
    public GameStats _stats         {get; private set;}
    public byte topDif;
    public bool demo;
    public int[] _droneAb;
    public int[] droneLO;
    public bool circuitTut = false;

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
        _Artifact += (byte)amount;
    }

    public bool HasCompletedEasyMode()
    {
        return topDif>0;
    }

    public void UpdateMainWeaponData(WeaponData weaponData, int index)
    {
        _mwData[index] = weaponData;
    }

    public void UpdateAltWeaponData(WeaponData weaponData, int index)
    {
        _awData[index] = weaponData;
    }

    public void UpdateMainWeaponLoadout(int mainWeapon)
    {
        Vector2 loadout = new Vector2(mainWeapon, _loadout.y);
        _loadout = loadout;
    }

    public void UpdateAltWeaponLoadout(int altWeapon)
    {
        Vector2 loadout = new Vector2(_loadout.x, altWeapon);
        _loadout = loadout;
    }

    public void UpdateFirstLoad(byte firstLoad)
    {
        _fplay = firstLoad;
    }

    public void UpdateBGMVolume(float volume)
    {
        _BGMV = volume;
    }

    public void UpdateSFXVolume(float volume)
    {
        _SFXV = volume;
    }

    public void UpdateDroneLoadout(int[] loadout)
    {
        droneLO = loadout;
    }

    public void ResetAllData()
    {
        CreateData();
        LogManager.instance.ClearLogs();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void CreateData()
    {
        _fplay = 0;
        _BGMV = 0.5f;
        _SFXV = 0.5f;
        _Cash = 0;
        _Artifact = 0;
        EastE = false;
        topDif = 0;
        tyPanel = false;
        circuitTut = false;
        CreateWeaponData();
        CreateDroneAbilityData();
        _stats = new GameStats();
        SavePlayerData();
    }

    public void CreateWeaponData()
    {
        _mwData = new WeaponData[4];
        for (int i = 0; i < _mwData.Length; i++)
        {
            WeaponData weaponData = new WeaponData();
            _mwData[i] = weaponData;
            _mwData[i].weaponIndex = (byte)i;
            _mwData[i].unlocked = 0;
            _mwData[i].level = 0;
            _mwData[i].mainWeapon = 1;
        }
        _awData = new WeaponData[4];
        for (int i = 0; i < _awData.Length; i++)
        {
            WeaponData weaponData = new WeaponData();
            _awData[i] = weaponData;
            _awData[i].weaponIndex = (byte)i;
            _awData[i].unlocked = 0;
            _awData[i].level = 0;
            _awData[i].mainWeapon = 0;
        }
        _mwData[0].unlocked = 1;
        _awData[0].unlocked = 1;
        _loadout = new Vector2(0, 0);
    }

    public void PatchWeaponData( WeaponData[] mainWeaponData, WeaponData[] altWeaponData)
    {
        // Ensure the main weapon data is initialized
        if (mainWeaponData.Length != 4)
        {
            print("Patching main weapon data");
            var oldMainWeaponData = mainWeaponData;
            var newMainWeaponData = new WeaponData[4];
            for (int i = 0; i < newMainWeaponData.Length; i++)
            {
                if (i < oldMainWeaponData.Length)
                {
                    newMainWeaponData[i] = oldMainWeaponData[i];
                }
                else
                {
                    newMainWeaponData[i] = new WeaponData(); // Initialize new entries
                    newMainWeaponData[i].weaponIndex = (byte)i;
                    newMainWeaponData[i].mainWeapon = 1; // Ensure main weapon flag is set
                    newMainWeaponData[i].unlocked = (i == 0) ? (byte)1 : (byte)0; // Only the first weapon is unlocked by default
                    newMainWeaponData[i].level = 0; // Reset level for new entries
                }
            }
            _mwData = newMainWeaponData;
        }
        // Ensure the alt weapon data is initialized
        if (altWeaponData.Length != 4)
        {
            print("Patching alt weapon data");
            var oldAltWeaponData = altWeaponData;
            var newAltWeaponData = new WeaponData[4];
            for (int i = 0; i < newAltWeaponData.Length; i++)
            {
                if (i < oldAltWeaponData.Length)
                {
                    newAltWeaponData[i] = oldAltWeaponData[i];
                }
                else
                {
                    newAltWeaponData[i] = new WeaponData(); // Initialize new entries
                    newAltWeaponData[i].weaponIndex = (byte)i;
                    newAltWeaponData[i].mainWeapon = 0; // Ensure main weapon flag is set
                    newAltWeaponData[i].unlocked = (i == 0) ? (byte)1 : (byte)0; // Only the first weapon is unlocked by default
                    newAltWeaponData[i].level = 0; // Reset level for new entries
                }
            }
            _awData = newAltWeaponData;
        }
    }

    public void CreateDroneAbilityData()
    {
        _droneAb = new int[15];
        for (int i = 0; i < _droneAb.Length; i++)
        {
            _droneAb[i] = -1; // -1 means locked
        }
        _droneAb[0] = 0; // 0 means unlocked
        _droneAb[1] = 0;

        droneLO = new int[5];
        for (int i = 0; i < droneLO.Length; i++)
        {
            droneLO[i] = -2;
        }
        droneLO[0] = 0;
        droneLO[1] = 1;
    }

    public void SavePlayerData()
    {
        SaveData saveData = new SaveData();
        // Assign the values from the PlayerSavedData instance to the SaveData instance
        saveData.BGMVolume = _BGMV;
        saveData.SFXVolume = _SFXV;
        saveData.cash = _Cash;
        saveData.arti = _Artifact;
        saveData.mwData = _mwData;
        saveData.awData = _awData;
        saveData.loadout = _loadout;
        saveData.fplay = _fplay;
        saveData.stats = _stats;
        saveData.eastE = EastE;
        saveData.topDif = topDif;
        saveData.tyPanel = tyPanel;
        saveData.droneAb = _droneAb;
        saveData.cirTut = circuitTut;
        saveData.droneLO = droneLO;

        string jsonData = JsonUtility.ToJson(saveData, false);
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
                _BGMV = savedData.BGMVolume;
                _SFXV = savedData.SFXVolume;
                _Cash = savedData.cash;
                _Artifact = savedData.arti;
                if (savedData.mwData.Length == 0 || savedData.awData.Length == 0)
                {
                    CreateWeaponData(); // Ensure weapon data is initialized if missing
                    corrupted = true;
                }
                else
                {
                    _mwData = savedData.mwData;
                    _awData = savedData.awData;
                    _loadout = savedData.loadout;
                }
                PatchWeaponData(savedData.mwData, savedData.awData);
                _fplay = savedData.fplay;
                _stats = savedData.stats;
                EastE = savedData.eastE;
                topDif = savedData.topDif;
                tyPanel = savedData.tyPanel;
                circuitTut = savedData.cirTut;
                if (savedData.droneAb.Length == 0)
                {
                    CreateDroneAbilityData(); // Ensure drone data is initialized if missing
                    corrupted = true;
                }
                else
                {
                    _droneAb = savedData.droneAb;
                    droneLO = savedData.droneLO;
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
    public int cash;
    public byte arti;
    public GameStats stats;
    public WeaponData[] mwData;
    public WeaponData[] awData;
    public Vector2 loadout;
    public byte fplay;
    public float BGMVolume;
    public float SFXVolume;
    public byte topDif;
    public bool eastE;
    public bool tyPanel;
    public int[] droneAb;
    public int[] droneLO;
    public bool cirTut;
}
