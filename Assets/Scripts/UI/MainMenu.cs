using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject RDMenu;
    public GameObject AchMenu;
    public PlayerInput playerInput;
    public MechLoadOut loadOut;
    public SceneLoader sceneLoader;

    private void Start()
    {
        AudioManager.instance.PlayMusic(0);
        sceneLoader = SceneLoader.instance;
    }

    public void StartGame()
    {
        sceneLoader.LoadScene(2, true);
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

    public IEnumerator OpenRDMenu()
    {
        PauseInput(true);
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
