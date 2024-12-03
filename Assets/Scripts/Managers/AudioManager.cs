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
    Hurt
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioClip[] musicClips;
    public AudioClip[] effectClips;
    public AudioClip[] hurtClips;
    public AudioClip[] healClips;
    public AudioMixer audioMixer;
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

    private void Start()
    {
        //Init();
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
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(musicVolume) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
    }

    // Play a music clip
    public void PlayMusic(int clipIndex)
    {
        DOVirtual.Float(musicVolume, 0.0001f, 1f, v => audioMixer.SetFloat("BGMVolume", Mathf.Log10(v) * 20)).OnComplete(FadeMusicIn);
        //DOTween.To(() => volume, x => volume = x, 0.0001f, 1f).OnComplete(() => FadeMusicIn());
        //audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
        currentClipIndex = clipIndex;
    }

    private void FadeMusicIn()
    {
        backgroundMusic.clip = musicClips[currentClipIndex];
        backgroundMusic.Play();
        DOVirtual.Float(0.0001f, musicVolume, 1f, v => audioMixer.SetFloat("BGMVolume", Mathf.Log10(v) * 20));
        //DOTween.To(() => volume, x => volume = x, musicVolume, 1f);
        //audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
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
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
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
