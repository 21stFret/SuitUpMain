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

    public HintManager hintManager;

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
        hintManager.HideHint();
        loadinBar.enabled = false;
        blackout.FadeOut();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(int sceneID, bool delay = false)
    {
        loadinBar.fillAmount = 0;
        StartCoroutine(LoadSceneAsync(sceneID, delay));
    }

    IEnumerator LoadSceneAsync(int sceneID ,bool delay = false)
    {

        blackout.PlayTween();

        if(audioManager == null)
        {
            audioManager = AudioManager.instance;
        }
        audioManager.PlayBGMusic(1);

        yield return new WaitForSeconds(1);
        hintManager.ShowRandomHint();

        loadinBar.enabled = true;

        float fakeProgress = 0;
        float fakeTime = 0;

        if (delay)
        {
            while (fakeTime < minloadTime)
            {
                fakeTime += Time.deltaTime;
                fakeProgress = fakeTime / minloadTime;
                loadinBar.fillAmount = fakeProgress;
                yield return null;
            }
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress +fakeProgress);
            loadinBar.fillAmount = progress;
            yield return null;
        }
    }
}
