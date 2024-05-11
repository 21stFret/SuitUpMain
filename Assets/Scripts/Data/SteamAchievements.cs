using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamAchievements : MonoBehaviour
{
    public bool CheckAchievement(string id)
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.GetAchievement(id, out bool achieved);
            return achieved;
        }
        return false;
    }

    public void UnlockAchievement(string id)
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.SetAchievement(id);
            SteamUserStats.StoreStats();
        }
    }

    public void ResetAllAchievements()
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.ResetAllStats(true);
        }
    }

    public void SetStat(string id, int value)
    {
        if (SteamManager.Initialized)
        {
            SteamUserStats.SetStat(id, value);
            SteamUserStats.StoreStats();
        }
    }
}
