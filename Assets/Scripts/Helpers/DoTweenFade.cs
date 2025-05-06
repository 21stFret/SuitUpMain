using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DoTweenFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Material material;
    public Image image;
    public float fadeDuration = 1;
    public LoopType loopType;
    public int loopCount = 0;
    public float fadeValue;
    public bool disableOnEnd;

    public Tween tween;

    private int loopCountValue = 0;

    void Start()
    {
    }

    public void PlayTween()
    {
        if(tween != null)
        {
            return; // If a tween is already playing, do nothing
        }
        if (canvasGroup != null)
        {
            tween = canvasGroup.DOFade(fadeValue, fadeDuration).SetLoops(loopCount, loopType).OnComplete(OnComplete);;
        }
        if(image != null)
        {
            tween = image.DOFade(fadeValue, fadeDuration).SetLoops(loopCount, loopType).OnComplete(OnComplete);;
        }
        if(material != null)
        {
            tween = material.DOFade(fadeValue, fadeDuration).SetLoops(loopCount, loopType).OnComplete(OnComplete);;
        }
    }

    public void KillTween()
    {
        if (canvasGroup != null)
        {
            canvasGroup.DOKill();
            canvasGroup.alpha = 0;
        }
        if (image != null)
        {
            image.DOKill();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
        if (material != null)
        {
            material.DOKill();
            material.color = new Color(material.color.r, material.color.g, material.color.b, 0);
        }
    }

    public void FadeIn()
    {
        KillTween(); // Kill any existing tweens first

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, fadeDuration);
        }
        if (image != null)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            image.DOFade(1, fadeDuration);
        }
        if (material != null)
        {
            material.color = new Color(material.color.r, material.color.g, material.color.b, 0);
            material.DOFade(1, fadeDuration);
        }
    }

    public void FadeOut()
    {
        if (canvasGroup != null)
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(0, fadeDuration).OnComplete(OnComplete);
        }
        if (image != null)
        {
            image.DOKill();
            image.DOFade(0, fadeDuration).OnComplete(OnComplete);
        }
        if (material != null)
        {
            material.DOKill();
            material.DOFade(0, fadeDuration).OnComplete(OnComplete);
        }
    }

    public void OnComplete()
    {
        if(loopCount>0)
        {
            loopCountValue++;
            if(loopCountValue >= loopCount-1)
            {
                loopCountValue = 0;
                tween.Kill();
                tween = null;
            }
        }
        else
        {
            tween.Kill();
            tween = null; // Reset the tween reference
            if (disableOnEnd)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
