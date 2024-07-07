using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitGame : MonoBehaviour
{
    public MechLoader mechLoadOut;
    public StatsUI statsUI;
    public ConnectWeaponHolderToManager weaponHolder;
    public MainMenu mainMenu;
    public bool MainMenu;

    private void Start()
    {
        DelayedStart();
    }

    private void DelayedStart()
    {
        PlayerSavedData.instance.LoadPlayerData();

        if (!MainMenu)
        {
            Time.timeScale = 1;
            AudioManager.instance.Init();
            return;
        }

        print("Init Main Menu");
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
        mechLoadOut.Init();
        statsUI.UpdateCash(PlayerSavedData.instance._Cash);
        statsUI.UpdateArtifact(PlayerSavedData.instance._Artifact);
        if(SetupGame.instance.inGame)
        {
            mainMenu.OpenInLoadout();
        }
        SetupGame.instance.inGame = true;
    }
}
