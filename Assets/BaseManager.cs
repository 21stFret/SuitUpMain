using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    public static BaseManager instance; 
    public MechLoader mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;

    private void Awake()
    {
        if (instance == null) instance = this;
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
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
        mechLoadOut.Init();
        AudioManager.instance.Init();
        AudioManager.instance.PlayMusic(3);


        Debug.Log("Base Initialized");
    }
}
