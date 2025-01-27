using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    public static BaseManager instance; 
    public MechLoader mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;
    public StatsUI statsUI;
    public SceneLoader sceneLoader;
    public TestPlayerData playerData;

    private void Awake()
    {
        if (instance == null) instance = this;
        sceneLoader = SceneLoader.instance;
    }

    public void Start()
    {
        InitBaseArea();
    }

    public void InitBaseArea()
    {
        BattleMech.instance.myCharacterController.ToggleCanMove(true);
        BattleMech.instance.playerInput.SwitchCurrentActionMap("UI");
        BattleMech.instance.playerInput.SwitchCurrentActionMap("Gameplay");
        weaponHolder.SetupWeaponsManager();
        if(PlayerSavedData.instance._mainWeaponData == null)
        {
            playerData.InittestData();
        }
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
        mechLoadOut.Init();
        AudioManager.instance.Init();
        AudioManager.instance.PlayMusic(3);
        statsUI.UpdateCash(PlayerSavedData.instance._Cash);
        statsUI.UpdateArtifact(PlayerSavedData.instance._Artifact);

        Debug.Log("Base Initialized");
    }

    public void StartGame(int value)
    {
        Time.timeScale = 1;
        sceneLoader.LoadScene(value, true);
    }
}
