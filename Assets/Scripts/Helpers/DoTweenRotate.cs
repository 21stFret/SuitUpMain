using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoTweenRotate : MonoBehaviour
{
    public float rotationSpeed;
    public Ease ease;
    public Vector3 endRoatation;
    public LoopType loopType;
    public int loopCount;

    private void Start()
    {
        print("started tween rotation");
        transform.DOLocalRotate(endRoatation, rotationSpeed).SetLoops(loopCount, loopType).SetEase(ease);
    }
}
