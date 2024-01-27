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

    public TMP_Text killCountText;

    public GameObject gameOverPanel;
    public GameObject gameOverButton;
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

    public void UpdateKillCount(int killCount)
    {
        killCountText.text = killCount.ToString();
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(gameOverButton);
    }
}
