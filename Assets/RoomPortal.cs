using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPortal : MonoBehaviour
{
    public PortalEffect portalEffect;
    public PortalEffect visualPortalEffect;
    public AudioClip[] portalSounds;
    public AudioSource audioSource;
    public bool _active;

    private void OnTriggerEnter(Collider other)
    {
        if(!_active) return;

        if (other.CompareTag("Player"))
        {
            visualPortalEffect.gameObject.SetActive(true);
            visualPortalEffect.StartFirstPersonEffect();
            GameManager.instance.LoadNextRoom();
            _active = false;
            audioSource.clip = portalSounds[Random.Range(0, portalSounds.Length)];
            audioSource.Play();
        }
    }
}
