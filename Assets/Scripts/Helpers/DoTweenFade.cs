using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoTweenFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    void Start()
    {
    }

    public void PlayTween()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(1, 1).OnComplete(ReverseTween);

    }
    public void ReverseTween()
    {
        canvasGroup.DOFade(0, 1);
    }
}
