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
    public Animator objectiveAnimator;

    public void Init()
    {
        objectiveBar.enabled = true;
        objectiveBG.enabled = true;
        objectiveBar.fillAmount = 0;
        objectiveText.text = "";
    }

    public void UpdateBar(float fillamount)
    {
        objectiveBar.fillAmount = fillamount;
    }

    public void UpdateObjective(string objective)
    {
        // Update the objective text
        objectiveText.text = objective;
        ShowIntro();
    }

    public void ShowIntro()
    {
        objectiveAnimator.SetTrigger("Show");
    }

    public IEnumerator ObjectiveComplete()
    {
        string objective = "Objective Complete! \n Collect reward to comtinue";
        GameUI.instance.objectiveUI.UpdateObjective(objective);
        yield return new WaitForSeconds(2f);
        objectiveBar.enabled = false;
        objectiveBG.enabled = false;

    }

    public void ResetObjective()
    {
        objectiveBar.enabled = true;
        objectiveBG.enabled = true;
        objectiveText.transform.DOMoveY(objectiveText.transform.position.y - 25, 0.5f);
    }
}
