using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject loadOutMenu;
    public GameObject RDMenu;
    public GameObject AchMenu;
    public PlayerInput playerInput;
    public MechLoadOut loadOut;
    public SceneLoader sceneLoader;
    public GameObject[] menus;
    public int currentMenuIndex;
    public VirtualCameraSwitcher virtualCameraSwitcher;
    public TMP_Text headerText;

    private void Start()
    {
        AudioManager.instance.PlayMusic(0);
        sceneLoader = SceneLoader.instance;
    }

    public void StartGame()
    {
        sceneLoader.LoadScene(2, true);
    }

    public void ResetData()
    {
        PlayerSavedData.instance.ResetAllData();
    }

    #region New Way
    public void CycleUP()
    {
        StartCoroutine(OpenMenu(true));
    }

    public void CycleDOwn()
    {
        StartCoroutine(OpenMenu(false));
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
        menus[menu].SetActive(true);
        menus[menu].GetComponent<DoTweenFade>().FadeIn();
        menus[currentMenuIndex].SetActive(false);
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

    public IEnumerator OpenLoadOutMenu()
    {
        virtualCameraSwitcher.SwitchToVirtualCamera(1);
        currentMenuIndex = 0;
        mainMenu.GetComponent<DoTweenFade>().FadeOut();
        PauseInput(true);
        yield return new WaitForSeconds(0.8f);
        loadOutMenu.GetComponent<DoTweenFade>().canvasGroup.alpha = 0;
        loadOutMenu.SetActive(true);
        loadOutMenu.GetComponent<DoTweenFade>().FadeIn();
        mainMenu.SetActive(false);
        PauseInput(false);
    }

    public IEnumerator OpenRDMenu()
    {
        loadOutMenu.GetComponent<DoTweenFade>().FadeOut();
        PauseInput(true);
        RDMenu.GetComponent<DoTweenFade>().FadeIn();
        RDMenu.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        mainMenu.SetActive(false);
        PauseInput(false);
    }

    public IEnumerator CloseRDMenu()
    {
        PauseInput(true);
        mainMenu.SetActive(true);
        loadOut.Init();
        yield return new WaitForSeconds(1.8f);
        RDMenu.SetActive(false);
        PauseInput(false);
    }

    public IEnumerator OpenAchMenu()
    {
        PauseInput(true);
        AchMenu.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        mainMenu.SetActive(false);
        PauseInput(false);
    }

    public IEnumerator CloseAchMenu()
    {
        PauseInput(true);
        mainMenu.SetActive(true);
        yield return new WaitForSeconds(1.8f);
        AchMenu.SetActive(false);
        PauseInput(false);
    }

    public void OpenRDMenuButton()
    {
        StartCoroutine(OpenRDMenu());
    }

    public void CloseRDMenuButton()
    {
        StartCoroutine(CloseRDMenu());
    }

    public void OpenAchMenuButton()
    {
        StartCoroutine(OpenAchMenu());
    }

    public void CloseAchMenuButton()
    {
        StartCoroutine(CloseAchMenu());
    }

    public void OpenLoadOutMenuButton()
    {
        StartCoroutine(OpenLoadOutMenu());
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void PauseInput(bool pause)
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
