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
    public DroneController droneController;
    public ModUI modUI;

    public TMP_Text killCountText;

    public GameObject gameOverPanel;
    public GameObject completePanel;
    public GameObject gameOverButton;
    public GameObject completeButton;
    public EventSystem eventSystem;

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
        droneController.airdropMenu.SetActive(false);
    }

    public void UpdateKillCount(int killCount)
    {
        killCountText.text = killCount.ToString();
    }

    public void ShowGameOverPanel()
    {
        pauseMenu.menuLocked = true;
        gameOverPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(gameOverButton);
    }

    public void ShowCompletePanel()
    {
        pauseMenu.menuLocked = true;
        completePanel.SetActive(true);
        eventSystem.SetSelectedGameObject(completeButton);
    }
}
