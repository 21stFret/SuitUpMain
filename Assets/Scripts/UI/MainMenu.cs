using DG.Tweening;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    public GameObject mainMenu;
    public GameObject playButton;
    public GameObject loadOutMenu;
    public GameObject RDMenu;
    public GameObject AchMenu;
    public PlayerInput playerInput;
    public MechLoader loadOut;
    public SceneLoader sceneLoader;
    public GameObject[] menus;
    public GameObject[] menuBUttons;
    public int currentMenuIndex;
    public VirtualCameraSwitcher virtualCameraSwitcher;
    public TMP_Text headerText;
    private bool inMainMenu;
    public GameObject header;

    private void Awake()
    {
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
        sceneLoader = SceneLoader.instance;
    }

    public void StartGame(int value)
    {
        if (PlayerSavedData.instance._firstLoad)
        {
            sceneLoader.LoadScene(3, true);
        }
        else
        {
            sceneLoader.LoadScene(value, true);
        }
    }

    public void ResetData()
    {
        PlayerSavedData.instance.ResetAllData();
    }

    #region New Way
    public void CycleUP(InputAction.CallbackContext context)
    {
        if(inMainMenu)
        {
            return;
        }
        if (context.performed)
        {
            StartCoroutine(OpenMenu(false));
        }

    }

    public void CycleUP()
    {
        StartCoroutine(OpenMenu(true));
    }    
    
    public void CycleDOwn()
    {
        StartCoroutine(OpenMenu(false));
    }

    public void CycleDOwn(InputAction.CallbackContext context)
    {
        if (inMainMenu)
        {
            return;
        }
        if (context.performed)
        {
            StartCoroutine(OpenMenu(true));
        }

    }

    public IEnumerator OpenMenu(bool up)
    {
        int menu = currentMenuIndex;
        if (up)
        {
            menu++;
            if (menu > menus.Length-1)
            {
                menu = 0;
            }
        }
        else
        {
            menu--;
            if (menu < 0)
            {
                menu = menus.Length-1;
            }
        }
        int camIndex = menu + 1;
        virtualCameraSwitcher.SwitchToVirtualCamera(camIndex);
        menus[currentMenuIndex].GetComponent<DoTweenFade>().FadeOut();
        PauseInput(true);
        ToggleText(false);
        yield return new WaitForSeconds(0.8f);
        if (menus[menu].name == "load out")
        {
            loadOut.Init();
        }
        menus[menu].SetActive(true);
        menus[menu].GetComponent<DoTweenFade>().FadeIn();
        menus[currentMenuIndex].SetActive(false);
        EventSystem.current.SetSelectedGameObject(menuBUttons[menu]);
        PauseInput(false);
        currentMenuIndex = menu;
        ToggleText(true);

    }

    private void ToggleText(bool on)
    {
        if(on)
        {
            headerText.text = menus[currentMenuIndex].name;
            headerText.DOFade(1, 0.5f);
        }
        else
        {
            headerText.DOFade(0, 0.5f);
        }
    }
    #endregion

    public void OpenInLoadout()
    {
        virtualCameraSwitcher.SwitchToVirtualCamera(1);
        currentMenuIndex = 0;
        ToggleText(true);
        PauseInput(true);
        header.SetActive(true);
        header.GetComponent<DoTweenFade>().FadeIn();
        loadOutMenu.GetComponent<DoTweenFade>().canvasGroup.alpha = 0;
        loadOut.Init();
        loadOutMenu.SetActive(true);
        loadOutMenu.GetComponent<DoTweenFade>().FadeIn();
        mainMenu.SetActive(false);
        PauseInput(false);
        inMainMenu = false;
        EventSystem.current.SetSelectedGameObject(menuBUttons[0]);
    }

    public IEnumerator OpenMainMenu()
    {
        virtualCameraSwitcher.SwitchToVirtualCamera(0);
        currentMenuIndex = 0;
        loadOutMenu.GetComponent<DoTweenFade>().FadeOut();
        header.GetComponent<DoTweenFade>().FadeOut();
        PauseInput(true);
        loadOut.Init();
        yield return new WaitForSeconds(0.8f);
        mainMenu.SetActive(true);
        mainMenu.GetComponent<DoTweenFade>().FadeIn();
        loadOutMenu.SetActive(false);
        RDMenu.SetActive(false);
        AchMenu.SetActive(false);
        header.SetActive(false);
        PauseInput(false);
        inMainMenu = true;
        EventSystem.current.SetSelectedGameObject(playButton);
    }

    public IEnumerator OpenLoadOutMenu()
    {

        virtualCameraSwitcher.SwitchToVirtualCamera(1);
        currentMenuIndex = 0;
        ToggleText(true);
        mainMenu.GetComponent<DoTweenFade>().FadeOut();
        loadOut.Init();
        PauseInput(true);
        yield return new WaitForSeconds(0.8f);
        header.SetActive(true);
        header.GetComponent<DoTweenFade>().FadeIn();
        loadOutMenu.GetComponent<DoTweenFade>().canvasGroup.alpha = 0;
        loadOutMenu.SetActive(true);
        loadOutMenu.GetComponent<DoTweenFade>().FadeIn();
        mainMenu.SetActive(false);
        PauseInput(false);
        inMainMenu = false;
        EventSystem.current.SetSelectedGameObject(menuBUttons[0]);
    }

    public void OpenLoadOutMenuButton()
    {
        StartCoroutine(OpenLoadOutMenu());
    }

    public void OpenMainMenuButton(InputAction.CallbackContext context)
    {
        if(inMainMenu)
        {
            return;
        }
        if (context.performed)
        {
            StartCoroutine(OpenMainMenu());
        }
    }    
    
    public void OpenMainMenuButton()
    {
        StartCoroutine(OpenMainMenu());
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void PauseInput(bool pause)
    {
        if (pause)
        {
            playerInput.DeactivateInput();
        }
        else
        {
            playerInput.ActivateInput();
        }
    }
    
}
