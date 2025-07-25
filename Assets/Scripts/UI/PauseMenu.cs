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
    public GameObject datalogsMenu;
    public GameObject settingsPopup;
    public GameObject quitMenu;
    public bool isPaused;
    public PlayerInput playerInput;
    public EventSystem eventSystem;
    public GameObject firstSelectedButton;
    public GameObject settingsSelectedButton;
    public GameObject logsSelectedButton;
    public GameObject quitSelectedButton;
    public GameObject modSelectedButton;
    public GameObject cheatsSelectedButton;
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
    public LogManager logManager;
    private bool hiddenMenu, hiddenCircuitMenu;
    private bool wasPreviouslyConnected = false;

    public bool canQuickOpen = false;

    private Callback<GameOverlayActivated_t> m_GameOverlayActivated;

    public SequenceInputController sequenceInputController;

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
        canQuickOpen = false;
        if(sequenceInputController != null)
        {
            sequenceInputController.OnSequenceComplete += () => OpenCheatsMenu(true);
        }

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
            if (_modUI.modUI.activeSelf)
            {
                hiddenMenu = true;
                _modUI.CloseModUI();
            }
            if (_modUI.circuitBoardPanel.activeSelf)
            {
                hiddenCircuitMenu = true;
                _modUI.CloseCircuitBoardPauseMenu();
            }
        }

        if (droneControllerUI != null)
        {
            if (droneControllerUI.airdropMenu.activeSelf)
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
        BattleMech.instance.myCharacterController.ToggleCanMove(false);
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

    private void OpenPauseMenu()
    {
        SetTitleText("paused");
        pauseMenu.SetActive(true);
        menu.SetActive(true);
        SetOnBackAction(ResumeGame);
        backImage.SetActive(true);
        SwapBackIcon();
        if (lastSelectedButton != null)
        {
            eventSystem.SetSelectedGameObject(lastSelectedButton);
        }
        else
        {
            eventSystem.SetSelectedGameObject(firstSelectedButton);
        }
        if(sequenceInputController != null)
        {
            sequenceInputController.LoadSetSequence();
        }

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
        SetTitleText("controls");
        SwapControlsMenu();
        controlsMenu.SetActive(value);
        menu.SetActive(false);
        backImage.SetActive(true);
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

    public void OpenDataLogMenu(bool value)
    {
        logManager.LoadUI();
        SetTitleText("data logs");
        AudioManager.instance.PlaySFX(SFX.Confirm);
        datalogsMenu.SetActive(value);
        menu.SetActive(false);
        backImage.SetActive(true);
        if (!value)
        {
            OpenPauseMenu();
            return;
        }
        lastSelectedButton = eventSystem.currentSelectedGameObject;
        SetOnBackAction(() => { OpenDataLogMenu(false); });
        eventSystem.SetSelectedGameObject(logsSelectedButton);
    }

    public void OpenCheatsMenu(bool value)
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        SetTitleText("cheats");
        cheatsMenu.SetActive(value);
        backImage.SetActive(true);
        menu.SetActive(false);
        if (!value)
        {
            OpenPauseMenu();
            return;
        }
        lastSelectedButton = eventSystem.currentSelectedGameObject;
        SetOnBackAction(() => { OpenCheatsMenu(false); });
        eventSystem.SetSelectedGameObject(cheatsSelectedButton);
    }

    public void ToggleModMenu(bool value)
    {
        AudioManager.instance.PlaySFX(SFX.Confirm);
        menu.SetActive(false);
        backImage.SetActive(true);
        if (value)
        {
            lastSelectedButton = eventSystem.currentSelectedGameObject;
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
        SetOnBackAction(() => { ToggleModMenu(false); });
    }

    public void QuickOpenDataLogMenu(InputAction.CallbackContext context)
    {
        if (context.performed && canQuickOpen)
        {
            logManager.dataLogPopUpUI.DisableLogPopUp();
            AudioManager.instance.PlaySFX(SFX.Confirm);
            PauseGame();
            OpenDataLogMenu(true);
            canQuickOpen = false;
        }
    }
    
    public void ResumGameInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        if (!menuOpen)
        {
            return;
        }
        AudioManager.instance.PlaySFX(SFX.Select);
        menuOpen = false;
        if (cheatsMenu != null)
        {
            cheatsMenu.SetActive(false);
        }
        pauseMenu.SetActive(false);
        menu.SetActive(false);
        settingsMenu.SetActive(false);
        backImage.SetActive(false);
        pauseModUI.HidePauseMods();
        if (datalogsMenu != null)
        {
            datalogsMenu.SetActive(false);
        }
        lastSelectedButton = null;
        onPressBack = null;
        Time.timeScale = 1;
        isPaused = false;
        if (hiddenMenu)
        {
            hiddenMenu = false;
            _modUI.SHowHiddenMenu();
            return;
        }
        if (hiddenCircuitMenu)
        {
            hiddenCircuitMenu = false;
            _modUI.ShowHiddenCircuitMenu();
            return;
        }
        playerInput.SwitchCurrentActionMap("Gameplay");
        BattleMech.instance.myCharacterController.ToggleCanMove(true);
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
        cheatsMenu?.SetActive(false);
        pauseMenu.SetActive(false);
        menu.SetActive(false);
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
