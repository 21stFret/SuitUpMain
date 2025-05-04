using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public CrawlerSpawner crawlerSpawner;
    public List<Battle> Battles = new List<Battle>();
    public Battle currentBattle;
    public int currentBattleIndex;
    public CapturePoint capturePoint;
    public Pickup roomDrop;
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

        _objectiveComplete = false;

        if (_gameManager.currentAreaType == AreaType.Jungle && 
            currentBattleIndex == Battles.Count - 1)
        {
            _usingBattleType = BattleType.Hunt;
        }

        Color color = Color.white;
        float fillAmount = 0;
        bool showBar = false;
        bool showpercent = false;
        bool showSurvive = false;
        string showDestroy = "";
        var type = _usingBattleType;
        GenerateNewBattle(type);
        switch (type)
        {
            case BattleType.Hunt:
                objectiveMessage = "Take out the target!";
                color = Color.red;
                fillAmount = 1;
                AudioManager.instance.PlayBossMusic(1);
                StartCoroutine(HuntBattle());
                break;
            case BattleType.Upload:
                objectiveMessage = "Locate the drop and upload the data";
                color = Color.cyan;
                fillAmount = 0;
                showBar = true;
                showpercent = true;
                SpawnCapturePoint();
                break;
            case BattleType.Survive:
                objectiveMessage = "Survive the horde!";
                color = Color.green;
                fillAmount = 0;
                surviveTimeT = surviveTime;
                showBar = true;
                StartCoroutine(SurviveBattle());
                AudioManager.instance.PlayBossMusic(0);
                showSurvive = true;
                break;
            case BattleType.Exterminate:
                objectiveMessage = "Exterminate all enemies!";
                //color = Color.white;
                fillAmount = 0;
                break;
        }
        _gameManager.gameUI.objectiveUI.Init(showBar, showpercent, showSurvive, showDestroy);
        GameManager.instance.gameUI.objectiveUI.objectiveBar.color = color;
        _gameManager.gameUI.objectiveUI.objectiveBar.fillAmount = fillAmount;
    }

    private Crawler GetHuntedTarget()
    {
        return crawlerSpawner.huntedTarget;
    }

    private IEnumerator HuntBattle()
    {
        _gameManager.areaManager.DayNightCycle(true);
        lightningController.active = true;
        yield return new WaitForSeconds(0.5f);
        _gameManager.gameUI.objectiveUI.Init(true, false, false, GetHuntedTarget().name);
        while (crawlerSpawner.huntedTarget != null && GetHuntedTarget()._targetHealth.health > 0)
        {
            _gameManager.gameUI.objectiveUI.UpdateBar(GetHuntedTarget()._targetHealth.health / GetHuntedTarget()._targetHealth.maxHealth);
            yield return null;
        }
        ObjectiveComplete();
        crawlerSpawner.KillAllCrawlers();
        crawlerSpawner.EndBattle();
        lightningController.active = false;
        _gameManager.areaManager.DayNightCycle(false);
        GameUI.instance.objectiveUI.HideObjectivePanel();
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
        if (crawlerSpawner.activeCrawlerCount == 0)
        {
            ObjectiveComplete();
            GameUI.instance.StartCoroutine(GameUI.instance.objectiveUI.ObjectiveComplete());
        }
        else
        {
            GameUI.instance.objectiveUI.UpdateObjective("Finish them off!");
            _usingBattleType = BattleType.Exterminate;
        }
        crawlerSpawner.EndBattle();
        lightningController.active = false;
        _gameManager.areaManager.DayNightCycle(false);
        GameUI.instance.objectiveUI.HideObjectivePanel();
    }

    private void SpawnRunner()
    {
        crawlerSpawner.runner.Init();
        crawlerSpawner.runner.Spawn();
    }

    private void SpawnCapturePoint()
    {
        Vector3 pos = Random.insideUnitSphere * 20;
        pos.y = 1;
        pos += _gameManager.playerInput.transform.position;
        capturePoint.transform.position = pos;
        Invoke("InitCapturePoint", 1);
    }

    public void ResetOnNewArea()
    {
        currentBattleIndex = 0;
    }

    private void InitCapturePoint()
    {
        //  called by invoke in spawnCapturePoint
        capturePoint.Init();
    }

    public void ObjectiveComplete()
    {

        if(_objectiveComplete)
        {
            return;
        }
        _objectiveComplete = true;
        StartCoroutine(_gameManager.gameUI.objectiveUI.ObjectiveComplete());
        crawlerSpawner.EndBattle();
        lightningController.active = false;
        currentBattleIndex++;
        _gameManager.gameActive = false;
        AudioManager.instance.PlayBGMusic(5);
        roomDrop.gameObject.SetActive(true);
        SetPickUpPosition();

        if (currentBattleIndex >= Battles.Count)
        {
            if (_gameManager.currentAreaType == AreaType.Jungle)
            {
                _gameManager.EndGame(true);
                return;
            }

            roomDrop.Init(ModBuildType.UPGRADE);
        }
        else
        {
            roomDrop.Init(_gameManager.nextBuildtoLoad);
        }
        // add secondary room drop for upgrade when doing mini boss as to not break flow of the player chosen portal
    }

    public void ObjectiveFailed()
    {
        crawlerSpawner.EndBattle();
    }

    private void SetPickUpPosition()
    {
        Vector3 pos;
        pos = Vector3.zero;
        pos.y = 3;
        roomDrop.transform.position = pos;
        return;
    }

    public void UpdateCrawlerSpawner()
    {
        crawlerSpawner.battleManager = this;
        crawlerSpawner.waveText.text = "Here they come...";
        
        if(GameManager.instance.areaManager.currentRoom != null)
        {
            EnvironmentArea area = GameManager.instance.areaManager.currentRoom.GetComponentInChildren<EnvironmentArea>();
            crawlerSpawner.spawnPoints = area.spawnPoints;
        }
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
    private void GenerateNewBattle(BattleType type)
    {
        if (armyGen == null)
        {
            Debug.LogError("WaveGenerator is not assigned!");
            return;
        }
        currentBattle = Battles[currentBattleIndex];
        armyGen.LoadAllSquadsFromExcel();
        currentBattle.battleArmy = armyGen.BuildArmy();
    }
}