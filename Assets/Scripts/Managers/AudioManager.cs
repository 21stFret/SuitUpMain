using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Audio;


public enum SFX
{
    Move,
    Select,
    Back,
    Confirm,
    Error,
    Unlock
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [NamedArray(new string[] { "Main Menu", "Loading", "Tutorial", "Base", "Win Game", "DownTime" })]
    public AudioClip[] musicClips;
    public AudioClip[] battleClips;
    public AudioClip[] bossTracks;
    [NamedArray(new string[] { "Move", "Select", "Back", "Confirm", "Error", "Unlock" })]
    public AudioClip[] effectClips;
    public AudioClip[] hurtClips;
    public AudioClip[] healClips;
    public AudioClip[] crawalerDeathClips;
    public AudioClip[] crawalerSpawnClips;
    public AudioMixer audioMixer;
    public AudioSource backgroundMusic;
    public AudioSource soundEffect;
    [Range(0, 1)]
    public float musicVolume = 1f; // Volume for music
    [Range(0, 1)]
    public float sfxVolume = 1f;   // Volume for sound effects

    private int currentClipIndex;
    private AudioClip currentClip;

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

    private void Start()
    {
        //Init();
    }

    private void Update()
    {
        if(eventSystem == null)
        {
            eventSystem = EventSystem.current;
        }
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
        musicVolume = PlayerSavedData.instance._BGMV;
        sfxVolume = PlayerSavedData.instance._SFXV;
        // Set initial volumes
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    // Play a music clip
    public void PlayMusic(AudioClip clip)
    {
        currentClip = clip;
        DOVirtual.Float(musicVolume, 0.0001f, 1f, v => audioMixer.SetFloat("BGMVolume", Mathf.Log10(v) * 20)).OnComplete(FadeMusicIn);
    }

    public AudioClip GetRandomDeathNoise()
    {
        return crawalerDeathClips[Random.Range(0, crawalerDeathClips.Length)];
    }

    public AudioClip GetRandomSpawnNoise()
    {
        return crawalerSpawnClips[Random.Range(0, crawalerSpawnClips.Length)];
    }

    public void PlayBossMusic(int trackID = -1)
    {
        if (trackID == -1)
        {
            trackID = Random.Range(0, bossTracks.Length);
        }

        PlayMusic(bossTracks[trackID]);
    }

    public void PlayBattleMusic(int trackID = -1)
    {
        if (trackID == -1)
        {
            trackID = Random.Range(0, battleClips.Length);
        }
        PlayMusic(battleClips[trackID]);
    }

    public void PlayBGMusic(int trackID = -1)
    {
        if(trackID == -1)
        {
            trackID = Random.Range(0, musicClips.Length);
        }
        PlayMusic(musicClips[trackID]);
    }

    private void FadeMusicIn()
    {
        backgroundMusic.clip = currentClip;
        backgroundMusic.Play();
        DOVirtual.Float(0.0001f, musicVolume, 1f, v => audioMixer.SetFloat("BGMVolume", Mathf.Log10(v) * 20));
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

    public void PlayHeal()
    {
        soundEffect.pitch = Random.Range(0.8f, 1.2f);
        soundEffect.PlayOneShot(healClips[Random.Range(0, healClips.Length)]);
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
        float adjustedVolume = volume * 0.5f;
        float targetVolume = Mathf.Log10(adjustedVolume) * 20;
        audioMixer.SetFloat("BGMVolume", targetVolume);
        PlayerSavedData.instance.UpdateBGMVolume(musicVolume);
        PlayerSavedData.instance.SavePlayerData();
    }

    // Adjust sound effects volume
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerSavedData.instance.UpdateSFXVolume(sfxVolume);
        PlayerSavedData.instance.SavePlayerData();
    }
}
