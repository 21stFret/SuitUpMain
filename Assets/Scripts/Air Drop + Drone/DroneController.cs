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
    public int airDropCost;
    public AirDropTimer airDropTimer;
    public GameObject airdropMenu;
    public TMP_Text[] texts;
    public PlayerInput playerInput;
    public DoTweenFade fade;
    public EventSystem eventSystem;
    public GameObject firstSelected;
    public F3DMissileLauncher missileLauncher;
    private GameUI gameUI;

    private void Start()
    {
        gameUI = GameUI.instance;
    }

    public void OnOpenMenu(InputAction.CallbackContext context)
    {
        if(!context.performed)
        {
            return;
        }
        if(gameUI.pauseMenu.isPaused || gameUI.modOpen || !GameManager.instance.gameActive)
        {
            return;
        }
        if(!airDropTimer.activated)
        {
            return;
        }
        airdropMenu.SetActive(true);
        UpdatePrice();
        playerInput.SwitchCurrentActionMap("UI");
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    public void OnCloseMenu(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (GameUI.instance.pauseMenu.isPaused)
        {
            return;
        }
        CloseMenu();
    }

    private void CloseMenu()
    {
        airdropMenu.SetActive(false);
        playerInput.SwitchCurrentActionMap("Gameplay");
    }

    public void InitAirSupport(int type)
    {
        if (PlayerSavedData.instance._Cash < airDropCost)
        {
            fade.PlayTween();
            return;
        }
        switch (type)
        {
            case 0:
                InitDrone(0);
                break;
            case 1:
                InitDrone(1);
                break;
            case 2:
                InitDrone(2);
                break;
            case 3:
                MissileStrike();
                break;
        }
        CashCollector.Instance.AddCash(-airDropCost);
        airDropTimer.ResetAirDrop();
        CloseMenu();
    }

    public void InitDrone(int type)
    {
        crate.crateType = (CrateType)type;
        drone.Init();
    }

    public void MissileStrike()
    {
        missileLauncher.LaunchMissiles(5);
    }

    private void UpdatePrice()
    {
        foreach(var text in texts)
        {
            text.text = "$" + airDropCost.ToString();
        }
    }
}
