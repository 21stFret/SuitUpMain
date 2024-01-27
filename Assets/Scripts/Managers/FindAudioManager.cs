using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FindAudioManager : MonoBehaviour
{
    void Start()
    {
        AudioManager.instance.eventSystem = GetComponent<EventSystem>();
    }

}
