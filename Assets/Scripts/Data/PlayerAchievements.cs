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

        List<Dictionary<string, object>> data = CSVReader.Read("Achievements");
        for(var i = 0; i < data.Count; i++)
        {
            Achievement achievement = new Achievement();
            achievement.id = data[i]["id"].ToString();
            achievement.name = data[i]["name"].ToString();
            achievement.description = data[i]["description"].ToString();
            achievement.achieved = false;
            achievements.Add(achievement);
        }
    
    
    }

    public void SetAcheivementFromSteam()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }

        foreach (Achievement achievement in achievements)
        {
            SetAchievement(achievement.id, steamAchievements.CheckAchievement(achievement.id));
        }
    }

    public Achievement GetAchievement(string id)
    {
        Achievement achievement = achievements.Find(a => a.id == id);
        if(achievement.id == null)
        {
            print ("Achievement not found");
        }
        else
        {
            print("Achievement found: " + achievement.id);
        }
        return achievement;
    }

    public void SetAchievement(string id, bool achieved)
    {
        Achievement achievement = achievements.Find(a => a.id == id);
        if(achievement.id == null)
        {
            print ("Achievement not found");
            return;
        }
        print("Setting " + achievement.id + " to " + achieved);
        achievement.achieved = achieved;
        achievements[achievements.FindIndex(a => a.id == id)] = achievement;
    }

    public void SetAchievementByName(string idName, bool achieved)
    {
        Achievement achievement = achievements.Find(a => a.name == idName);
        achievement.achieved = achieved;
        achievements[achievements.FindIndex(a => a.name == idName)] = achievement;
    }

    public void UpdateSteamAchievements(string id)
    {
        steamAchievements.UnlockAchievement(id);
    }

}
