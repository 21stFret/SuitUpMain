using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DoTweenFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Image image;
    public float fadeDuration = 1;
    public LoopType loopType;
    public int loopCount = 0;
    public float fadeValue;

    void Start()
    {
    }

    public void PlayTween()
    {
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(fadeValue, fadeDuration).SetLoops(loopCount, loopType);
        }
        if(image != null)
        {
            image.DOFade(fadeValue, fadeDuration).SetLoops(loopCount, loopType);
        }
    }

    public void FadeOut()
    {
        canvasGroup.DOFade(0, fadeDuration);
        image.DOFade(0, fadeDuration);
    }
}
