using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject menu;
    public GameObject cheatsMenu;
    public GameObject controlsMenu;
    public GameObject settingsMenu;
    public bool isPaused;
    public PlayerInput playerInput;
    public EventSystem eventSystem;
    public GameObject firstSelectedButton;
    public bool menuLocked;

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
        if(menuLocked)
        {
            return;
        }
        GameUI.instance.CloseAll();
        pauseMenu.SetActive(true);
        menu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        playerInput.SwitchCurrentActionMap("UI");
        eventSystem.SetSelectedGameObject(firstSelectedButton);
        MechBattleController.instance.characterController.runAudio.Stop();
    }

    public void ResumeGame()
    {
        cheatsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        menu.SetActive(false);
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        playerInput.SwitchCurrentActionMap("Gameplay");
    }



    public void QuitGame()
    {
        Application.Quit();
    }

}
