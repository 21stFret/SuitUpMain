using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerBurstSpawner : MonoBehaviour
{
    public CrawlerSpawner crawlerSpawner;
    public Battle currentBattle;
    public float burstTimer;
    public int burstSpawnAmount;
    public float timeElapsed;
    public float totalTimeElapsesd;
    public CrawlerSquad crawlerSquad;
    public bool isActive;

    public Vector3 spawnPosition;

    public void Init()
    {
        timeElapsed = 0f;
        totalTimeElapsesd = 0f;
        isActive = true;
        currentBattle = crawlerSpawner.battleManager.currentBattle;
    }

    private void Update()
    {
        if (isActive)
        {
            BurstTimer();
        }
    }


    private void BurstTimer()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= burstTimer)
        {
            timeElapsed = 0f;
            int min = Mathf.RoundToInt(currentBattle.burstMin + totalTimeElapsesd);
            min = Mathf.Clamp(min, currentBattle.burstMin, currentBattle.burstMax);
            burstSpawnAmount = Random.Range(min, currentBattle.burstMax);
            spawnPosition = transform.position;
            spawnPosition.y +=1;
            var newSquad = new List<Crawler>();
            foreach (CrawlerGroup grouop in crawlerSquad.crawlerGroups)
            {
                newSquad.AddRange(crawlerSpawner.GenerateNewSquad(grouop.type, grouop.amount));
            }
            if(newSquad.Count > 0)
            {
                crawlerSpawner.SpawnFromArmy(newSquad, spawnPosition);
            }
            else
            {
                print("No army in list");
            }

        }
    }
}
