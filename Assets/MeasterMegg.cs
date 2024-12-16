using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasterMegg : MonoBehaviour
{
    private int count = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Crate"))
        {
            count++;
        }

        if (count >= 3)
        {
            Debug.Log("You collected 3 crates! Well done loser haha! Just kidding your'e awesome :P Have 5000 cash! ");
            PlayerSavedData.instance.UpdatePlayerCash(5000);
        }
    }
}
