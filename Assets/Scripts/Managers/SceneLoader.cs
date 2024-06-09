using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;
    public DoTweenFade blackout;
    public Image loadinBar;
    public float minloadTime = 2;
    public AudioManager audioManager;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(loadinBar == null)
        {
            return;
        }
        loadinBar.enabled = false;
        blackout.FadeOut();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(int sceneID, bool delay = false)
    {
        StartCoroutine(LoadSceneAsync(sceneID, delay));
    }

    IEnumerator LoadSceneAsync(int sceneID ,bool delay = false)
    {

        blackout.PlayTween();

        if(audioManager == null)
        {
            audioManager = AudioManager.instance;
        }
        audioManager.PlayMusic(2);

        yield return new WaitForSeconds(1);

        loadinBar.enabled = true;

        if (delay)
        {
            yield return new WaitForSeconds(minloadTime);
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadinBar.fillAmount = progress;
            yield return null;
        }
    }
}
