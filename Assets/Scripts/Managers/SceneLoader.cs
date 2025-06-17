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
    private bool isLoading = false;
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
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (loadinBar == null)
        {
            return;
        }
        hintManager.HideHint();
        loadinBar.enabled = false;
        StartCoroutine(DelayFade());
        isLoading = false;
    }

    private IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(0.5f);
        blackout.FadeOut();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void LoadScene(int sceneID, bool delay = false)
    {
        if (isLoading)
        {
            Debug.Log("Already loading a scene, ignoring request for scene ID: " + sceneID);
            return;
        }

        isLoading = true;
        loadinBar.fillAmount = 0;
        StartCoroutine(LoadSceneAsync(sceneID, delay));
    }

    IEnumerator LoadSceneAsync(int sceneID, bool delay = false)
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
        
        // Start the loading operation but don't allow it to complete automatically
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        operation.allowSceneActivation = false;
        
        float cycleTime = 2.0f; // Time in seconds for one full cycle
        float cycleProgress = 0f;

        // Keep cycling the loading bar until the actual loading is nearly complete
        while (operation.progress < 0.9f)
        {
            // Cycle the loading bar
            cycleProgress += Time.deltaTime / cycleTime;
            if (cycleProgress > 1f)
                cycleProgress -= 1f; // Reset but don't jump (smooth looping)
                
            // Map the progress from 0-1 to display on loading bar
            loadinBar.fillAmount = cycleProgress;
            
            yield return null;
        }
        
        // If we need a minimum loading time
        if (delay)
        {
            float elapsedTime = 0f;
            while (elapsedTime < minloadTime)
            {
                elapsedTime += Time.deltaTime;
                
                // Continue cycling the bar
                cycleProgress += Time.deltaTime / cycleTime;
                if (cycleProgress > 1f)
                    cycleProgress -= 1f;
                    
                loadinBar.fillAmount = cycleProgress;
                
                yield return null;
            }
        }
        
        // Fill the bar completely before activating the scene
        float fillTime = 0.5f;
        float fillTimer = 0f;
        float startFill = loadinBar.fillAmount;
        
        while (fillTimer < fillTime)
        {
            fillTimer += Time.deltaTime;
            loadinBar.fillAmount = Mathf.Lerp(startFill, 1f, fillTimer / fillTime);
            yield return null;
        }
        
        // Finally allow the scene to activate
        operation.allowSceneActivation = true;
    }
}
