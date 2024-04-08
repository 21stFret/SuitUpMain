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

    public void OnOpenMenu()
    {
        if(!airDropTimer.activated)
        {
            return;
        }
        airdropMenu.SetActive(true);
        UpdatePrice();
        playerInput.SwitchCurrentActionMap("UI");
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    public void OnCloseMenu()
    {
        airdropMenu.SetActive(false);
        playerInput.SwitchCurrentActionMap("Gameplay");
    }

    public void InitDrone(int type)
    {
        if(PlayerSavedData.instance._Cash < airDropCost)
        {
            fade.PlayTween();
            return;
        }
        CashCollector.Instance.AddCash(-airDropCost);
        crate.crateType = (CrateType)type;
        drone.Init();
        airDropTimer.ResetAirDrop();
        OnCloseMenu();
    }

    public void MissileStrike()
    {
        if (PlayerSavedData.instance._Cash < airDropCost)
        {
            fade.PlayTween();
            return;
        }
        missileLauncher.LaunchMissiles(5);
        CashCollector.Instance.AddCash(-airDropCost);
        airDropTimer.ResetAirDrop();
        OnCloseMenu();
    }

    private void UpdatePrice()
    {
        foreach(var text in texts)
        {
            text.text = "$" + airDropCost.ToString();
        }
    }
}
