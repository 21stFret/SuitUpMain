using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;


public enum SFX
{
    Move,
    Select,
    Back,
    Confirm,
    Error,
    Hurt
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioClip[] musicClips;
    public AudioClip[] effectClips;
    public AudioClip[] hurtClips;

    public AudioSource backgroundMusic;
    public AudioSource soundEffect;
    [Range(0, 1)]
    public float musicVolume = 1f; // Volume for music
    [Range(0, 1)]
    public float sfxVolume = 1f;   // Volume for sound effects

    private int currentClipIndex;

    public EventSystem eventSystem;
    private GameObject lastSelectedObject;

    public bool buttonClicked;

    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (eventSystem.currentSelectedGameObject != lastSelectedObject)
        {
            lastSelectedObject = eventSystem.currentSelectedGameObject;
            if(lastSelectedObject == null)
            {
                return;
            }
            if(lastSelectedObject.layer == 11)
            {
                return;
            }
            if (buttonClicked)
            {
                buttonClicked = false;
                return;
            }
            PlaySFX(SFX.Move);
        }  
    }

    public void Init()
    {
        //Get initial volumes
        musicVolume = PlayerSavedData.instance._BGMVolume;
        sfxVolume = PlayerSavedData.instance._SFXVolume;
        // Set initial volumes
        backgroundMusic.volume = musicVolume;
        soundEffect.volume = sfxVolume;
    }

    // Play a music clip
    public void PlayMusic(int clipIndex)
    {
        backgroundMusic.DOFade(0, 1f).OnComplete(FadeMusicIn);
        currentClipIndex = clipIndex;
    }

    private void FadeMusicIn()
    {
        backgroundMusic.clip = musicClips[currentClipIndex];
        backgroundMusic.Play();
        backgroundMusic.DOFade(musicVolume, 1f);
    }

    // Play a sound effect clip
    public void PlaySFX(SFX clip)
    {
        soundEffect.pitch = 1f;
        soundEffect.PlayOneShot(effectClips[(int)clip]);
    }

    public void PlayHurt()
    {
        soundEffect.pitch = Random.Range(0.8f, 1.2f);
        soundEffect.PlayOneShot(hurtClips[Random.Range(0, hurtClips.Length)]);
    }

    public void PlayButtonSFX(int clipID)
    {
        soundEffect.pitch = 1f;
        buttonClicked = true;
        soundEffect.PlayOneShot(effectClips[clipID]);
    }

    public void PlaySFXFromClip(AudioClip clip)
    {
        soundEffect.pitch = 1f;
        soundEffect.PlayOneShot(clip);
    }

    public void PlaySFXFromClipLooping(AudioClip clip)
    {
        soundEffect.pitch = 1f;
        soundEffect.clip = clip;
        soundEffect.loop = true;
        soundEffect.Play();
    }

    public void StopSFX()
    {
        soundEffect.loop = false;
        soundEffect.Stop();
    }
    // Adjust music volume
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        backgroundMusic.volume = musicVolume;
        PlayerSavedData.instance.UpdateBGMVolume(musicVolume);
        PlayerSavedData.instance.SavePlayerData();
    }

    // Adjust sound effects volume
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        soundEffect.volume = sfxVolume;
        PlayerSavedData.instance.UpdateSFXVolume(sfxVolume);
        PlayerSavedData.instance.SavePlayerData();
    }
}
