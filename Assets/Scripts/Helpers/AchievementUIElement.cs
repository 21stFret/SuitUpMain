using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementUIElement : MonoBehaviour
{
    public TMP_Text nameText;
    public GameObject achievedBG;
    public string description;
    public Image icon;
    public Color achievedColor;

    public void SetAchievement(Achievement achievement)
    {
        nameText.text = achievement.name;
        achievedBG.SetActive(achievement.achieved);
        icon.color = !achievement.achieved ? Color.white :achievedColor;
    }
}
