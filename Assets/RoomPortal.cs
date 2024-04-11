using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPortal : MonoBehaviour
{
    public PortalEffect portalEffect;
    public PortalEffect visualPortalEffect;
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
        }
    }
}
