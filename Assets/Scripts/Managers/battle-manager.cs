using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FORGE3D;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public CrawlerSpawner crawlerSpawner;
    public List<Battle> Battles = new List<Battle>();
    public Battle currentBattle;
    public int currentBattleIndex;
    public CapturePoint capturePoint;
    public Pickup roomDrop;
    public HealthPickup healthPickup;
    public LayerMask dropLayer;
    public string objectiveMessage;
    public float dificultyMultiplier = 1;
    public float surviveTime=60;
    private float surviveTimeT;
    public LightningController lightningController;
    public BattleType _usingBattleType;

    [InspectorButton("ObjectiveComplete")]
    public bool setBattleType;

    [InspectorButton ("SetBattleType")]
    public bool updateBattle;

    private GameManager _gameManager;

    private bool _objectiveComplete;

    public Coroutine currentBattleCoroutine;

    private void Awake()
    {
        if (instance == null) instance = this;
        surviveTime *= dificultyMultiplier;
    }

    public void SetBattleType()
    {
        _gameManager = GameManager.instance;
        currentBattle = Battles[currentBattleIndex];
        _usingBattleType = currentBattle.battleType;
        if (currentBattleCoroutine != null)
        {
            StopCoroutine(currentBattleCoroutine);
        }
        currentBattleCoroutine = null;

        _objectiveComplete = false;

        if (_gameManager.currentAreaType == AreaType.Jungle &&
            currentBattleIndex == Battles.Count - 1)
        {
            _usingBattleType = BattleType.Hunt;
        }

        if (_gameManager.currentAreaType == AreaType.Desert &&
        currentBattleIndex == Battles.Count - 1)
        {
            _usingBattleType = BattleType.MiniBoss;
        }
        
        Color color = Color.white;
        float fillAmount = 0;
        bool showBar = false;
        bool showpercent = false;
        bool showSurvive = false;
        string showDestroy = "";
        var type = _usingBattleType;
        GenerateNewBattle();
        bool skip = false;
        switch (type)
        {
            case BattleType.Hunt:
                objectiveMessage = "Take out the target!";
                color = Color.red;
                fillAmount = 1;
                AudioManager.instance.PlayBossMusic(1);
                currentBattleCoroutine = StartCoroutine(HuntBattle());
                break;
            case BattleType.Upload:
                objectiveMessage = "Locate the drop and upload the data";
                color = Color.cyan;
                fillAmount = 0;
                showBar = true;
                showpercent = true;
                SpawnCapturePoint();
                _gameManager.areaManager.DayNightCycle(true);
                break;
            case BattleType.Survive:
                objectiveMessage = "Survive the horde!";
                color = Color.green;
                fillAmount = 0;
                surviveTimeT = surviveTime;
                showBar = true;
                currentBattleCoroutine = StartCoroutine(SurviveBattle());
                AudioManager.instance.PlayBossMusic(0);
                showSurvive = true;
                break;
            case BattleType.Exterminate:
                objectiveMessage = "Exterminate all enemies!";
                _gameManager.areaManager.DayNightCycle(true);
                //color = Color.white;
                fillAmount = 0;
                break;
            case BattleType.MiniBoss:
                objectiveMessage = "Defeat the Centipedes!";
                AudioManager.instance.PlayBossMusic(2);
                CentipideBossManager.instance.StartBossFight();
                color = Color.red;
                fillAmount = 1;
                AudioManager.instance.PlayBossMusic(1);
                _gameManager.areaManager.DayNightCycle(true);
                _gameManager.gameUI.objectiveUI.Init(true, false, false, "Centipede Boss");
                GameManager.instance.gameUI.objectiveUI.objectiveBar.color = color;
                _gameManager.gameUI.objectiveUI.objectiveBar.fillAmount = fillAmount;
                skip = true;
                break;
        }
        if (skip) return;
        _gameManager.gameUI.objectiveUI.Init(showBar, showpercent, showSurvive, showDestroy);
        GameManager.instance.gameUI.objectiveUI.objectiveBar.color = color;
        _gameManager.gameUI.objectiveUI.objectiveBar.fillAmount = fillAmount;
    }
    
    private IEnumerator HuntBattle()
    {
        _gameManager.areaManager.DayNightCycle(true);
        lightningController.active = true;
        yield return new WaitForSeconds(0.5f);
        _gameManager.gameUI.objectiveUI.Init(true, false, false, "Mutant Crawler");
        var hunted = crawlerSpawner.huntedTarget;
        while (hunted != null && hunted._targetHealth.health > 0)
        {
            _gameManager.gameUI.objectiveUI.UpdateBar(hunted._targetHealth.health / hunted._targetHealth.maxHealth);
            yield return null;
        }
        ObjectiveComplete();
        lightningController.active = false;
        _gameManager.areaManager.DayNightCycle(false);
    }

    private IEnumerator SurviveBattle()
    {
        lightningController.active = true;
        while (surviveTimeT > 0)
        {
            if (BattleMech.instance.isDead)
            {
                yield break;
            }
            surviveTimeT -= Time.deltaTime;
            _gameManager.gameUI.objectiveUI.UpdateBar(surviveTimeT / surviveTime);
            yield return null;
        }
        if(!_gameManager.gameActive)
        {
            yield break;
        }
        _gameManager.areaManager.missileLauncher.missilePayload = MissilePayload.FatMan;
        _gameManager.areaManager.missileLauncher.SpawnExplosion(Vector3.zero);
        PostProcessController.instance.NukeEffect();
        ObjectiveComplete();
        GameUI.instance.StartCoroutine(GameUI.instance.objectiveUI.ObjectiveComplete());
        crawlerSpawner.EndBattle();
        lightningController.active = false;
        _gameManager.areaManager.DayNightCycle(false);
        GameUI.instance.objectiveUI.HideObjectivePanel();
    }

    private void SpawnCapturePoint()
    {
        Invoke("InitCapturePoint", 1);
    }

    private Vector3 GetRandomSpawnPoint()
    {
        if (crawlerSpawner.spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points available for BattleManager!");
            return Vector3.zero;
        }
        int randomIndex = Random.Range(0, crawlerSpawner.spawnPoints.Count);
        UpdateSpawnPoints();
        return crawlerSpawner.spawnPoints[randomIndex].transform.position;
    }

    public void ResetOnNewArea()
    {
        currentBattleIndex = 0;
    }

    private void InitCapturePoint()
    {
        //  called by invoke in spawnCapturePoint
        Vector3 pos = GetRandomSpawnPoint();
        Vector3 directionToCenter = (Vector3.zero - pos).normalized;
        float randomDistance = Random.Range(5f, 20f);
        Vector3 spawnPoint = pos + (directionToCenter * randomDistance);
        spawnPoint.y = 1;
        capturePoint.transform.position = spawnPoint;
        capturePoint.Init();
    }

    public void ObjectiveComplete()
    {
        if (_objectiveComplete)
        {
            return;
        }
        _objectiveComplete = true;
        StartCoroutine(_gameManager.gameUI.objectiveUI.ObjectiveComplete());
        crawlerSpawner.EndBattle();
        if (capturePoint != null && !capturePoint.isCaptured)
        {
            capturePoint.ProcessCapture();
        }
        lightningController.active = false;

        SetPickUpPosition();

        bool chipreward = currentBattleIndex % 2 != 1;

        currentBattleIndex++;
        _gameManager.gameActive = false;
        AudioManager.instance.PlayBGMusic(5);

        if (currentBattleIndex >= Battles.Count)
        {
            if (_gameManager.currentAreaType == AreaType.Jungle)
            {
                _gameManager.EndGame(true);
                return;
            }
            roomDrop.Init(_gameManager.nextBuildtoLoad, true);
        }
        else
        {
            roomDrop.Init(_gameManager.nextBuildtoLoad);
        }

        healthPickup.Init();
        if (chipreward) // If the index is odd, we show the room drop
        {
            roomDrop.gameObject.SetActive(true);
            healthPickup.RemovePickup();
        }
        else
        {
            roomDrop.gameObject.SetActive(false);
            healthPickup.ResetPickup();
        }

        GameUI.instance.objectiveUI.HideObjectivePanel();
    }

    private void SetPickUpPosition()
    {
        Vector3 pos;
        pos = crawlerSpawner.spawnPoints[0].transform.position;
        pos.y = 3;
        roomDrop.transform.position = pos;
        healthPickup.transform.position = pos;
        return;
    }

    public void UpdateCrawlerSpawner()
    {
        crawlerSpawner.battleManager = this;
        crawlerSpawner.waveText.text = "Here they come...";

        UpdateSpawnPoints();
        crawlerSpawner.LoadBattle();
        if (_usingBattleType == BattleType.Exterminate)
        {
            crawlerSpawner.BeginSpawningSquads();
        }
        if (_usingBattleType == BattleType.Hunt)
        {
            crawlerSpawner.SpawnBoss();
        }
    }

    private void UpdateSpawnPoints()
    {
        if (GameManager.instance.areaManager.currentRoom != null)
        {
            print("Updating spawn points for CrawlerSpawner");
            EnvironmentArea area = GameManager.instance.areaManager.currentRoom.GetComponentInChildren<EnvironmentArea>();
            crawlerSpawner.spawnPoints = area.spawnPoints;
        }
        else
        {
            Debug.LogError("No current room found in AreaManager!");
        }
    }
    
    private bool _isCoroutineRunning = false;
    public IEnumerator CheckActiveEnemies()
    {
        if (_isCoroutineRunning) yield break;
        _isCoroutineRunning = true;
        if (crawlerSpawner.activeCrawlerCount == 0)
        {
            yield return new WaitForSeconds(1);
            if (BattleMech.instance.isDead)
            {
                yield break;
            }
            if (crawlerSpawner.activeCrawlerCount == 0)
            {
                if (crawlerSpawner.battleRound < crawlerSpawner.battleRoundMax)
                {
                    crawlerSpawner.BeginSpawningSquads();
                }
                else
                {
                    ObjectiveComplete();
                }
            }
        }
        _isCoroutineRunning = false;
    }

    [SerializeField] public ArmyGenerator armyGen;
    private void GenerateNewBattle()
    {
        if (armyGen == null)
        {
            Debug.LogError("WaveGenerator is not assigned!");
            return;
        }
        currentBattle = Battles[currentBattleIndex];
        armyGen.LoadAllSquadsFromExcel();
        armyGen.currentBattleType = _usingBattleType;
        currentBattle.battleArmy = armyGen.BuildArmy();
    }
}