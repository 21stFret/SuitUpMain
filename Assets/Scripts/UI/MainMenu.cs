using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject RDMenu;
    public PlayerInput playerInput;

    private void Start()
    {
        AudioManager.instance.PlayMusic(0);
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void OpenRDMenuButton()
    {
        StartCoroutine(OpenRDMenu());
    }

    public void CloseRDMenuButton()
    {
        StartCoroutine(CloseRDMenu());
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
        yield return new WaitForSeconds(0.8f);
        RDMenu.SetActive(false);
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
