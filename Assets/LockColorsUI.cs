using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockColorsUI : MonoBehaviour
{
    public List<Image> lockColors = new List<Image>();
    public TMP_Text lockNameText;

    public void SetText(string _lock)
    {
        if (lockNameText != null)
        {
            lockNameText.text = _lock;
        }
    }

    public void SetColor(ComboLock _lock)
    {
        for (int i = 0; i < lockColors.Count; i++)
        {
            switch (_lock.buildLocks[i])
            {
                case ModBuildType.ASSAULT:
                    lockColors[i].color = Color.red; // Example color for assault
                    break;
                case ModBuildType.TECH:
                    lockColors[i].color = Color.cyan; // Example color for tech
                    break;
                case ModBuildType.TANK:
                    lockColors[i].color = Color.green; // Example color for tank
                    break;
                case ModBuildType.AGILITY:
                    lockColors[i].color = Color.yellow; // Example color for agility
                    break;
            }
        }
    }
}
