using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTweenScale : MonoBehaviour
{
    public float duration;
    public Ease ease;
    public Vector3 endPos;
    public LoopType loopType;
    public int loopCount;
    public bool autoStart;


    private void Start()
    {
        if (autoStart)
        {
            transform.DOScale(endPos, duration).SetLoops(loopCount, loopType).SetEase(ease);
        }
    }

    public void StartTween()
    {
        transform.DOScale(endPos, duration).SetLoops(loopCount, loopType).SetEase(ease);
    }
}
