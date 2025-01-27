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

    private void Init()
    {
        playerAchievements = PlayerAchievements.instance;
        playerSavedData = PlayerSavedData.instance;
        achievementPrefabs = new List<AchievementUIElement>(GetComponentsInChildren<AchievementUIElement>());
    }

    private void OnEnable()
    {
        Init();
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

        var selectedObject = eventSystem.currentSelectedGameObject;
        if (selectedObject != null)
        {
            var selected = selectedObject.GetComponent<AchievementUIElement>();
            if (selected == null)
            {
                return;
            }
            _selected = selected.gameObject;
            descriptionText.text = eventSystem.currentSelectedGameObject.GetComponent<AchievementUIElement>().description;
        }
    }

    public void UpdateAchievements()
    {
        if (playerAchievements == null)
        {
            return;
        }

        for (int i = 0; i < achievementPrefabs.Count; i++)
        {
            achievementPrefabs[i].gameObject.SetActive(false);
        }

        playerAchievements.SetAcheivementFromSteam();

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
        statPrefabs[0].statText.text = "Total Kills";
        statPrefabs[1].statText.text = "Minigun Kills";
        statPrefabs[2].statText.text = "Shotgun Kills";
        statPrefabs[3].statText.text = "Flamer Kills";
        statPrefabs[4].statText.text = "Lightning Kills";
        statPrefabs[5].statText.text = "Cryo Kills";
        statPrefabs[6].statText.text = "Grenade Kills";
        statPrefabs[7].statText.text = "Laser Kills";
        statPrefabs[8].statText.text = "Total Deaths";
        statPrefabs[9].statText.text = "Highest Wave";
        statPrefabs[10].statText.text = "Total Elites";
        statPrefabs[11].statText.text = "Total Bosses";
        statPrefabs[12].statText.text = "Total Play Time";
        statPrefabs[13].statText.text = "Total Upgrades";
        statPrefabs[14].statText.text = "Total Parts";
        statPrefabs[15].statText.text = "Total Distance";



        statPrefabs[0].value.text = playerSavedData._gameStats.totalKills.ToString();
        statPrefabs[1].value.text = playerSavedData._gameStats.minigunKills.ToString();
        statPrefabs[2].value.text = playerSavedData._gameStats.shotgunKills.ToString();
        statPrefabs[3].value.text = playerSavedData._gameStats.flamerKills.ToString();
        statPrefabs[4].value.text = playerSavedData._gameStats.lightningKills.ToString();
        statPrefabs[5].value.text = playerSavedData._gameStats.cryoKills.ToString();
        statPrefabs[6].value.text = playerSavedData._gameStats.grenadeKills.ToString();
        statPrefabs[7].value.text = playerSavedData._gameStats.laserKills.ToString();
        statPrefabs[8].value.text = playerSavedData._gameStats.totalDeaths.ToString();
        statPrefabs[9].value.text = playerSavedData._gameStats.highestWave.ToString();
        statPrefabs[10].value.text = playerSavedData._gameStats.totalElites.ToString();
        statPrefabs[11].value.text = playerSavedData._gameStats.totalBosses.ToString();
        statPrefabs[12].value.text = playerSavedData._gameStats.totalPlayTime.ToString();
        statPrefabs[13].value.text = playerSavedData._gameStats.totalUpgrades.ToString();
        statPrefabs[14].value.text = playerSavedData._gameStats.totalParts.ToString();
        statPrefabs[15].value.text = playerSavedData._gameStats.totalDistance.ToString();

        


    }
}
