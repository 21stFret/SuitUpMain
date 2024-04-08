using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitGame : MonoBehaviour
{
    public MechLoadOut mechLoadOut;
    public StatsUI statsUI;
    public ConnectWeaponHolderToManager weaponHolder;

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
        print("Init Main Menu");
        Time.timeScale = 1;
        PlayerSavedData.instance.LoadPlayerData();
        if(PlayerSavedData.instance._firstLoad)
        {
            PlayerSavedData.instance.UpdateFirstLoad(false);
            // show welcome pop up
        }
        AudioManager.instance.Init();
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
        mechLoadOut.Init();
        statsUI.UpdateCash(PlayerSavedData.instance._Cash);
    }
}
