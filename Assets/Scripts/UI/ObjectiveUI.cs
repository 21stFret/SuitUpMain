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
    public GameObject surviveText;
    public GameObject destoryText;
    public TMP_Text progressText;
    public TMP_Text percentageText;
    public Animator hintAnimator;
    public TMP_Text hintText;
    public float ObjectiveFlashTime = 2f;
    public GameObject objectivePanel;

    public void Init(bool showBar, bool showPercent, bool showSurvive, string showDestroy)
    {
        objectiveBar.enabled = showBar;
        objectiveBG.enabled = showBar;
        objectiveBar.fillAmount = 0;
        surviveText.SetActive(showSurvive);
        percentageText.text = "";
        percentageText.enabled = showPercent;
        progressText.text = BattleManager.instance._usingBattleType == BattleType.Survive ? "survive" : "upload";
        if(showDestroy != "")
        {
            destoryText.GetComponent<TMP_Text>().text = showDestroy;       
            destoryText.SetActive(true);
        }
        objectivePanel.SetActive(showBar);
    }

    public void UpdateBar(float fillamount)
    {
        objectiveBar.fillAmount = fillamount;
    }

    public void UpdateObjective(string objective)
    {
        // Update the objective text
        hintText.text = objective;
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
        percentageText.text = objective;
        //TogglePanel(true);
    }

    public void TogglePanel(bool open)
    {
        hintAnimator.SetBool("Open", open);
    }

    public void HideObjectivePanel()
    {
        objectiveBar.enabled = false;
        objectiveBG.enabled = false;
        progressText.text = "";
        objectivePanel.SetActive(false);
    }

    public IEnumerator ObjectiveComplete()
    {
        yield return new WaitForSeconds(1f);
        objectiveBar.enabled = false;
        objectiveBG.enabled = false;
        progressText.text = "";
        objectivePanel.SetActive(false);
        yield return new WaitForSeconds(1f);
        string objective = "Objective Complete! \n Collect reward to continue!";
        UpdateObjective(objective);
    }
}
