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
        if(material != null)
        {
            material.DOFade(fadeValue, fadeDuration).SetLoops(loopCount, loopType);
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
            canvasGroup.DOFade(0, fadeDuration);
        }
        if (image != null)
        {
            image.DOFade(0, fadeDuration);
        }
        if (material != null)
        {
            material.DOFade(0, fadeDuration);
        }
    }
}
