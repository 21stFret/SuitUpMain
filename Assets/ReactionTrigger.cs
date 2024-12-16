using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReactionTrigger : MonoBehaviour
{
    public Animator animator;
    public string boolName;

    public UnityEvent triggerEvent;
    public UnityEvent triggerEndEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerEvent();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EndEvent();
        }
    }

    private void TriggerEvent()
    {
        if(animator != null)
        {
            animator.SetBool(boolName, true);
        }
        if(triggerEvent != null)
        {
            triggerEvent.Invoke();
        }
    }

    private void EndEvent()
    {
        if(animator != null)
        {
            animator.SetBool(boolName, false);
        }
        if(triggerEndEvent != null)
        {
            triggerEndEvent.Invoke();
        }
    }
}
