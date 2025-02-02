using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPortal : MonoBehaviour
{
    public PortalEffect visualPortalEffect;
    public AudioClip[] portalSounds;
    public AudioSource audioSource;
    public bool _active;
    public bool voidPortal;
    public VoidPortalManager voidPortalManager;
    public ModBuildType portalType;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        visualPortalEffect = GetComponent<PortalEffect>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!_active) return;
        if (other.CompareTag("Player"))
        {
            StartCoroutine(LoadRoomRoutine());
        }

    }

    private IEnumerator LoadRoomRoutine()
    {
        BattleMech.instance.myCharacterController.ToggleCanMove(false);
        voidPortalManager.StartFirstPersonEffect(portalType);
        yield return new WaitForSeconds(1f);
        if (voidPortal)
        {
            GameManager.instance.StartCoroutine(GameManager.instance.LoadVoidRoom());
            voidPortalManager.StopVoidEffect();
        }
        else
        {
            GameManager.instance.LoadNextRoom();
            GameManager.instance.nextBuildtoLoad = portalType;
        }

        voidPortalManager.StopAllEffects();
        _active = false;
        audioSource.clip = portalSounds[Random.Range(0, portalSounds.Length)];
        audioSource.Play();      
    }
}
