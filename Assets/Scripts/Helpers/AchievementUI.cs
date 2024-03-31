using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    public PlayerAchievements playerAchievements;
    public PlayerSavedData playerSavedData;
    public List<AchievementUIElement> achievementPrefabs;
    public List<Sprite> icons;
    public List<StatUIElement> statPrefabs;
    public TMP_Text descriptionText;
    public EventSystem eventSystem;
    private GameObject _selected;

    private void Awake()
    {
        playerAchievements = PlayerAchievements.instance;
        playerSavedData = PlayerSavedData.instance;
        achievementPrefabs = new List<AchievementUIElement>(GetComponentsInChildren<AchievementUIElement>());
    }

    private void OnEnable()
    {
        UpdateAchievements();
        UpdateStats();
    }

    private void Update()
    {
        UpdateInfoBox();
    }

    public void UpdateInfoBox()
    {
        if(_selected == eventSystem.currentSelectedGameObject)
        {
            return;
        }

        var selected = eventSystem.currentSelectedGameObject.GetComponent<AchievementUIElement>();
        if (selected != null)
        {
            _selected = selected.gameObject;
            descriptionText.text = eventSystem.currentSelectedGameObject.GetComponent<AchievementUIElement>().description;
        }
    }

    public void UpdateAchievements()
    {
        for (int i = 0; i < achievementPrefabs.Count; i++)
        {
            achievementPrefabs[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < playerAchievements.achievements.Count; i++)
        {
            achievementPrefabs[i].SetAchievement(playerAchievements.achievements[i]);
            achievementPrefabs[i].gameObject.SetActive(true);
            achievementPrefabs[i].icon.sprite = icons[i];
            achievementPrefabs[i].description = playerAchievements.achievements[i].description;
        }
    }

    public void UpdateStats()
    {
        statPrefabs[0].statText.text = playerSavedData._gameStats.totalKills.ToString();
        statPrefabs[2].statText.text = playerSavedData._gameStats.shotgunKills.ToString();
        statPrefabs[3].statText.text = playerSavedData._gameStats.flamerKills.ToString();
        statPrefabs[4].statText.text = playerSavedData._gameStats.lightningKills.ToString();
        statPrefabs[5].statText.text = playerSavedData._gameStats.cryoKills.ToString();
        statPrefabs[6].statText.text = playerSavedData._gameStats.grenadeKills.ToString();
        statPrefabs[1].statText.text = playerSavedData._gameStats.minigunKills.ToString();
    }
}
