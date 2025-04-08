using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudPatch : MonoBehaviour
{
    private MYCharacterController player;
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
            player.TriggerSand();
        }
    }
}
