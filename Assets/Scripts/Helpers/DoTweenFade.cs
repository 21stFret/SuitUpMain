using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoTweenFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1;
    public LoopType loopType;
    public int loopCount = 0;

    void Start()
    {
    }

    public void FadeIn()
    {
        canvasGroup.DOFade(1, fadeDuration).SetLoops(loopCount, loopType);
    }

    public void FadeOut()
    {
        canvasGroup.DOFade(0, fadeDuration);
    }
}
