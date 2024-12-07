using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudPatch : MonoBehaviour
{
    private MYCharacterController player;
    private float savedSpeed;
    public float slowAmount = 0.2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(player != null)
            {
                return;
            }
            player = other.gameObject.GetComponent<MYCharacterController>();
            player.ToggleSlow(slowAmount, true);
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
            player.ToggleSlow(slowAmount, false);
            player = null;
        }
    }
}
