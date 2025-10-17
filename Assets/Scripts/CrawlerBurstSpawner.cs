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
    public int squadCount;
    public Transform spawnPosition;
    [HideInInspector]
    public List<Crawler> _activeCrawlers = new List<Crawler>();
    public int maxCrawlers = 10;

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
            var newSquad = new List<Crawler>();
            for (int i = 0; i < squadCount && i < crawlerSquad.crawlerGroups.Length; i++)
            {
                var group = crawlerSquad.crawlerGroups[i];
                newSquad.AddRange(crawlerSpawner.GenerateNewSquad(group.type, group.amount));
            }
            if (newSquad.Count > 0)
            {
                crawlerSpawner.SpawnFromArmy(newSquad, spawnPosition);
                _activeCrawlers.AddRange(newSquad);
                foreach (var crawler in newSquad)
                {
                    crawler._burstSpawner = this;
                }
            }
        }
    }
}
