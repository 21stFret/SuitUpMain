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
    public DefendObjective defendBase;
    public Pickup roomDrop;
    public LayerMask dropLayer;
    public string objectiveMessage;

    [InspectorButton("ObjectiveComplete")]
    public bool setBattleType;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void SetBattleType()
    {
        GenerateNewBattle(Battles[currentBattleIndex].battleType);
        GameManager.instance.gameUI.objectiveUI.ResetObjective();
        Color color = Color.white;
        float fillAmount = 0;
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
                color = Color.blue;
                fillAmount = 0;
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
        Vector3 pos = Random.insideUnitSphere * 50;
        pos.y = 1;
        capturePoint.transform.position = pos;
        Invoke("InitCapturePoint", 3);
    }

    private void InitCapturePoint()
    {
        capturePoint.Init();
    }

    public void ObjectiveComplete()
    {
        StartCoroutine(GameManager.instance.gameUI.objectiveUI.ObjectiveComplete());
        crawlerSpawner.EndBattle();

        if (currentBattleIndex == Battles.Count - 1)
        {
            GameManager.instance.EndGame(true);
            return;
        }
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

            // Check if the position is valid (not obstructed)
            if (!Physics.SphereCast(pos, minDistance, Vector3.down, out RaycastHit hit, 30, dropLayer))
            {
                roomDrop.transform.position = pos;
                return;
            }
        } while (attempts < maxAttempts);

        // If we've exceeded max attempts, fall back to a position at minDistance
        Vector3 fallbackDirection = Random.insideUnitSphere.normalized;
        pos = playerPos + fallbackDirection * minDistance;
        pos.y = 1;
        roomDrop.transform.position = pos;
    }

    public void UpdateCrawlerSpawner()
    {
        crawlerSpawner.battleManager = this;
        crawlerSpawner.waveText.text = "Here they come...";
        crawlerSpawner.spawnPoints = GameManager.instance.areaManager.currentRoom.GetComponentInChildren<EnvironmentArea>().spawnPoints;
        crawlerSpawner.LoadBattle();
        if (currentBattle.battleType == BattleType.Exterminate)
        {
            crawlerSpawner.BeginSpawningSquads();
        }
    }

    public IEnumerator CheckActiveEnemies()
    {
        if (crawlerSpawner.activeCrawlerCount == 0)
        {
            yield return new WaitForSeconds(1);
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
        currentBattle.battleArmy = ArmyGen.BuildArmy();
    }
}