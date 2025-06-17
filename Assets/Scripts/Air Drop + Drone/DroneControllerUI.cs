using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using FORGE3D;
using UnityEngine.EventSystems;

public enum DroneType
{
    BurstStrike,
    Repair,
    BombingRun,
    Guided,
    Napalm,
    Mines,
    LittleBoy,
    Companion,
    Orbital,
}


public class DroneControllerUI : MonoBehaviour
{
    public DroneSystem drone;
    public AirDropCharger airDropTimer;
    public GameObject airdropMenu;
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
    private bool isMenuOpen;

    public bool testingFreeUse;

    private float menuopenforTime;

    public DroneType currentDroneType;

    private void Start()
    {
        gameUI = GameUI.instance;
        timesUsed = 0;
        SetupSequencers();
        CloseMenu();
    }

    void Update()
    {
        if(!isMenuOpen)
        {
            return;
        }
        menuopenforTime -= Time.unscaledDeltaTime;
        if (menuopenforTime <= 0f && isMenuOpen)
        {
            CloseMenu();
            menuopenforTime = 5f;
        }
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
            GameObject uiObject = uiObjects[i];
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

        if(!airDropTimer.charged && !testingFreeUse)
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

        
        if(isMenuOpen)
        {
            CloseMenu();
            return;
        }
        isMenuOpen = true;
        menuopenforTime = 5f;

        SetupSequencers();

        AudioManager.instance.PlaySFX(SFX.Select);
        Time.timeScale = 0.3f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        airdropMenu.SetActive(true);
        foreach (DroneInputUI sequence in droneInputs)
        {
            if (!sequence.isActive)
            {
                continue;
            }
            sequence.sequenceInputController.StartNewSequence();
        }
        //playerInput.SwitchCurrentActionMap("UI");
    }

    public void OnCloseMenu(InputAction.CallbackContext context)
    {
        if (!context.performed)
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
        CloseMenu();
    }

    public void CloseMenu()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F ;
        airdropMenu.SetActive(false);
        isMenuOpen = false;
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

        int droneAbilityCharges = 0;

        if (!testingFreeUse)
        {
            droneAbilityCharges = airDropTimer.charges - 1;
            DroneAbility droneAbility = drone.droneAbilityManager._droneAbilities[(int)type];

            if (drone.GetChargeInt(droneAbility, droneAbilityCharges) == 0)
            {
                droneAbilityCharges--;
                if (droneAbilityCharges <= 0)
                {
                    print("Cannot use drone ability");
                    return;
                }
                if (drone.GetChargeInt(droneAbility, droneAbilityCharges) == 0)
                {
                    droneAbilityCharges--;
                    if (droneAbilityCharges <= 0)
                    {
                        print("Cannot use drone ability");
                        return;
                    }
                }
            }
        }
        else
        {
            droneAbilityCharges = 2;
        }   


        currentDroneType = type;
        StartCoroutine(InputDelay());
        drone.UseDroneAbility(type, droneAbilityCharges + 1);

        if (type == DroneType.Repair)
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

    public void MissileStrike(Ordanance ordanance)
    {
        missileLauncher.LaunchMissiles(missileAmount,ordanance);
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

    public void LittleBoyLaunch()
    {
        missileLauncher.LaunchNuke();
    }


    public void FullyChargeDrone()
    {
        airDropTimer.ActivateButton(true);
    }

    public void ChargeDroneOnHit(float value)
    {
        airDropTimer.ChargeDrone(value);
    }

    public bool CanUseDrone()
    {
        return airDropTimer.charges > 0 || testingFreeUse;
    }
}
