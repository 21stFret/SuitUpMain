using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SettingsUI : MonoBehaviour
{
    public Slider BGM;
    public Slider SFX;

    private void OnEnable()
    {
        BGM.value = AudioManager.instance.musicVolume;
        SFX.value = AudioManager.instance.sfxVolume;
    }

    public void SetBGMVolume(float value)
    {
        AudioManager.instance.SetMusicVolume(value);
    }
    
    public void SetSFXVolume(float value)
    {
        AudioManager.instance.SetSFXVolume(value);
    }

    public void PlayTestSFX()
    {
        AudioManager.instance.PlayButtonSFX(0);
    }
}
