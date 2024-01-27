using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isPaused;
    public PlayerInput playerInput;
    public EventSystem eventSystem;
    public GameObject controlsButton;

    void Start()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    void Update()
    {
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        playerInput.SwitchCurrentActionMap("UI");
        eventSystem.SetSelectedGameObject(controlsButton);
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        playerInput.SwitchCurrentActionMap("Gameplay");
    }



    public void QuitGame()
    {
        Application.Quit();
    }

}
