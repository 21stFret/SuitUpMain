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
    public int expCount;
    public int cashCount;
    public int artifactCount;
    public PlayerInput playerInput;
    public MechLoadOut mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;
    public ManualWeaponController altWeaponController;
    private PlayerSavedData playerSavedData;

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
        playerSavedData = PlayerSavedData.instance;
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(playerSavedData._mainWeaponData, playerSavedData._altWeaponData);
        mechLoadOut.Init();
        crawlerSpawner.isActive = true;
        AudioManager.instance.PlayMusic(1);
        killCount = 0;
        expCount = 0;
        cashCount = 0;
    }

    public void ReachedWaveNumber(int waveNumber)
    {
        if (waveNumber > playerSavedData._gameStats.highestWave)
        {
            playerSavedData._gameStats.highestWave = waveNumber;
        }
    }

    public void CheckActiveEnemies()
    {
        if (crawlerSpawner.activeCrawlerCount == 0)
        {
            // do logic when all enemies are dead in final round
            if (crawlerSpawner.spawnRound == crawlerSpawner.spawnRoundMax)
            {
                // Mission Complete
                EndGame(true);
            }
        }
    }

    public void UpdateKillCount(int count, WeaponType weapon)
    {
        killCount += count;
        gameUI.UpdateKillCount(killCount);
        CheckPlayerAchievements(count, weapon);
        CheckActiveEnemies();
    }

    public void AddExp(int count)
    {
        expCount += count;
    }

    private void CheckPlayerAchievements(int count, WeaponType weapon)
    {
        if (PlayerAchievements.instance == null)
        {
            return;
        }
        int totalKills = playerSavedData._gameStats.totalKills + killCount;
        if (totalKills > 100)
        {
            if (PlayerAchievements.instance.GetAchievement("KILL_100").id != null)
            {
                PlayerAchievements.instance.SetAchievement("KILL_100", true);
            }
        }
        switch (weapon)
        {
            case WeaponType.Minigun:
                playerSavedData._gameStats.minigunKills += count;
                if (playerSavedData._gameStats.minigunKills >= 100)
                {
                    PlayerAchievements.instance.SetAchievement("MINIGUN_100", true);
                }
                break;
            case WeaponType.Shotgun:
                playerSavedData._gameStats.shotgunKills += count;
                if (playerSavedData._gameStats.shotgunKills >= 100)
                {
                    PlayerAchievements.instance.SetAchievement("SHOTGUN_100", true);
                }
                break;
            case WeaponType.Flame:
                playerSavedData._gameStats.flamerKills += count;
                if (playerSavedData._gameStats.flamerKills >= 100)
                {
                    PlayerAchievements.instance.SetAchievement("FLAMER_100", true);
                }
                break;
            case WeaponType.Lightning:
                playerSavedData._gameStats.lightningKills += count;
                if (playerSavedData._gameStats.lightningKills >= 100)
                {
                    PlayerAchievements.instance.SetAchievement("LIGHTNING_100", true);
                }
                break;
            case WeaponType.Cryo:
                playerSavedData._gameStats.cryoKills += count;
                if (playerSavedData._gameStats.cryoKills >= 100)
                {
                    PlayerAchievements.instance.SetAchievement("CRYO_100", true);
                }
                break;
            case WeaponType.Grenade:
                playerSavedData._gameStats.grenadeKills += count;
                if (playerSavedData._gameStats.grenadeKills >= 100)
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

    public void LoadMainMenu()
    {
        altWeaponController.ClearWeaponInputs();
        SceneLoader.instance.LoadScene(1);
    }

    public void EndGame(bool won)
    {
        crawlerSpawner.isActive = false;
        gameUI.ShowEndGamePanel(won);
        SwapPlayerInput("UI");
        playerSavedData.UpdatePlayerCash(cashCount);
        playerSavedData.UpdatePlayerExp(expCount);
        playerSavedData.UpdatePlayerArtifact(artifactCount);
        playerSavedData.SavePlayerData();
    }
}