using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Steamworks;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject menu;
    public GameObject cheatsMenu;
    public GameObject controlsMenu;
    public GameObject controlsPC;
    public GameObject controlsGamePad;
    public GameObject settingsMenu;
    public GameObject settingsPopup;
    public GameObject quitMenu;
    public bool isPaused;
    public PlayerInput playerInput;
    public EventSystem eventSystem;
    public GameObject firstSelectedButton;
    public GameObject settingsSelectedButton;
    public GameObject quitSelectedButton;
    public GameObject modSelectedButton;
    public bool menuLocked;
    private bool menuOpen;
    public PauseModUI pauseModUI;
    public TMP_Text titleText;
    public Action onPressBack;
    public GameObject backImage;
    public Sprite PcBack, gamepadBack;
    public GameObject lastSelectedButton;
    public ModUI _modUI;
    public DroneControllerUI droneControllerUI;
    private bool hiddenMenu;
    private bool wasPreviouslyConnected = false;

    private Callback<GameOverlayActivated_t> m_GameOverlayActivated;

    private void OnEnable()
    {
        if (SteamManager.Initialized)
        {
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnOverlayActivated);
        }
    }

    private void OnDisable()
    {
        m_GameOverlayActivated = null;
    }

    void Start()
    {
        pauseModUI.pauseMenu = this;
        pauseMenu.SetActive(false);
        isPaused = false;
        menuOpen = false;
        lastSelectedButton = null;
    }

    void Update()
    {
        CheckControllerConnection();
    }

    private void CheckControllerConnection()
    {
        bool isConnected = Gamepad.current != null;

        if (wasPreviouslyConnected && !isConnected)
        {
            PauseGame();
        }

        wasPreviouslyConnected = isConnected;
    }

    private void OnOverlayActivated(GameOverlayActivated_t callback)
    {
        bool isOverlayActive = callback.m_bActive == 1;
        
        if (isOverlayActive)
        {
            PauseGame();
            playerInput.DeactivateInput();
        }
        else
        {
            ResumeGame();
            playerInput.ActivateInput();
        }
    }

    public void PauseGameInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        PauseGame();
    }

    public void PauseGame()
    {
        if (menuLocked || menuOpen)
        {
            return;
        }
        OpenPauseMenu();
        if (_modUI != null)
        {
            if(_modUI.modUI.activeSelf)
            {
                hiddenMenu = true;
                _modUI.CloseModUI();
            }
        }
        if(droneControllerUI != null)
        {
            if(droneControllerUI.airdropMenu.activeSelf)
            {
                droneControllerUI.CloseMenu();
            }

        }
        menuOpen = true;
        GameUI.instance.CloseAll();
        pauseModUI.HidePauseMods();
        Time.timeScale = 0;
        isPaused = true;
        playerInput.SwitchCurrentActionMap("UI");
        BattleMech.instance.myCharacterController.runAudio.Stop();

    }

    public void InvokeOnBack(InputAction.CallbackContext context)
    {   
        if (context.performed)
        {
            if (onPressBack != null)
            {
                onPressBack.Invoke();
            }
        }
    }

    private void OpenPauseMenu()
    {
        SetTitleText("paused");
        pauseMenu.SetActive(true);
        menu.SetActive(true);
        SetOnBackAction(ResumeGame);
        backImage.SetActive(false);
        SwapBackIcon();
        if (lastSelectedButton != null)
        {
            eventSystem.SetSelectedGameObject(lastSelectedButton);
        }
        else
        {
            eventSystem.SetSelectedGameObject(firstSelectedButton);
        }
    }


    public void SetOnBackAction(Action action)
    {
        onPressBack = null;
        onPressBack += action;
    }

    public void SetTitleText(string title)
    {
        titleText.text = title;
    }

    public void SwapBackIcon()
    {
        backImage.GetComponent<Image>().sprite = InputTracker.instance.usingMouse ? PcBack : gamepadBack;
    }

    public void OpenSettingsMenu(bool value)
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        settingsMenu.SetActive(value);
        settingsPopup.SetActive(false);
        menu.SetActive(false);
        backImage.SetActive(true);
        if (!value)
        {
            OpenPauseMenu();
            return;
        }
        lastSelectedButton = eventSystem.currentSelectedGameObject;
        SetOnBackAction(() => { OpenSettingsMenu(false); });
        eventSystem.SetSelectedGameObject(settingsSelectedButton);
    }

    public void OpenControlsMenu(bool value)
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        controlsMenu.SetActive(value);
        SwapControlsMenu();
        backImage.SetActive(true);
        menu.SetActive(false);
        if (!value)
        {
            OpenPauseMenu();
            return;
        }
        lastSelectedButton = eventSystem.currentSelectedGameObject;
        SetOnBackAction(() => { OpenControlsMenu(false); });
    }

    public void OpenQuitMenu(bool value)
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        quitMenu.SetActive(value);
        backImage.SetActive(true);
        menu.SetActive(false);
        if (!value)
        {
            OpenPauseMenu();
            return;
        }
        lastSelectedButton = eventSystem.currentSelectedGameObject;
        SetOnBackAction(() => { OpenQuitMenu(false); });
        eventSystem.SetSelectedGameObject(quitSelectedButton);
    }

    public void OpenCheatsMenu()
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        cheatsMenu.SetActive(true);
        backImage.SetActive(true);
        menu.SetActive(false);
    }

    public void ToggleModMenu(bool value)
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        menu.SetActive(false);
        backImage.SetActive(true);
        if (value)
        {
            pauseModUI.ShowPauseMods();
        }
        else
        {
            ModEntry entry = eventSystem.currentSelectedGameObject.GetComponent<ModEntry>();
            if (entry != null)
            {
                entry.OnHoverExit();
            }
            pauseModUI.HidePauseMods();
            SetOnBackAction(ResumeGame);
            OpenPauseMenu();
            return;
        }
        lastSelectedButton = eventSystem.currentSelectedGameObject;
        SetOnBackAction(() => { ToggleModMenu(false); });
        eventSystem.SetSelectedGameObject(modSelectedButton);
        modSelectedButton.GetComponent<ModEntry>().OnHoverEnter();
    }

    public void ResumeGame()
    {
        if(!menuOpen)
        {
            return;
        }
        AudioManager.instance.PlaySFX(SFX.Select);
        menuOpen = false;
        cheatsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        menu.SetActive(false);
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        backImage.SetActive(false);
        pauseModUI.HidePauseMods();
        lastSelectedButton = null;
        onPressBack = null;
        Time.timeScale = 1;
        isPaused = false;
        if(hiddenMenu)
        {
            hiddenMenu = false;
            _modUI.SHowHiddenMenu();
            return;
        }
        playerInput.SwitchCurrentActionMap("Gameplay");
    }

    public void SwapControlsMenu()
    {
        AudioManager.instance.PlaySFX(SFX.Move);
        bool PC = InputTracker.instance.usingMouse;
        SwapBackIcon();
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
        AudioManager.instance.PlaySFX(SFX.Select);
        menuOpen = false;
        cheatsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        menu.SetActive(false);
        controlsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        backImage.SetActive(false);
        pauseModUI.HidePauseMods();
        lastSelectedButton = null;
        onPressBack = null;
        Time.timeScale = 1;
        isPaused = false;
        BattleMech.instance.mechHealth.Die();
    }

    public void ShowBackImage(bool value)
    {
        backImage.SetActive(value);
    }

}
