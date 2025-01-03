using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerEvent : MonoBehaviour
{
    public float timeToWait = 1.0f;
    public UnityEvent onTimerEnd;

    public void StartTimer()
    {
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(timeToWait);
        onTimerEnd?.Invoke();
    }
}
