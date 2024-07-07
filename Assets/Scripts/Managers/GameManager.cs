using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Text.RegularExpressions;

public enum BattleType
{
    Kill,
    Defend,
    Capture,
    Survive
}

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
    public float playTime;
    public PlayerInput playerInput;
    public MechLoader mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;
    public ManualWeaponController altWeaponController;
    private PlayerSavedData playerSavedData;
    public List<GameObject> rooms = new List<GameObject>();
    public GameObject voidArea;
    public VoidPortalManager voidPortalManager;
    public List<Battle> Battles = new List<Battle>();
    public RoomPortal RoomPortal;
    public Pickup roomDrop;
    public int currentRoomIndex;
    public int currentBattleIndex;
    public bool endlessMode;
    public bool gameActive;
    private bool dayTime;
    public GameObject dayLight;
    public GameObject nightLight;
    public CapturePoint capturePoint;
    public DefendObjective defendBase;

    private int mutliShotKillCount;
    private bool triggerShotMultiKill; 
    private int mutliKillCount;
    private bool triggerMultiKill;
    public float rewardMultiplier;

    public bool playOnAwake = false;

    [InspectorButton("SpawnPortalsToNextRoom")]
    public bool spawnPortal;

    public ModBuildType nextBuildtoLoad;

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
        currentRoomIndex = 0;
        LoadRoomRandom();
        playerSavedData = PlayerSavedData.instance;
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(playerSavedData._mainWeaponData, playerSavedData._altWeaponData);
        mechLoadOut.Init();
        nextBuildtoLoad = (ModBuildType)Random.Range(0, 4);

        if (!playOnAwake)
        {
            return;
        }

        gameActive = true;
        killCount = 0;
        expCount = 0;
        cashCount = 0;
        dayTime = true;
        SetBattleType();
        currentBattleIndex = 0;
        UpdateCrawlerSpawner();
        AudioManager.instance.PlayMusic(1);
    }

    private IEnumerator ShowControls()
    {
        yield return new WaitForSeconds(1);
        gameUI.pauseMenu.PauseGame();
        gameUI.pauseMenu.menu.SetActive(false);
        gameUI.pauseMenu.controlsMenu.SetActive(true);
        gameUI.eventSystem.SetSelectedGameObject(gameUI.pauseMenu.controlsSelectedButton);
        gameUI.pauseMenu.SwapControlsMenu();
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
        if (crawlerSpawner.battleRound >= crawlerSpawner.battleRoundMax)
        {
            if (crawlerSpawner.activeCrawlerCount == 0)
            {
                yield return new WaitForSeconds(1);

                if (crawlerSpawner.activeCrawlerCount == 0)
                {
                    /*
                    if (endlessMode)
                    {
                        SpawnPortalToNextRoom();
                        yield break;
                    }
                    */

                    ObjectiveComplete();
                }
            }
        }
    }

    public void SpawnPortalsToNextRoom()
    {
        //show sign to portal
        crawlerSpawner.waveText.text = "Head through the Portal!";
        voidPortalManager.StartEffect();
    }

    public IEnumerator LoadVoidRoom()
    {
        RoomPortal.portalEffect.StopEffect();   
        yield return new WaitForSeconds(2);
        rooms[currentRoomIndex].SetActive(false);
        voidArea.SetActive(true);
        yield return new WaitForSeconds(1);
        RoomPortal.visualPortalEffect.StopFirstPersonEffect();
        yield return new WaitForSeconds(1);
        gameUI.gameUIFade.FadeIn();
    }

    public void UpdateKillCount(int count, WeaponType weapon)
    {
        killCount += count;
        gameUI.UpdateKillCount(killCount);
        CheckPlayerAchievements(count, weapon);

        if(Battles[currentBattleIndex].battleType == BattleType.Survive)
        {
            StartCoroutine(CheckActiveEnemies());
        }

    }

    public void AddExp(int count)
    {
        expCount += count;
    }

    private void CheckShotMultiKill()
    {
        mutliShotKillCount++;

        if(!triggerShotMultiKill)
        {
            triggerMultiKill = true;
            StartCoroutine(ResetShotMultiKill());
        }

        if(mutliShotKillCount == 3)
        {
            PlayerAchievements.instance.SetAchievement("SHOTGUN_HIT_3");
            print("Killed 3 at once");
        }
        if (mutliShotKillCount == 5)
        {
            PlayerAchievements.instance.SetAchievement("SHOTGUN_HIT_5");
            print("Killed 5 at once");
        }
        if (mutliShotKillCount == 10)
        {
            PlayerAchievements.instance.SetAchievement("SHOTGUN_HIT_10");
            print("Killed 10 at once");
        }
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
        {
            PlayerAchievements.instance.SetAchievement("MULTIKILL_15");
            print("Killed 3 at once");
        }
        if (mutliKillCount == 20)
        {
            PlayerAchievements.instance.SetAchievement("MULTIKILL_20");
            print("Killed 5 at once");
        }
        if (mutliKillCount == 30)
        {
            PlayerAchievements.instance.SetAchievement("MULTIKILL_30");
            print("Killed 10 at once");
        }
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
            print("No Achievements");
            return;
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
            case WeaponType.Shotgun:
                playerSavedData._gameStats.shotgunKills += count;
                CheckShotMultiKill();
                break;
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
                if (playerSavedData._gameStats.cryoKills == 25)
                {
                    PlayerAchievements.instance.SetAchievement("FREEZE_25");
                }
                if (playerSavedData._gameStats.cryoKills == 50)
                {
                    PlayerAchievements.instance.SetAchievement("FREEZE_50");
                }
                if (playerSavedData._gameStats.cryoKills == 100)
                {
                    PlayerAchievements.instance.SetAchievement("FREEZE_100");
                }
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
                CheckMultiKill();
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

    public IEnumerator DelayedLoadNextRoom()
    {
        gameUI.gameUIFade.FadeOut();
        yield return new WaitForSeconds(2);
        CashCollector.instance.DestroyParts();
        currentBattleIndex++;
        voidArea.SetActive(false);
        LoadRoomRandom();
        SetBattleType();
        UpdateCrawlerSpawner();
        DayNightCycle();
        roomDrop.gameObject.SetActive(false);
        //RoomPortal.portalEffect.StopEffect();
        yield return new WaitForSeconds(1);
        RoomPortal.visualPortalEffect.StopFirstPersonEffect();
        yield return new WaitForSeconds(1);
        gameUI.gameUIFade.FadeIn();

    }

    public void LoadNextRoom()
    {
        StartCoroutine(DelayedLoadNextRoom());
    }

    private void LoadRoomRandom()
    {
        rooms[currentRoomIndex].SetActive(false);
        currentRoomIndex = Random.Range(0, rooms.Count);
        if (currentRoomIndex == rooms.Count)
        {
            currentRoomIndex = 0;
        }
        rooms[currentRoomIndex].SetActive(true);

        ProceduralLevelGeneration levelGen = rooms[currentRoomIndex].GetComponent<ProceduralLevelGeneration>();
        if (levelGen != null)
        {
            levelGen.GenerateLevel();
        }
    }

    private void SetBattleType()
    {
        var type = Battles[currentBattleIndex].battleType;
        gameUI.objectiveUI.ResetObjective();
        switch (type)
        {
            case BattleType.Kill:
                crawlerSpawner.endless = true;
                gameUI.objectiveUI.UpdateObjective("Hunt down and kill the target");
                gameUI.objectiveUI.objectiveBar.color = Color.red;
                gameUI.objectiveUI.objectiveBar.fillAmount = 1;
                SpawnRunner();
                break;
            case BattleType.Defend:
                crawlerSpawner.endless = true;
                gameUI.objectiveUI.UpdateObjective("Defend the base for 1 minute");
                gameUI.objectiveUI.objectiveBar.color = Color.yellow;
                gameUI.objectiveUI.objectiveBar.fillAmount = 0;
                SpawnDefendBase();
                break;
            case BattleType.Capture:
                crawlerSpawner.endless = true;
                gameUI.objectiveUI.UpdateObjective("Locate the drop and upload the data");
                gameUI.objectiveUI.objectiveBar.color = Color.blue;
                gameUI.objectiveUI.objectiveBar.fillAmount = 0;
                SpawnCapturePoint();
                break;
            case BattleType.Survive:
                crawlerSpawner.endless = false;
                gameUI.objectiveUI.UpdateObjective("Survive all waves!");
                gameUI.objectiveUI.objectiveBar.color = Color.green;
                gameUI.objectiveUI.objectiveBar.fillAmount = 0;
                break;
        }
    }

    private void SpawnDefendBase()
    {
        defendBase.gameObject.SetActive(true);
        defendBase.Init();
    }

    private void SpawnRunner()
    {
        crawlerSpawner.runner.Init();
        crawlerSpawner.runner.Spawn();
    }

    private void SpawnCapturePoint()
    {
        Vector3 pos = Random.insideUnitSphere * 50;
        pos.y = 1;
        capturePoint.transform.position = pos;
        capturePoint.gameObject.SetActive(true);
        capturePoint.Init();
    }

    public void ObjectiveComplete(bool complete = true)
    {
        StartCoroutine(gameUI.objectiveUI.ObjectiveComplete(complete));
        crawlerSpawner.EndBattle();

        if (currentBattleIndex == Battles.Count - 1)
        {
            EndGame(true);
            return;
        }

        crawlerSpawner.waveText.text = "Collect your reward!";
        roomDrop.gameObject.SetActive(true);
        roomDrop.Init(nextBuildtoLoad);
    }



    private void DayNightCycle()
    {
        float random = Random.Range(0, 100);
        if (random < 50)
        {
            dayTime = true;
        }
        else
        {
            dayTime = false;
        }

        if (dayTime)
        {
            dayLight.SetActive(true);
            nightLight.SetActive(false);
        }
        else
        {
            nightLight.SetActive(true);
            dayLight.SetActive(false);
        }
    }

    public void UpdateCrawlerSpawner()
    {
        crawlerSpawner.battleRound = 0;
        crawlerSpawner.roundTimer = 5;
        crawlerSpawner.waveText.text = "Here they come...";
        crawlerSpawner.battleManager = Battles[currentBattleIndex];
        crawlerSpawner.battleRoundMax = Battles[currentBattleIndex].battleWaves.Count;
        crawlerSpawner.isActive = true;
        crawlerSpawner.spawnPoints = rooms[currentRoomIndex].GetComponent<EnvironmentArea>().spawnPoints;
    }

    private void EndGame(bool won)
    {
        AudioManager.instance.PlayMusic(3);
        playTime = Time.timeSinceLevelLoad;
        gameActive = false;
        crawlerSpawner.isActive = false;
        if (won)
        {
            rewardMultiplier = 1.5f;
            cashCount = Mathf.CeilToInt(cashCount * rewardMultiplier);
        }
        else
        { 
            rewardMultiplier = 1; 
        }
        gameUI.ShowEndGamePanel(won);
        SwapPlayerInput("UI");
        playerSavedData.UpdatePlayerCash(cashCount);
        playerSavedData.UpdatePlayerExp(expCount);
        playerSavedData.UpdatePlayerArtifact(artifactCount);
        playerSavedData._gameStats.totalKills += killCount;
        playerSavedData._gameStats.totalPlayTime += playTime;
        playerSavedData._gameStats.totalParts += crawlerParts;
        playerSavedData._gameStats.totalDistance += mechLoadOut.GetComponent<MYCharacterController>().distanceTravelled;

        if(PlayerAchievements.instance != null)
        {
            EndGameAchievements();
        }

        playerSavedData.SavePlayerData();
    }

    private void EndGameAchievements()
    {
        if (playerSavedData._gameStats.totalPlayTime > 3600)
        {
            PlayerAchievements.instance.SetAchievement("PLAY_TIME_60");
        }
        if (playerSavedData._gameStats.totalPlayTime > 10800)
        {
            PlayerAchievements.instance.SetAchievement("PLAY_TIME_180");
        }
        if (playerSavedData._gameStats.totalPlayTime > 25200)
        {
            PlayerAchievements.instance.SetAchievement("PLAY_TIME_420");
        }

        if (playerSavedData._gameStats.totalParts > 100)
        {
            PlayerAchievements.instance.SetAchievement("ABLOOD_100");
        }
        if (playerSavedData._gameStats.totalParts > 500)
        {
            PlayerAchievements.instance.SetAchievement("ABLOOD_500");
        }
        if (playerSavedData._gameStats.totalParts > 1000)
        {
            PlayerAchievements.instance.SetAchievement("ABLOOD_1000");
        }

        if(playerSavedData._gameStats.totalDistance > 1000)
        {
            PlayerAchievements.instance.SetAchievement("DISTANCE_1000");
        }
        if (playerSavedData._gameStats.totalDistance > 10000)
        {
            PlayerAchievements.instance.SetAchievement("DISTANCE_10000");
        }
        if (playerSavedData._gameStats.totalDistance > 42195)
        {
            PlayerAchievements.instance.SetAchievement("DISTANCE_42195");
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