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

        if (count >= 3)
        {
            if(PlayerSavedData.instance != null)
            {
                if(!PlayerSavedData.instance._triggeredEasterEgg)
                {
                    PlayerSavedData.instance.UpdatePlayerCash(5000);
                    PlayerSavedData.instance._triggeredEasterEgg = true;
                    text.text = "You collected 3 crates! Well done loser haha! Just kidding your'e awesome :P Have 1000 cash!";
                }
                else
                {
                    text.text = "Only once per play through, dont be a #@*$!";
                }

            }
            menu.SetBool("Open", true);
        }
    }

    private IEnumerator CloseMenu()
    {
        yield return new WaitForSeconds(5);
        menu.SetBool("Open", false);
    }
}
