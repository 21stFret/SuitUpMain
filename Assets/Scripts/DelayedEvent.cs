using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEvent : MonoBehaviour
{
    public float delay = 1f;
    public UnityEvent triggerEvent;

    public void TriggerEvent()
    {
        StartCoroutine(DelayedTrigger());
    }

    private IEnumerator DelayedTrigger()
    {
        yield return new WaitForSeconds(delay);
        triggerEvent.Invoke();
    }
}
