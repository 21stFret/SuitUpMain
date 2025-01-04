using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using System;
using System.Collections;

public class CinemachinePriorityMonitor : MonoBehaviour
{
    public CinemachineBrain cinemachineBrain;
    public UnityEvent OnTargetPriorityReached;

    public void StartBlendWait()
    {
        StartCoroutine(WaitForBlend());
    }

    private IEnumerator WaitForBlend()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !cinemachineBrain.IsBlending);
        OnTargetPriorityReached?.Invoke();
    }
}