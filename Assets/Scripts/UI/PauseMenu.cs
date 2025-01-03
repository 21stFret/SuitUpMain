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
    public GameObject controlsPC;
    public GameObject controlsGamePad;
    public GameObject settingsMenu;
    public bool isPaused;
    public PlayerInput playerInput;
    public EventSystem eventSystem;
    public GameObject firstSelectedButton;
    public GameObject controlsSelectedButton;
    public bool menuLocked;
    private bool menuOpen;
    public PauseModUI pauseModUI;

    void Start()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
        menuOpen = false;
    }

    void Update()
    {
    }

    public void PauseGame()
    {
        if(menuLocked || menuOpen)
        {
            return;
        }
        menuOpen = true;
        GameUI.instance.CloseAll();
        pauseMenu.SetActive(true);
        menu.SetActive(true);
        pauseModUI.ShowPauseMods();
        Time.timeScale = 0;
        isPaused = true;
        playerInput.SwitchCurrentActionMap("UI");
        eventSystem.SetSelectedGameObject(firstSelectedButton);
        BattleMech.instance.myCharacterController.runAudio.Stop();
    }

    public void ResumeGame()
    {
        if(!menuOpen)
        {
            return;
        }
        menuOpen = false;
        cheatsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        menu.SetActive(false);
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
        playerInput.SwitchCurrentActionMap("Gameplay");
    }

    public void SwapControlsMenu()
    {
        bool PC = InputTracker.instance.usingMouse;
        if (PC)
        {
            controlsGamePad.SetActive(false);
            controlsPC.SetActive(true);
        }
        else
        {
            controlsGamePad.SetActive(true);
            controlsPC.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
    }

}
