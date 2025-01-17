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

    [InspectorButton("ObjectiveComplete")]
    public bool setBattleType;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void SetBattleType()
    {
        GenerateNewBattle(Battles[currentBattleIndex].battleType);

        Color color = Color.white;
        float fillAmount = 0;
        bool showBar = false;
        var type = Battles[currentBattleIndex].battleType;
        switch (type)
        {
            case BattleType.Hunt:
                objectiveMessage = "Hunt down and kill the target";
                color = Color.red;
                fillAmount = 1;
                SpawnRunner();
                break;
            case BattleType.Upload:
                objectiveMessage = "Locate the drop and upload the data";
                color = Color.cyan;
                fillAmount = 0;
                showBar = true;
                SpawnCapturePoint();
                break;
            case BattleType.Survive:
                objectiveMessage = "Survive all waves!";
                color = Color.green;
                fillAmount = 0;
                break;
            case BattleType.Exterminate:
                objectiveMessage = "Exterminate all enemies!";
                color = Color.white;
                fillAmount = 0;
                break;
        }
        GameManager.instance.gameUI.objectiveUI.Init(showBar);
        GameManager.instance.gameUI.objectiveUI.objectiveBar.color = color;
        GameManager.instance.gameUI.objectiveUI.objectiveBar.fillAmount = fillAmount;
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
        pos += GameManager.instance.playerInput.transform.position;
        capturePoint.transform.position = pos;
        Invoke("InitCapturePoint", 2);
    }

    public void ResetOnNewArea()
    {
        currentBattleIndex = 0;
    }

    private void InitCapturePoint()
    {
        capturePoint.Init();
    }

    public void ObjectiveComplete()
    {
        StartCoroutine(GameManager.instance.gameUI.objectiveUI.ObjectiveComplete());
        crawlerSpawner.EndBattle();
        currentBattleIndex++;

        if (currentBattleIndex >= Battles.Count - 1 && GameManager.instance.currentAreaType==AreaType.Jungle)
        {
            GameManager.instance.EndGame(true);
            return;
        }
        GameManager.instance.gameActive = false;
        AudioManager.instance.PlayMusic(5);
        SetPickUpPosition();
        roomDrop.gameObject.SetActive(true);
        roomDrop.Init(GameManager.instance.nextBuildtoLoad);
    }

    public void ObjectiveFailed()
    {
        crawlerSpawner.EndBattle();
    }

    private void SetPickUpPosition(float minDistance = 5f, float maxDistance = 15f, int maxAttempts = 10)
    {
        Vector3 playerPos = GameManager.instance.playerInput.transform.position;
        Vector3 pos;
        int attempts = 0;

        do
        {
            // Generate a random position within a spherical shell
            Vector3 randomDirection = Random.insideUnitSphere.normalized;
            float randomDistance = Random.Range(minDistance, maxDistance);
            pos = playerPos + randomDirection * randomDistance;
            pos.y = 3;

            attempts++;

            if(Vector3.Distance(playerPos, pos) < minDistance) continue;
            if(Vector3.Distance(Vector3.zero, pos) > 50) continue;

            // Check if the position is valid (not obstructed)
            Collider[] colliders = Physics.OverlapSphere(pos, 1, dropLayer);

            if (colliders.Length==0)
            {
                roomDrop.transform.position = pos;
                return;
            }
        } while (attempts < maxAttempts);

        // If we've exceeded max attempts, fall back to a position at minDistance
        print("Failed to find a valid drop position, falling back to map centre");
        pos = Vector3.zero;
        pos.y = 3;
        roomDrop.transform.position = pos;
    }

    public void UpdateCrawlerSpawner()
    {
        crawlerSpawner.battleManager = this;
        crawlerSpawner.waveText.text = "Here they come...";
        EnvironmentArea area = GameManager.instance.areaManager.currentRoom.GetComponentInChildren<EnvironmentArea>();
        crawlerSpawner.spawnPoints = area.spawnPoints;
        area.RefreshArea();
        crawlerSpawner.LoadBattle();
        if (currentBattle.battleType == BattleType.Exterminate)
        {
            crawlerSpawner.BeginSpawningSquads();
        }
    }

    public IEnumerator CheckActiveEnemies()
    {
        if (BattleMech.instance.isDead)
        {
            yield break;
        }
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
    }

    [SerializeField] private ArmyGenerator ArmyGen;
    private void GenerateNewBattle(BattleType type)
    {
        if (ArmyGen == null)
        {
            Debug.LogError("WaveGenerator is not assigned!");
            return;
        }
        currentBattle = Battles[currentBattleIndex];
        ArmyGen.LoadAllSquadsFromExcel();
        currentBattle.battleArmy = ArmyGen.BuildArmy();
    }
}