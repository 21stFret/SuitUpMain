using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Profiling;

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
        triggeredEasterEgg = false;
        highestDifficulty = 0;
        hasSeenThankYouPanel = false;
        CreateWeaponData();
        _playerLoadout = new Vector2(0, 0);
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
        saveData.triggeredEasterEgg = triggeredEasterEgg;
        saveData.highestDifficulty = highestDifficulty;
        saveData.hasSeenThankYouPanel = hasSeenThankYouPanel;

        // Convert the SaveData instance to JSON
        string jsonData = JsonUtility.ToJson(saveData, true);

        byte[] byteData;

        byteData = Encoding.ASCII.GetBytes(jsonData);

        string dataPath = Application.persistentDataPath.ToString();
        string dataFileName = "saveData.sav";
        string fullPath = Path.Combine(dataPath, dataFileName);

        // create the file in the path if it doesn't exist
        // if the file path or name does not exist, return the default SO
        if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        }

        // attempt to save here data
        try
        {
            // save datahere
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(jsonData);
                }
            }
            //print("Saved Data Complete" + jsonData);
            //print("Saved Data Path" + fullPath);
        }
        catch (Exception e)
        {
            // write out error here
            Debug.LogError("Failed to save data to: " + fullPath);
            Debug.LogError("Error " + e.Message);
            return;
        }


    }

    public void LoadPlayerData()
    {
        string dataPath = Application.persistentDataPath.ToString();
        string dataFileName = "saveData.sav";
        string fullPath = Path.Combine(dataPath, dataFileName);

        if (File.Exists(fullPath))
        {
            try
            {
                // load the serialized data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // deserialize the data from Json back into the C# object
                SaveData savedData = JsonUtility.FromJson<SaveData>(dataToLoad);
                _BGMVolume = savedData.BGMVolume;
                _SFXVolume = savedData.SFXVolume;
                _playerLevel = savedData.playerLevel;
                _Cash = savedData.playerCash;
                _Exp = savedData.playerExp;
                _Artifact = savedData.playerArtifact;
                _mainWeaponData = savedData.mainWeaponData;
                _altWeaponData = savedData.altWeaponData;
                _playerLoadout = savedData.playerLoadout;
                _firstLoad = savedData.firstLoad;
                _gameStats = savedData.gameStats;
                triggeredEasterEgg = savedData.triggeredEasterEgg;
                highestDifficulty = savedData.highestDifficulty;
                hasSeenThankYouPanel = savedData.hasSeenThankYouPanel;

                print("Loaded Data Complete" + dataToLoad);
            }
            catch (Exception e)
            {

                Debug.LogError("Error occured when trying to load file at path: "
                    + fullPath + " and backup did not work.\n" + e);

            }
        }
        else
        {
            print("No data found to load, creating data");
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
}
