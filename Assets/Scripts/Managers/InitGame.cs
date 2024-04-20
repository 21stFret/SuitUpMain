using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitGame : MonoBehaviour
{
    public MechLoadOut mechLoadOut;
    public StatsUI statsUI;
    public ConnectWeaponHolderToManager weaponHolder;
    public bool MainMenu;

    private void OnEnable()
    {
        //Invoke("DelayedStart", 0.2f);
    }

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
            if (PlayerSavedData.instance._firstLoad)
            {
                PlayerSavedData.instance.UpdateFirstLoad(false);
                // show welcome pop up
            }
            AudioManager.instance.Init();
            return;
        }

        print("Init Main Menu");
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
        mechLoadOut.Init();
        statsUI.UpdateCash(PlayerSavedData.instance._Cash);
    }
}
