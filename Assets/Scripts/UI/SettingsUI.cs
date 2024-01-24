using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider BGM;
    public Slider SFX;

    private void OnEnable()
    {
        BGM.value = AudioManager.instance.musicVolume;
        SFX.value = AudioManager.instance.sfxVolume;
    }
}
