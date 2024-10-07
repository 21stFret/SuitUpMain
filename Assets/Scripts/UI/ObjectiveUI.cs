using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUI : MonoBehaviour
{
    public Image objectiveBar;
    public Image objectiveBG;
    public TMP_Text objectiveText;
    public TMP_Text progressText;
    public Animator objectiveAnimator;
    public float ObjectiveFlashTime = 2f;

    public void Init(bool showBar)
    {
        objectiveBar.enabled = showBar;
        objectiveBG.enabled = showBar;
        objectiveBar.fillAmount = 0;
        objectiveText.text = "";
        progressText.text = showBar? "Upload Progress 0%" : "";
    }

    public void UpdateBar(float fillamount)
    {
        objectiveBar.fillAmount = fillamount;
    }

    public void UpdateObjective(string objective)
    {
        // Update the objective text
        objectiveText.text = objective;
        StartCoroutine(ShowObjective());
    }

    public IEnumerator ShowObjective()
    {
        TogglePanel(true);
        yield return new WaitForSeconds(ObjectiveFlashTime);
        TogglePanel(false);
    }

    public void UpdateUpload(string objective)
    {
        // Update the objective text
        objectiveBar.enabled = true;
        objectiveBG.enabled = true;
        progressText.text = objective;
        //TogglePanel(true);
    }

    public void TogglePanel(bool open)
    {
        objectiveAnimator.SetBool("Open", open);
    }

    public IEnumerator ObjectiveComplete()
    {
        yield return new WaitForSeconds(1f);
        objectiveBar.enabled = false;
        objectiveBG.enabled = false;
        progressText.text = "";
        yield return new WaitForSeconds(1f);
        string objective = "Objective Complete! \n Collect reward to continue!";
        UpdateObjective(objective);
    }
}
