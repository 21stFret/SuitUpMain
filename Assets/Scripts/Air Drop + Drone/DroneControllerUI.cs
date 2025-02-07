using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using FORGE3D;
using UnityEngine.EventSystems;

public enum DroneType
{
    Repair,
    Airstrike,
    Orbital,
    ElementBomb,
    Shield,
    Companion
}


public class DroneControllerUI : MonoBehaviour
{
    public DroneSystem drone;
    public AirDropCrate crate;
    public AirDropCharger airDropTimer;
    public GameObject airdropMenu;
    public PlayerInput playerInput;
    public EventSystem eventSystem;
    public GameObject firstSelected;
    public F3DMissileLauncher missileLauncher;
    public int missileAmount;
    private GameUI gameUI;
    public int timesUsed;
    private bool inputDelay;
    public int airstikes;
    public int crates;
    public bool tutorial;
    public List<DroneInputUI> droneInputs;
    public List<GameObject> uiObjects;

    [InspectorButton("FullyChargeDrone")]
    public bool chargeDrone;

    [InspectorButton("TestActiveate")]
    public bool testActiveate;
    public DroneType testDrone;

    private void Start()
    {
        gameUI = GameUI.instance;
        timesUsed =0;
        SetupSequencers();
    }

    public void TestActiveate()
    {
        ActivateDroneInput(testDrone);
    }

    public void ActivateDroneInput(DroneType type)
    {
        DroneInputUI droneInput = null;
        foreach (DroneInputUI sequence in droneInputs)
        {
            if (!sequence.isActive)
            {
                droneInput = sequence;
                break;
            }
        }
        if (droneInput == null)
        {
            print("No more drone inputs available");
            return;
        }
        droneInput.droneType = type;
        droneInput.isActive = true;
    }

     private void SetupSequencers()
     {
        for (int i = 0; i < droneInputs.Count; i++)
        {
            DroneInputUI droneInput = droneInputs[i];
            droneInput.gameObject.SetActive(false);
            droneInput.UIObject.SetActive(false);
            if (!droneInput.isActive)
            {
                continue;
            }
            droneInput.gameObject.SetActive(true);
            GameObject uiObject = uiObjects[(int)droneInput.droneType];
            uiObject.transform.SetParent(droneInput.UIObject.transform);
            uiObject.transform.localPosition = Vector3.zero;
            uiObject.SetActive(true);
            droneInput.UIObject.SetActive(true);
            droneInput.droneAbilityName.text = droneInput.droneType.ToString();


            // Clear existing event handlers
            droneInput.sequenceInputController.RemoveAllListeners();
            droneInput.sequenceInputController.OnSequenceComplete += () => InitAirSupport(droneInput.droneType);
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
            if (gameUI.pauseMenu.isPaused || gameUI.modUI.modUI.activeSelf || !GameManager.instance.gameActive)
            {
                return;
            }
        }

        SetupSequencers();

        AudioManager.instance.PlaySFX(SFX.Select);
        Time.timeScale = 0.3f;
        airdropMenu.SetActive(true);
        foreach (DroneInputUI sequence in droneInputs)
        {
            if (!sequence.isActive)
            {
                continue;
            }
            sequence.sequenceInputController.StartNewSequence();
        }
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void OnCloseMenu(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (gameUI.pauseMenu.isPaused || gameUI.modUI.modUI.activeSelf || !GameManager.instance.gameActive)
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
        if(tutorial)
        {
            Invoke("FullyChargeDrone", 3f);
        }
    }

    public void InitAirSupport(DroneType type)
    {
        if(inputDelay)
        {
            return;
        }

        StartCoroutine(InputDelay());
        drone.Init(type);

        if(type == DroneType.Repair || type == DroneType.Shield)
        {
            crates++;
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

    public void MissileStrike()
    {
        missileLauncher.FatMan = false;
        missileLauncher.LaunchMissiles(missileAmount);
        airstikes++;

        if(PlayerAchievements.instance == null)
        {
            return;
        }
        if(airstikes == 2)
        {
            PlayerAchievements.instance.SetAchievement("AIRSTRIKE_2");
        }
        if (airstikes == 5)
        {
            PlayerAchievements.instance.SetAchievement("AIRSTRIKE_5");
        }
    }

    public void FatManLaunch()
    {
        missileLauncher.FatMan = true;
        missileLauncher.LaunchMissiles(1);
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
