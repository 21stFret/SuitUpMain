using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoTweenRotate : MonoBehaviour
{
    public float rotationSpeed;
    public Vector3 endRoatation;
    public LoopType loopType;
    public int loopCount;

    private void Start()
    {
        print("started tween rotation");
        transform.DOLocalRotate(endRoatation, rotationSpeed, RotateMode.FastBeyond360).SetLoops(loopCount, loopType).SetEase(Ease.Linear);
    }
}
