using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class CrawlerSpawner : MonoBehaviour
{
    [Header("Object Pools")]
    [SerializeField] private List<Crawler> crawlers = new List<Crawler>();
    [SerializeField] private List<Crawler> crawlerDaddy =  new List<Crawler>();
    [SerializeField] private List<Crawler> albinos =  new List<Crawler>();
    [SerializeField] private List<Crawler> spitters =  new List<Crawler>();
    private List<Crawler> activeCrawlers = new List<Crawler>();
    public int activeCrawlerCount { get { return activeCrawlers.Count; } }
    [SerializeField] private List<Transform> spawnPoints;
    [Header("Spawn Settings")]
    private Transform spawnPoint;
    public PortalEffect portalEffect;
    [SerializeField] public float roundTimer;
    public int spawnRound;
    public int spawnRoundMax;

    public float timeElapsed = 0f;
    public TMP_Text timeText;
    public TMP_Text waveText;

    public static CrawlerSpawner instance;
    public RoomWaves waveManager;
    public BattleWave currentWave;

    public bool isActive;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        spawnRoundMax = waveManager.battleWaves.Count;
        InitCrawlers();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            TestSpawning();
        }
        if (!isActive)
        {
            return;
        }

        RoundTimer();
    }

    private void RoundTimer()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= roundTimer)
        {
            BeginSpawning();
            timeElapsed = 0f;
        }
        timeText.text = ((roundTimer - timeElapsed)).ToString("00:00");
    }

    private void InitCrawlers()
    {
        spawnPoint = spawnPoints[0];
        foreach (Crawler crawler in crawlers)
        {
            crawler.Init();
        }
        foreach (CrawlerDaddy daddy in crawlerDaddy)
        {
            daddy.Init();
        }
        foreach (Crawler albino in albinos)
        {
            albino.Init();
        }
        foreach (Crawler spitter in spitters)
        {
            spitter.Init();
        }
    }

    private void BeginSpawning()
    {
        if(!isActive)
        {
            return;
        }

        SelectSpawnPoint();
        StartCoroutine(SpawnDelay());
        IncrementSpawnRound();
    }

    private void IncrementSpawnRound()
    {
        spawnRound++;

        if (GameManager.instance != null)
        {
            GameManager.instance.ReachedWaveNumber(spawnRound);
        }

        currentWave = waveManager.battleWaves[spawnRound-1];
        waveText.text = "Wave " + (spawnRound).ToString();
        roundTimer = currentWave.roundTimer;

        if (spawnRound == spawnRoundMax)
        {
            print("Max Round Reached");
            isActive = false;
            timeText.text = "";
            waveText.text = "Final Wave";
        }

    }

    private void SelectSpawnPoint()
    {
        int randomSpawnPoint = Random.Range(0, spawnPoints.Count);
        spawnPoint = spawnPoints[randomSpawnPoint];
    }

    private IEnumerator SpawnDelay()
    {
        PLaySpawnEffect();
        yield return new WaitForSeconds(1f);
        SpawnWave();
    }

    public void TestSpawning()
    {
        currentWave = waveManager.battleWaves[spawnRound];
        SpawnWave();
    }

    private void SpawnWave()
    {
        var waveCrawlers = currentWave.crawlersInWave; 
        for (int i = 0; i < waveCrawlers.Length; i++)
        {
            SpawnCrawler(waveCrawlers[i].type, waveCrawlers[i].count);
        }

    }

    private void SpawnCrawler(CrawlerType crawler, int amount)
    {
        if(amount<1)
        {
            return;
        }
        var list = new List<Crawler>();
        switch (crawler)
        {
            case CrawlerType.Crawler:
                list = crawlers;
                break;
            case CrawlerType.Daddy:
                list = crawlerDaddy;
                break;
            case CrawlerType.Albino:
                list = albinos;
                break;
            case CrawlerType.Spitter:
                list = spitters;
                break;
        }

        if (amount > list.Count)
        {
            amount = list.Count;
        }
        if (amount <= 0)
        {
            print("No "+crawler+" Crawlers to Spawn");
            return;
        }
        for (int i = 0; i < amount; i++)
        {
            Vector3 randomCircle = Random.insideUnitSphere;
            randomCircle.z = 0;
            Vector3 randomPoint = randomCircle+ spawnPoint.position;
            list[0].transform.position = randomPoint;
            list[0].transform.rotation = spawnPoint.rotation * Quaternion.Euler(0, randomCircle.y, 0);
            StartCoroutine(SpawnRandomizer(list[0], i * 0.2f));
            AddToActiveList(list[0]);
        }

    }

    private IEnumerator SpawnRandomizer(Crawler bug, float delay)
    {
        yield return new WaitForSeconds(delay);
        bug.Spawn();
    }

    private void PLaySpawnEffect()
    {
        portalEffect.transform.position = spawnPoint.position;
        portalEffect.transform.rotation = spawnPoint.rotation;
        portalEffect.StartEffect();
    }

    public void SpawnAtPoint(Vector3 point, int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 randomCircle = Random.insideUnitSphere;
            randomCircle.y = 1;
            var point1 = point + randomCircle;
            crawlers[0].transform.position = point1;
            crawlers[0].transform.rotation = Quaternion.identity;
            crawlers[0].Spawn();
            AddToActiveList(crawlers[0]);
        }
    }   


    public void AddtoRespawnList(Crawler crawler, CrawlerType type)
    {
        activeCrawlers.Remove(crawler);
        switch (type)
        {
            case CrawlerType.Crawler:
                crawlers.Add(crawler);
                break;
            case CrawlerType.Daddy:
                crawlerDaddy.Add(crawler);
                break;
            case CrawlerType.Albino:
                albinos.Add(crawler);
                break;
            case CrawlerType.Spitter:
                spitters.Add(crawler);
                break;
        }
    }

    public void AddToActiveList(Crawler crawler)
    {
        switch (crawler.crawlerType)
        {
            case CrawlerType.Crawler:
                crawlers.Remove(crawler);
                break;
            case CrawlerType.Daddy:
                crawlerDaddy.Remove(crawler);
                break;
            case CrawlerType.Albino:
                albinos.Remove(crawler);
                break;
            case CrawlerType.Spitter:
                spitters.Remove(crawler);
                break;
        }
        activeCrawlers.Add(crawler);
    }
}