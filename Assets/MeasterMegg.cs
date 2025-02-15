using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MeasterMegg : MonoBehaviour
{
    private int count = 0;
    public TMP_Text text;
    public Animator menu;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpecialCrate"))
        {
            count++;
        }

        if (count == 3)
        {
            if(PlayerSavedData.instance != null)
            {
                if(!PlayerSavedData.instance.triggeredEasterEgg)
                {
                    PlayerSavedData.instance.UpdatePlayerCash(3000);
                    PlayerSavedData.instance.triggeredEasterEgg = true;
                    BaseManager.instance.statsUI.UpdateCash(PlayerSavedData.instance._Cash);
                    PlayerSavedData.instance.SavePlayerData();
                    text.text = "You collected 3 crates! Well done loser! Just kidding you're awesome! Have 3000 cash!";
                    GetComponent<Collider>().enabled = false;
                    count = 10;
                }
                else
                {
                    text.text = "Only once per play through, dont be a #@*$!";
                }

            }
            StartCoroutine(CloseMenu());
        }
    }

    private IEnumerator CloseMenu()
    {
        menu.SetBool("Open", true);
        yield return new WaitForSeconds(5);
        menu.SetBool("Open", false);
    }
}
