using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ObjectSpawner objectSpawner;
    public GameUI gameUI;
    public int killCount;
    public PlayerInput playerInput;
    public MechLoadOut mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;
    public AltWeaponController altWeaponController;

    private void Awake()
    {
        // Create a singleton instance
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        DelayedStart();
    }

    public void DelayedStart()
    {
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
        mechLoadOut.Init();
        objectSpawner.isActive = true;
        AudioManager.instance.PlayMusic(1);
        killCount = 0;
    }

    public void UpdateKillCount(int count)
    {
        killCount += count;
        gameUI.UpdateKillCount(killCount);
        if(killCount > PlayerSavedData.instance._killCount)
        {
            PlayerSavedData.instance._killCount = killCount;
        }
    }

    public void SwapPlayerInput(string inputMap)
    {
        playerInput.SwitchCurrentActionMap(inputMap);
    }

    public void GameOver()
    {
        objectSpawner.isActive = false;
        gameUI.ShowGameOverPanel();
        SwapPlayerInput("UI");
    }

    public void LoadMainMenu()
    {
        altWeaponController.ClearWeaponInputs();
        PlayerSavedData.instance.SavePlayerData();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}