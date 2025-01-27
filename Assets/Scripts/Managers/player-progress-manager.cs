using UnityEngine;
using System.Collections;

public class PlayerProgressManager : MonoBehaviour
{
    public static PlayerProgressManager instance;
    public int killCount;
    public int expCount;
    public int cashCount;
    public int artifactCount;
    public int crawlerParts;
    public float playTime;
    public float rewardMultiplier;

    private PlayerSavedData playerSavedData;
    private int mutliShotKillCount;
    private bool triggerShotMultiKill;
    private int mutliKillCount;
    private bool triggerMultiKill;

    private void Awake()
    {
        if (instance == null) instance = this;
        playerSavedData = PlayerSavedData.instance;
    }

    public void ReachedWaveNumber(int waveNumber)
    {
        if (waveNumber > playerSavedData._gameStats.highestWave)
        {
            playerSavedData._gameStats.highestWave = waveNumber;
        }
    }

    public void UpdateKillCount(int count, WeaponType weapon)
    {
        killCount += count;
        GameManager.instance.gameUI.UpdateKillCount(killCount);
        CheckPlayerAchievements(count, weapon);
    }

    public void AddExp(int count)
    {
        expCount += count;
    }

    private void CheckShotMultiKill()
    {
        mutliShotKillCount++;

        if (!triggerShotMultiKill)
        {
            triggerShotMultiKill = true;
            StartCoroutine(ResetShotMultiKill());
        }

        if (mutliShotKillCount == 3)
            PlayerAchievements.instance.SetAchievement("SHOTGUN_HIT_3");
        if (mutliShotKillCount == 5)
            PlayerAchievements.instance.SetAchievement("SHOTGUN_HIT_5");
        if (mutliShotKillCount == 10)
            PlayerAchievements.instance.SetAchievement("SHOTGUN_HIT_10");
    }

    private IEnumerator ResetShotMultiKill()
    {
        yield return new WaitForSeconds(0.1f);
        mutliShotKillCount = 0;
        triggerShotMultiKill = false;
    }

    private void CheckMultiKill()
    {
        mutliKillCount++;

        if (!triggerMultiKill)
        {
            triggerMultiKill = true;
            StartCoroutine(ResetMultiKill());
        }

        if (mutliKillCount == 15)
            PlayerAchievements.instance.SetAchievement("MULTIKILL_15");
        if (mutliKillCount == 20)
            PlayerAchievements.instance.SetAchievement("MULTIKILL_20");
        if (mutliKillCount == 30)
            PlayerAchievements.instance.SetAchievement("MULTIKILL_30");
    }

    private IEnumerator ResetMultiKill()
    {
        yield return new WaitForSeconds(0.1f);
        mutliKillCount = 0;
        triggerMultiKill = false;
    }

    private void CheckPlayerAchievements(int count, WeaponType weapon)
    {
        if (PlayerAchievements.instance == null)
        {
            Debug.Log("No Achievements");
            return;
        }

        switch (weapon)
        {
            case WeaponType.Minigun:
                UpdateMinigunAchievements(count);
                break;
            case WeaponType.Shotgun:
                UpdateShotgunAchievements(count);
                break;
            case WeaponType.Flame:
                UpdateFlameAchievements(count);
                break;
            case WeaponType.Lightning:
                UpdateLightningAchievements(count);
                break;
            case WeaponType.Cryo:
                UpdateCryoAchievements(count);
                break;
            case WeaponType.AoE:
                UpdateGrenadeAchievements(count);
                break;
        }
    }

    private void UpdateMinigunAchievements(int count)
    {
        playerSavedData._gameStats.minigunKills += count;

        if (playerSavedData._gameStats.minigunKills == 50)
            PlayerAchievements.instance.SetAchievement("MINIGUN_50");
        if (playerSavedData._gameStats.minigunKills == 500)
            PlayerAchievements.instance.SetAchievement("MINIGUN_500");
        if (playerSavedData._gameStats.minigunKills == 2000)
            PlayerAchievements.instance.SetAchievement("MINIGUN_2000");
    }

    private void UpdateShotgunAchievements(int count)
    {
        playerSavedData._gameStats.shotgunKills += count;
        CheckShotMultiKill();
    }

    private void UpdateFlameAchievements(int count)
    {
        playerSavedData._gameStats.flamerKills += count;
        if (playerSavedData._gameStats.flamerKills == 100)
            PlayerAchievements.instance.SetAchievement("BURN_100");
        if (playerSavedData._gameStats.flamerKills == 500)
            PlayerAchievements.instance.SetAchievement("BURN_500");
        if (playerSavedData._gameStats.flamerKills == 1000)
            PlayerAchievements.instance.SetAchievement("BURN_1000");
    }

    private void UpdateLightningAchievements(int count)
    {
        playerSavedData._gameStats.lightningKills += count;
        if (playerSavedData._gameStats.lightningKills == 50)
            PlayerAchievements.instance.SetAchievement("SHOCK_50");
        if (playerSavedData._gameStats.lightningKills == 200)
            PlayerAchievements.instance.SetAchievement("SHOCK_200");
        if (playerSavedData._gameStats.lightningKills == 500)
            PlayerAchievements.instance.SetAchievement("SHOCK_500");
    }

