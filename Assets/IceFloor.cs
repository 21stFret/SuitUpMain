using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceFloor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MYCharacterController player = other.gameObject.GetComponent<MYCharacterController>();
            player.onIce = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MYCharacterController player = other.gameObject.GetComponent<MYCharacterController>();
            player.onIce = false;
        }
    }
}
