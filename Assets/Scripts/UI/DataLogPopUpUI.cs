using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataLogPopUpUI : MonoBehaviour
{
    public Animator animator;   
    public TMP_Text logTitleText;
    public TMP_Text logDescriptionText;

    public void EnableLogPopUp(string title, string description)
    {
        logTitleText.text = title;
        logDescriptionText.text = description;
        animator.SetBool("Open", true);
        animator.gameObject.SetActive(true);
        GameUI.instance.pauseMenu.canQuickOpen = true;
        StartCoroutine(DisableLogPopUpAfterDelay(5f)); // Disable after 5 seconds
    }

    public void DisableLogPopUp()
    {
        animator.SetBool("Open", false);
        GameUI.instance.pauseMenu.canQuickOpen = false;
    }

    private IEnumerator DisableLogPopUpAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DisableLogPopUp();
    }
}
