using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerBurstSpawner : MonoBehaviour
{
    public CrawlerSpawner crawlerSpawner;
    public float burstTimer;
    public int burstSpawnAmount;
    public float timeElapsed;
    public float totalTimeElapsesd;
    public CrawlerSquad crawlerSquad;
    public bool isActive;

    public Transform spawnPosition;

    public void Init()
    {
        timeElapsed = 0f;
        totalTimeElapsesd = 0f;
        isActive = true;
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
            int min = Mathf.RoundToInt(crawlerSpawner.localBurstMin + totalTimeElapsesd);
            min = Mathf.Clamp(min, crawlerSpawner.localBurstMin, crawlerSpawner.localBurstMax);
            burstSpawnAmount = Random.Range(min, crawlerSpawner.localBurstMax);
            spawnPosition = transform;
            spawnPosition.position += Vector3.up;
            var newSquad = new List<Crawler>();
            foreach (CrawlerGroup group in crawlerSquad.crawlerGroups)
            {
                newSquad.AddRange(crawlerSpawner.GenerateNewSquad(group.type, group.amount));
            }
            if(newSquad.Count > 0)
            {
                crawlerSpawner.SpawnFromArmy(newSquad, spawnPosition.position);
            }
            else
            {
                print("No army in list");
            }

        }
    }
}
