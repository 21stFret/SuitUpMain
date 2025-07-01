using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatInfoUI : MonoBehaviour
{
    public TMP_Text statNameText;
    public TMP_Text statValueText;

    public void UpdateStatValue(float value)
    {
        value -= 100;
        if (value < 0)
        {
            statValueText.color = Color.red;
        }
        else if (value > 0)
        {
            statValueText.color = Color.green;
        }
        else
        {
            statValueText.color = Color.white;
        }
        statValueText.text = value.ToString("0.00") + "%";
    }
    public void UpdateStatName(string name)
    {
        statNameText.text = name;
    }
}
