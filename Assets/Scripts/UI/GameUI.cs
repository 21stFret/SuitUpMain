using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class GameUI : MonoBehaviour
{
    public static GameUI instance;
    public PauseMenu pauseMenu;
    public DroneControllerUI droneController;
    public ModUI modUI;
    public bool modOpen;
    public TMP_Text killCountText;
    public GameObject completePanel;
    public GameObject completeButton;
    public TMP_Text gameEndText;
    public RewardMenu rewardMenu;
    public EventSystem eventSystem;
    public DoTweenFade gameUIFade;
    public ObjectiveUI objectiveUI;
    public GameObject blackout;
    public GameObject gameplayUI;

    private void Awake()
    {
        // Create a singleton instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        AudioManager.instance.eventSystem = eventSystem;
        killCountText.text = "0";
    }

    public void CloseAll()
    {
        if (droneController == null)
        {
            return;
        }
        droneController.airdropMenu.SetActive(false);
    }

    public void OpenModUI(ModBuildType pickType)
    {
        modUI.OpenModUI(pickType);
        modOpen = true;
    }

    public void CloseModUI()
    {
        modUI.CloseModUI();
        modOpen = false;
    }

    public void UpdateKillCount(int killCount)
    {
        killCountText.text = killCount.ToString();
    }

    public void ShowEndGamePanel(bool win)
    {
        if(win)
        {
            gameEndText.text = "Mission \n Complete";
        }
        else
        {
            gameEndText.text = "Mission \n Failed";
        }
        pauseMenu.menuLocked = true;
        completePanel.SetActive(true);
        gameplayUI.SetActive(false);
        blackout.SetActive(true);
        completePanel.GetComponentInParent<DoTweenFade>().PlayTween();
        eventSystem.SetSelectedGameObject(completeButton);
        var GM = PlayerProgressManager.instance;
        rewardMenu.SetRewards(GM.cashCount, GM.expCount, GM.artifactCount, GM.playTime, GM.rewardMultiplier);
    }
}
