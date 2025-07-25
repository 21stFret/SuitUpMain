using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitMainMenu : MonoBehaviour
{
    public MechLoader mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;
    public MainMenu mainMenu;
    // Start is called before the first frame update
    void Start()
    {
        PlayerSavedData.instance.LoadPlayerData();
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mwData, PlayerSavedData.instance._awData);
        mechLoadOut.Init();
        print("Init Main Menu");
    }
}
