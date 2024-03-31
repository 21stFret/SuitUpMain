using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudPatch : MonoBehaviour
{
    private MYCharacterController player;
    private float savedSpeed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(player != null)
            {
                return;
            }
            player = other.gameObject.GetComponent<MYCharacterController>();
            savedSpeed = player.Speed;
            player.Speed = player.Speed / 2;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (player == null)
            {
                return;
            }
            player.Speed = savedSpeed;
            player = null;
        }
    }
}
