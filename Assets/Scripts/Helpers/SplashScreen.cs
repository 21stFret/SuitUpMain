using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    public DoTweenFade doTweenFade;
    public DoTweenFade blackout;
    public SceneLoader sceneLoader;
    private bool skipped;

    private void Start()
    {
        sceneLoader = SceneLoader.instance;
        ShowSpalshScreen();
    }

    public void ShowSpalshScreen()
    {
        StartCoroutine(StartGame());
    }

    public IEnumerator StartGame()
    {
        blackout.FadeOut();
        yield return new WaitForSeconds(3);
        blackout.PlayTween();
        yield return new WaitForSeconds(1);
        doTweenFade.FadeOut();
        sceneLoader.LoadScene(1);

    }

    public void SkipSplashScreen()
    {
        if (skipped)
        {
            return;
        }
        StopAllCoroutines();
        blackout.PlayTween();
        doTweenFade.FadeOut();
        sceneLoader.LoadScene(1);
        skipped = true;
    }
}
