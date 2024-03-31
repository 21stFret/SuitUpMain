using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public CrawlerSpawner crawlerSpawner;
    public GameUI gameUI;
    public int killCount;
    public PlayerInput playerInput;
    public MechLoadOut mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;
    public ManualWeaponController altWeaponController;
    public GameStats gameStats;

    private void Awake()
    {
        // Create a singleton instance
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        DelayedStart();
    }

    public void DelayedStart()
    {
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
        mechLoadOut.Init();
        crawlerSpawner.isActive = true;
        AudioManager.instance.PlayMusic(1);
        killCount = 0;
    }

    public void ReachedWaveNumber(int waveNumber)
    {
        if (waveNumber > PlayerSavedData.instance._highScore)
        {
            PlayerSavedData.instance._highScore = waveNumber;
        }
        if (waveNumber >= 10)
        {
            //PlayerAchievements.instance.SetAchievement("WAVE_10", true);
            crawlerSpawner.isActive = false;
        }
    }

    public void CheckActiveEnemies()
    {
        if (crawlerSpawner.activeCrawlerCount == 0)
        {
            // do logic when all enemies are dead
            if (crawlerSpawner.spawnRound < crawlerSpawner.spawnRoundMax)
            {
            }
            else
            {
               // Mission Complete
               CompleteMission();
            }
        }
    }

    public void UpdateKillCount(int count, WeaponType weapon)
    {
        killCount += count;
        gameUI.UpdateKillCount(killCount);
        if(killCount > PlayerSavedData.instance._killCount)
        {
            PlayerSavedData.instance._killCount = killCount;
        }
        CheckPlayerAchievements(count, weapon);
        CheckActiveEnemies();
    }

    private void CheckPlayerAchievements(int count, WeaponType weapon)
    {
        if (PlayerAchievements.instance == null)
        {
            return;
        }
        if (killCount >= 100)
        {
            if (PlayerAchievements.instance.GetAchievement("KILL_100").id != null)
            {
                PlayerAchievements.instance.SetAchievement("KILL_100", true);
            }
        }
        switch (weapon)
        {
            case WeaponType.Minigun:
                gameStats.minigunKills += count;
                if (gameStats.minigunKills >= 100)
                {
                    PlayerAchievements.instance.SetAchievement("MINIGUN_100", true);
                }
                break;
            case WeaponType.Shotgun:
                gameStats.shotgunKills += count;
                if (gameStats.shotgunKills >= 100)
                {
                    PlayerAchievements.instance.SetAchievement("SHOTGUN_100", true);
                }
                break;
            case WeaponType.Flamer:
                gameStats.flamerKills += count;
                if (gameStats.flamerKills >= 100)
                {
                    PlayerAchievements.instance.SetAchievement("FLAMER_100", true);
                }
                break;
            case WeaponType.Lightning:
                gameStats.lightningKills += count;
                if (gameStats.lightningKills >= 100)
                {
                    PlayerAchievements.instance.SetAchievement("LIGHTNING_100", true);
                }
                break;
            case WeaponType.Cryo:
                gameStats.cryoKills += count;
                if (gameStats.cryoKills >= 100)
                {
                    PlayerAchievements.instance.SetAchievement("CRYO_100", true);
                }
                break;
            case WeaponType.Grenade:
                gameStats.grenadeKills += count;
                if (gameStats.grenadeKills >= 100)
                {
                    PlayerAchievements.instance.SetAchievement("GRENADE_100", true);
                }
                break;
        }
    }

    public void SwapPlayerInput(string inputMap)
    {
        playerInput.SwitchCurrentActionMap(inputMap);
    }

    public void GameOver()
    {
        EndGame();
        crawlerSpawner.isActive = false;
        gameUI.ShowGameOverPanel();
        SwapPlayerInput("UI");
    }

    private void CompleteMission()
    {
        print("Mission Complete!");
        EndGame();
        crawlerSpawner.isActive = false;
        gameUI.ShowCompletePanel();
        SwapPlayerInput("UI");
    }

    public void LoadMainMenu()
    {
        altWeaponController.ClearWeaponInputs();
        SceneLoader.instance.LoadScene(1);
    }

    private void EndGame()
    {
        PlayerSavedData.instance.SavePlayerData();
    }
}