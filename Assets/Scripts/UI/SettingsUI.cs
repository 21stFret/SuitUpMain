using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SettingsUI : MonoBehaviour
{
    public Slider BGM;
    public Slider SFX;
    public Toggle damageNumbersToggle;
    public bool damageNumbersOn;

    private void OnEnable()
    {
        BGM.value = AudioManager.instance.musicVolume;
        SFX.value = AudioManager.instance.sfxVolume;
        if (PlayerPrefs.HasKey("DamageNumbers"))
        {
            damageNumbersOn = PlayerPrefs.GetInt("DamageNumbers", 1) == 1;
        }
        else
        {
            PlayerPrefs.SetInt("DamageNumbers", 1);
        }
        damageNumbersToggle.isOn = damageNumbersOn;
    }

    public void SetDamageNumber(bool value)
    {
        damageNumbersOn = value;
        PlayerPrefs.SetInt("DamageNumbers", damageNumbersOn ? 1 : 0);
        var targets = FindObjectsOfType<TargetHealth>();
        foreach (var target in targets)
        {
            target.SetDamageNumbers(damageNumbersOn);
        }
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
