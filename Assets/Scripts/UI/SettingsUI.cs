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

    public void SetupDamageNumbers()
    {
        if (PlayerPrefs.HasKey("DamageNumbers"))
        {
            damageNumbersOn = PlayerPrefs.GetInt("DamageNumbers", 1) == 1;
        }
        else
        {
            PlayerPrefs.SetInt("DamageNumbers", 1);
        }
        damageNumbersToggle.isOn = damageNumbersOn;
        SetDamageNumber(damageNumbersOn);
        damageNumbersToggle.onValueChanged.AddListener(SetDamageNumber);
    }

    private void OnEnable()
    {
        BGM.value = AudioManager.instance.musicVolume;
        SFX.value = AudioManager.instance.sfxVolume;
        BGM.onValueChanged.RemoveAllListeners();
        SFX.onValueChanged.RemoveAllListeners();
        BGM.onValueChanged.AddListener(SetBGMVolume);
        SFX.onValueChanged.AddListener(SetSFXVolume);
        SetupDamageNumbers();
    }

    public void SetDamageNumber(bool value)
    {
        damageNumbersOn = value;
        PlayerPrefs.SetInt("DamageNumbers", damageNumbersOn ? 1 : 0);
        var targets = FindObjectsOfType<TargetHealth>(true);
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

    private void OnDisable()
    {
        BGM.onValueChanged.RemoveListener(SetBGMVolume);
        SFX.onValueChanged.RemoveListener(SetSFXVolume);
        damageNumbersToggle.onValueChanged.RemoveListener(SetDamageNumber);
    }
}
