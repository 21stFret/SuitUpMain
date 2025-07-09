using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

[System.Serializable]
public struct Achievement
{
    public string id;
    public string name;
    public string description;
    public bool achieved;
}

public class PlayerAchievements : MonoBehaviour
{
    public static PlayerAchievements instance;
    public SteamAchievements steamAchievements;
    public List<Achievement> achievements = new List<Achievement>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadFromExcell();
    }

    public void LoadFromExcell()
    { 
        achievements.Clear();

        List<Dictionary<string, object>> data = CSVReader.Read("Suit Up Data - Final Achievements");
        for(var i = 0; i < data.Count; i++)
        {
            Achievement achievement = new Achievement();
            achievement.id = data[i]["id"].ToString();
            achievement.name = data[i]["name"].ToString();
            achievement.description = data[i]["description"].ToString();
            achievement.achieved = false;
            achievements.Add(achievement);
        }
        SetAcheivementFromSteam();
    }

    public void SetAcheivementFromSteam()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }

        for (int i = 0; i < achievements.Count; i++)
        {
            Achievement achievement = achievements[i];
            achievement.achieved = steamAchievements.CheckAchievement(achievement.id);
            achievements[i] = achievement;
        }
    }

    public void SetAchievement(string id)
    {
        print("Achieved " + id);
        steamAchievements.UnlockAchievement(id);
    }

    public bool IsAchieved(string id)
    {
        Achievement achievement = achievements.Find(x => x.id == id);
        if (achievement.achieved)
        {
            return true;
        }
        return false;
    }

}
