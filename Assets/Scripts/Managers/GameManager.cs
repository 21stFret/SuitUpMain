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
    public int crawlerParts;
    public PlayerInput playerInput;
    public MechLoadOut mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;
    public ManualWeaponController altWeaponController;
    private PlayerSavedData playerSavedData;
    public List<GameObject> rooms = new List<GameObject>();
    public List<RoomWaves> roomWaves = new List<RoomWaves>();
    public RoomPortal RoomPortal;
    public Pickup roomDrop;
    public int currentRoomIndex;
    public bool endlessMode;
    public bool gameActive;



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
        if (SetupGame.instance != null)
        {
            SetupGame.instance.LinkGameManager(this);
        }
        else
        {
            Invoke("DelayedStart", 0.1f);
        }
    }

    public void DelayedStart()
    {
        gameActive = true;
        playerSavedData = PlayerSavedData.instance;
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(playerSavedData._mainWeaponData, playerSavedData._altWeaponData);
        mechLoadOut.Init();
        UpdateCrawlerSpawner();
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

    public IEnumerator CheckActiveEnemies()
    {
        // do logic when all enemies are dead in final round
        if (crawlerSpawner.spawnRound >= crawlerSpawner.spawnRoundMax)
        {
            if (crawlerSpawner.activeCrawlerCount == 0)
            {
                yield return new WaitForSeconds(1);

                if (crawlerSpawner.activeCrawlerCount == 0)
                {
                    if (endlessMode)
                    {
                        SpawnPortalToNextRoom();
                        yield break;
                    }
                    if (currentRoomIndex == rooms.Count - 1)
                    {
                        // Mission Complete
                        EndGame(true);
                    }
                    else
                    {
                        SpawnPortalToNextRoom();
                    }
                }
            }
        }
    }

    public void SpawnPortalToNextRoom()
    {
        //show sign to portal
        crawlerSpawner.waveText.text = "Head through the Portal!";
        RoomPortal.portalEffect.StartEffect();
        RoomPortal._active = true;
        roomDrop.gameObject.SetActive(true);
        roomDrop.Init();
    }

    public void UpdateKillCount(int count, WeaponType weapon)
    {
        killCount += count;
        playerSavedData._gameStats.totalKills += count;
        gameUI.UpdateKillCount(killCount);
        CheckPlayerAchievements(count, weapon);
        StartCoroutine(CheckActiveEnemies());
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

        int totalKills = playerSavedData._gameStats.totalKills;
        switch (totalKills)
        { 
            case 100:
                PlayerAchievements.instance.SetAchievement("KILL_100");
                break;
            case 1000:
                PlayerAchievements.instance.SetAchievement("KILL_1000");
                break;
            case 5000:
                PlayerAchievements.instance.SetAchievement("KILL_5000");
                break;
            case 10000:
                PlayerAchievements.instance.SetAchievement("KILL_10000");
                break;

        }
        switch (weapon)
        {
            case WeaponType.Minigun:
                playerSavedData._gameStats.minigunKills += count;

                if (playerSavedData._gameStats.minigunKills == 50)
                {
                    PlayerAchievements.instance.SetAchievement("MINIGUN_50");
                }
                if (playerSavedData._gameStats.minigunKills == 500)
                {
                    PlayerAchievements.instance.SetAchievement("MINIGUN_500");
                }
                if (playerSavedData._gameStats.minigunKills == 2000)
                {
                    PlayerAchievements.instance.SetAchievement("MINIGUN_2000");
                }
                break;
                /*
            case WeaponType.Shotgun:
                playerSavedData._gameStats.shotgunKills += count;
                if (playerSavedData._gameStats.shotgunKills >= 100)
                {
                    PlayerAchievements.instance.SetAchievement("SHOTGUN_100", true);
                }
                break;
                */
            case WeaponType.Flame:
                playerSavedData._gameStats.flamerKills += count;
                if (playerSavedData._gameStats.flamerKills == 100)
                {
                    PlayerAchievements.instance.SetAchievement("BURN_100");
                }
                if (playerSavedData._gameStats.flamerKills == 500)
                {
                    PlayerAchievements.instance.SetAchievement("BURN_500");
                }
                if (playerSavedData._gameStats.flamerKills == 1000)
                {
                    PlayerAchievements.instance.SetAchievement("BURN_1000");
                }
                break;
            case WeaponType.Lightning:
                playerSavedData._gameStats.lightningKills += count;
                if (playerSavedData._gameStats.lightningKills == 50)
                {
                    PlayerAchievements.instance.SetAchievement("SHOCK_50");
                }
                if (playerSavedData._gameStats.lightningKills == 200)
                {
                    PlayerAchievements.instance.SetAchievement("SHOCK_200");
                }
                if (playerSavedData._gameStats.lightningKills == 500)
                {
                    PlayerAchievements.instance.SetAchievement("SHOCK_500");
                }
                break;
                
            case WeaponType.Cryo:
                playerSavedData._gameStats.cryoKills += count;
                break;
                
            case WeaponType.Grenade:
                playerSavedData._gameStats.grenadeKills += count;
                if (playerSavedData._gameStats.grenadeKills == 50)
                {
                    PlayerAchievements.instance.SetAchievement("GRENADE_50");
                }
                if (playerSavedData._gameStats.grenadeKills == 250)
                {
                    PlayerAchievements.instance.SetAchievement("GRENADE_250");
                }
                if (playerSavedData._gameStats.grenadeKills == 500)
                {
                    PlayerAchievements.instance.SetAchievement("GRENADE_500");
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

    public void LoadNextRoom()
    {
        StartCoroutine(DelayedLoadNextRoom());
    }

    private void LoadRoom()
    {
        if (currentRoomIndex == rooms.Count)
        {
            if(endlessMode)
            {
                rooms[currentRoomIndex].SetActive(false);
                currentRoomIndex = 0;
                rooms[currentRoomIndex].SetActive(true);
                return;
            }
            
            print("End Game, no more rooms");
            return;
        }
        rooms[currentRoomIndex].SetActive(false);
        currentRoomIndex++;
        rooms[currentRoomIndex].SetActive(true);
    }

    public IEnumerator DelayedLoadNextRoom()
    {
        gameUI.gameUIFade.FadeOut();
        yield return new WaitForSeconds(2);
        CashCollector.Instance.DestroyParts();
        LoadRoom();
        UpdateCrawlerSpawner();
        RoomPortal.portalEffect.StopEffect();
        yield return new WaitForSeconds(1);
        RoomPortal.visualPortalEffect.StopFirstPersonEffect();
        yield return new WaitForSeconds(1);
        gameUI.gameUIFade.FadeIn();

    }

    public void UpdateCrawlerSpawner()
    {
        crawlerSpawner.spawnRound = 0;
        crawlerSpawner.roundTimer = 5;
        crawlerSpawner.waveText.text = "Here they come...";
        crawlerSpawner.waveManager = roomWaves[currentRoomIndex];
        crawlerSpawner.isActive = true;
    }

    private void EndGame(bool won)
    {
        AudioManager.instance.PlayMusic(3);
        gameActive = false;
        crawlerSpawner.isActive = false;
        gameUI.ShowEndGamePanel(won);
        SwapPlayerInput("UI");
        playerSavedData.UpdatePlayerCash(cashCount);
        playerSavedData.UpdatePlayerExp(expCount);
        playerSavedData.UpdatePlayerArtifact(artifactCount);
        playerSavedData._gameStats.totalPlayTime += Time.timeSinceLevelLoad;
        if(playerSavedData._gameStats.totalPlayTime > 1800)
        {
            PlayerAchievements.instance.SetAchievement("PLAY_TIME_30");
        }
        if (playerSavedData._gameStats.totalPlayTime > 10800)
        {
            PlayerAchievements.instance.SetAchievement("PLAY_TIME_180");
        }
        if (playerSavedData._gameStats.totalPlayTime > 25200)
        {
            PlayerAchievements.instance.SetAchievement("PLAY_TIME_420");
        }
        playerSavedData.SavePlayerData();
    }

    private IEnumerator EndGameDelay(bool win)
    {
        yield return new WaitForSeconds(1f);
        EndGame(win);
    }

    public void EndGameCall(bool win)
    {
        StartCoroutine(EndGameDelay(win));
    }
}