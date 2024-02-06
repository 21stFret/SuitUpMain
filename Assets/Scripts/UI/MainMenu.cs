using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject RDMenu;

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
        RDMenu.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        mainMenu.SetActive(false);
    }

    public IEnumerator CloseRDMenu()
    {
        mainMenu.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        RDMenu.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }   
}
