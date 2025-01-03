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
    public SequenceInputController[] sequenceInputController;

    [InspectorButton("FullyChargeDrone")]
    public bool chargeDrone;



    private void Start()
    {
        gameUI = GameUI.instance;
        timesUsed =0;
        SetupSequencers();
    }

    private void SetupSequencers()
    {
        for (int i = 0; i < sequenceInputController.Length; i++)
        {
            int currentIndex = i;  // Create a local copy of the index
            sequenceInputController[i].OnSequenceComplete += () => InitAirSupport(currentIndex);
        }
    }

    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if(!context.performed)
        {
            return;
        }

        if(!airDropTimer.charged)
        {
            return;
        }


        if (!tutorial)
        {
            if (gameUI.pauseMenu.isPaused || gameUI.modOpen || !GameManager.instance.gameActive)
            {
                return;
            }
        }

        AudioManager.instance.PlaySFX(SFX.Select);
        Time.timeScale = 0.3f;
        airdropMenu.SetActive(true);
        if(!tutorial)
        {
            InputTracker.instance.SetLastSelectedGameObject(firstSelected);
            foreach (SequenceInputController sequence in sequenceInputController)
            {
                sequence.StartNewSequence();
            }
        }
        playerInput.SwitchCurrentActionMap("UI");
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
                MissileStrike();
                break;
            case 3:

                break;
        }

        AudioManager.instance.PlaySFX(SFX.Confirm);

        timesUsed++;
        airDropTimer.ResetAirDrop();
        CloseMenu();
    }

    private IEnumerator InputDelay()
    {
        inputDelay = true;
        yield return new WaitForSeconds(2f);
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
        airDropTimer.ActivateButton(true);
    }

    public void ChargeDroneOnHit(float value)
    {
        if(airDropTimer.DroneCharge >= airDropTimer.DroneMaxCharge)
        {
            return;
        }
        airDropTimer.DroneCharge += value;
    }

    public bool CanUseDrone()
    {
        return airDropTimer.DroneCharge >= airDropTimer.DroneMaxCharge;
    }
}
