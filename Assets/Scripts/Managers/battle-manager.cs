using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public CrawlerSpawner crawlerSpawner;
    public List<Battle> Battles = new List<Battle>();
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
        var type = Battles[currentBattleIndex].battleType;
        GameManager.instance.gameUI.objectiveUI.ResetObjective();
        Color color = Color.white;
        float fillAmount = 0;
        switch (type)
        {
            case BattleType.Kill:
                objectiveMessage = "Hunt down and kill the target";
                color = Color.red;
                fillAmount = 1;
                SpawnRunner();
                break;
            case BattleType.Defend:
                objectiveMessage = "Defend the base for 1 minute";
                color = Color.yellow;
                fillAmount = 0;
                SpawnDefendBase();
                break;
            case BattleType.Capture:
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
        }
        GameManager.instance.gameUI.objectiveUI.objectiveBar.color = color;
        GameManager.instance.gameUI.objectiveUI.objectiveBar.fillAmount = fillAmount;
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
        crawlerSpawner.battleRound = 0;
        crawlerSpawner.roundTimer = 5;
        crawlerSpawner.waveText.text = "Here they come...";
        crawlerSpawner.battleManager = Battles[currentBattleIndex];
        crawlerSpawner.battleRoundMax = Battles[currentBattleIndex].battleWaves.Count;
        crawlerSpawner.isActive = true;
        crawlerSpawner.spawnPoints = GameManager.instance.areaManager.currentRoom.GetComponentInChildren<EnvironmentArea>().spawnPoints;
    }

    public IEnumerator CheckActiveEnemies()
    {
        if (crawlerSpawner.battleRound >= crawlerSpawner.battleRoundMax)
        {
            if (crawlerSpawner.activeCrawlerCount == 0)
            {
                yield return new WaitForSeconds(1);

                if (crawlerSpawner.activeCrawlerCount == 0)
                {
                    ObjectiveComplete();
                }
            }
        }
    }
}