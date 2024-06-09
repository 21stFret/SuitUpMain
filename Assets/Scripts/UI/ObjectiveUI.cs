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
    }

    public IEnumerator ObjectiveComplete()
    {
        GameUI.instance.objectiveUI.UpdateObjective("Objective Complete");
        objectiveText.material.SetFloat("_GlowPower", 1);
        yield return new WaitForSeconds(2f);
        objectiveBar.enabled = false;
        objectiveBG.enabled = false;
        objectiveText.material.SetFloat("_GlowPower", 0);
        objectiveText.transform.DOMoveY(objectiveText.transform.position.y + 25, 0.5f);

    }

    public void ResetObjective()
    {
        objectiveBar.enabled = true;
        objectiveBG.enabled = true;
        objectiveText.transform.DOMoveY(objectiveText.transform.position.y - 25, 0.5f);
    }
}
