using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using FORGE3D;
using UnityEngine.EventSystems;

public class DroneController : MonoBehaviour
{
    public AirDropDrone drone;
    public AirDropCrate crate;
    public AirDropCharger airDropTimer;
    public GameObject airdropMenu;
    public TMP_Text[] texts;
    public PlayerInput playerInput;
    public DoTweenFade fade;
    public EventSystem eventSystem;
    public GameObject firstSelected;
    public F3DMissileLauncher missileLauncher;
    public int missileAmount;
    private GameUI gameUI;
    public int timesUsed;
    private bool inputDelay;
    public int airstikes;
    public bool tutorial;

    private void Start()
    {
        gameUI = GameUI.instance;
        timesUsed =0;
    }

    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if(!context.performed)
        {
            return;
        }
        if(!tutorial)
        {
            if (gameUI.pauseMenu.isPaused || gameUI.modOpen || !GameManager.instance.gameActive)
            {
                return;
            }
        }
        if(!airDropTimer.charged)
        {
            return;
        }
        Time.timeScale = 0.3f;
        airdropMenu.SetActive(true);
        playerInput.SwitchCurrentActionMap("UI");
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    public void OnCloseMenu(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (gameUI.pauseMenu.isPaused || gameUI.modOpen || !GameManager.instance.gameActive)
        {
            return;
        }
        CloseMenu();
    }

    public void CloseMenu()
    {
        Time.timeScale = 1;
        airdropMenu.SetActive(false);
        playerInput.SwitchCurrentActionMap("Gameplay");
    }

    public void InitAirSupport(int type)
    {
        if(inputDelay)
        {
            return;
        }

        StartCoroutine(InputDelay());
        switch (type)
        {
            case 0:
                InitDrone(0);
                break;
            case 1:
                InitDrone(1);
                break;
            case 3:
                MissileStrike();
                break;
        }

        timesUsed++;
        airDropTimer.ResetAirDrop();
        CloseMenu();
    }

    private IEnumerator InputDelay()
    {
        inputDelay = true;
        yield return new WaitForSeconds(1f);
        inputDelay = false;
    }

    private void InitDrone(int type)
    {
        crate.transform.position = drone.transform.position;
        crate.crateType = (CrateType)type;
        drone.Init();
    }

    public void MissileStrike()
    {
        missileLauncher.LaunchMissiles(missileAmount);
        airstikes++;
        if(airstikes == 2)
        {
            PlayerAchievements.instance.SetAchievement("AIRSTRIKE_2");
        }
        if (airstikes == 5)
        {
            PlayerAchievements.instance.SetAchievement("AIRSTRIKE_5");
        }
    }

    public void FullyChargeDrone()
    {
        airDropTimer.DroneCharge = airDropTimer.DroneMaxCharge;
    }

    public void ChargeDroneOnHit(float value)
    {
        airDropTimer.DroneCharge += value;
    }

}