    private void UpdateCryoAchievements(int count)
    {
        playerSavedData._gameStats.cryoKills += count;
        if (playerSavedData._gameStats.cryoKills == 25)
            PlayerAchievements.instance.SetAchievement("FREEZE_25");
        if (playerSavedData._gameStats.cryoKills == 50)
            PlayerAchievements.instance.SetAchievement("FREEZE_50");
        if (playerSavedData._gameStats.cryoKills == 100)
            PlayerAchievements.instance.SetAchievement("FREEZE_100");
    }

    private void UpdateGrenadeAchievements(int count)
    {
        playerSavedData._gameStats.grenadeKills += count;
        if (playerSavedData._gameStats.grenadeKills == 50)
            PlayerAchievements.instance.SetAchievement("GRENADE_50");
        if (playerSavedData._gameStats.grenadeKills == 250)
            PlayerAchievements.instance.SetAchievement("GRENADE_250");
        if (playerSavedData._gameStats.grenadeKills == 500)
            PlayerAchievements.instance.SetAchievement("GRENADE_500");
        CheckMultiKill();
    }



    private void UpdatePlayerStats()
    {
        playerSavedData.UpdatePlayerCash(cashCount);
        playerSavedData.UpdatePlayerExp(expCount);
        playerSavedData.UpdatePlayerArtifact(artifactCount);
        playerSavedData._gameStats.totalKills += killCount;
        playerSavedData._gameStats.totalPlayTime += playTime;
        playerSavedData._gameStats.totalParts += crawlerParts;
        playerSavedData._gameStats.totalDistance += GameManager.instance.mechLoadOut.GetComponent<MYCharacterController>().distanceTravelled;
    }

    private void EndGameAchievements()
    {
        CheckPlayTimeAchievements();
        CheckTotalPartsAchievements();
        CheckTotalDistanceAchievements();
        CheckTotalKillsAchievements();
    }

    private void CheckPlayTimeAchievements()
    {
        if (playerSavedData._gameStats.totalPlayTime > 3600)
            PlayerAchievements.instance.SetAchievement("PLAY_TIME_60");
        if (playerSavedData._gameStats.totalPlayTime > 10800)
            PlayerAchievements.instance.SetAchievement("PLAY_TIME_180");
        if (playerSavedData._gameStats.totalPlayTime > 25200)
            PlayerAchievements.instance.SetAchievement("PLAY_TIME_420");
    }

    private void CheckTotalPartsAchievements()
    {
        if (playerSavedData._gameStats.totalParts > 100)
            PlayerAchievements.instance.SetAchievement("ABLOOD_100");
        if (playerSavedData._gameStats.totalParts > 500)
            PlayerAchievements.instance.SetAchievement("ABLOOD_500");
        if (playerSavedData._gameStats.totalParts > 1000)
            PlayerAchievements.instance.SetAchievement("ABLOOD_1000");
    }

    private void CheckTotalDistanceAchievements()
    {
        if (playerSavedData._gameStats.totalDistance > 1000)
            PlayerAchievements.instance.SetAchievement("DISTANCE_1000");
        if (playerSavedData._gameStats.totalDistance > 10000)
            PlayerAchievements.instance.SetAchievement("DISTANCE_10000");
        if (playerSavedData._gameStats.totalDistance > 42195)
            PlayerAchievements.instance.SetAchievement("DISTANCE_42195");
    }

    private void CheckTotalKillsAchievements()
    {
        int totalKills = playerSavedData._gameStats.totalKills;
        if (totalKills >= 10000)
            PlayerAchievements.instance.SetAchievement("KILL_10000");
        else if (totalKills >= 5000)
            PlayerAchievements.instance.SetAchievement("KILL_5000");
        else if (totalKills >= 1000)
            PlayerAchievements.instance.SetAchievement("KILL_1000");
        else if (totalKills >= 100)
            PlayerAchievements.instance.SetAchievement("KILL_100");
    }

    public void EndGamePlayerProgress(bool won, int difficultyLevel)
    {
        playTime = Time.timeSinceLevelLoad;
        if (won)
        {
            rewardMultiplier = GetRewardMultiplierByDifficulty(difficultyLevel);
            cashCount = Mathf.CeilToInt(cashCount * rewardMultiplier);
        }
        else
        {
            rewardMultiplier = 0.5f;
        }
        UpdatePlayerStats();
        if (PlayerAchievements.instance != null)
        {
            EndGameAchievements();
        }
        playerSavedData.SavePlayerData();
    }

    private float GetRewardMultiplierByDifficulty(int difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case 1: return 1.0f; // Easy
            case 2: return 1.5f; // Medium
            case 3: return 2.0f; // Hard
            default: return 1.0f; // Default to Easy
        }
    }
}