using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Linq;

public class CrawlerSpawner : MonoBehaviour
{
    [Header("Object Pools")]
    [SerializeField] private List<Crawler> crawlers = new List<Crawler>();
    [SerializeField] private List<Crawler> crawlerDaddy =  new List<Crawler>();
    [SerializeField] private List<Crawler> Daddycrawlers =  new List<Crawler>();
    [SerializeField] private List<Crawler> albinos =  new List<Crawler>();
    [SerializeField] private List<Crawler> spitters =  new List<Crawler>();
    [SerializeField] private List<Crawler> chargers =  new List<Crawler>();
    [SerializeField] private List<Crawler> activeCrawlers = new List<Crawler>();
    public int activeCrawlerCount { get { return activeCrawlers.Count; } }
    public List<Transform> spawnPoints;
    [Header("Spawn Settings")]
    private Transform spawnPoint;
    private int currentSpawnPoint;
    public List<PortalEffect> portalEffects;
    [SerializeField] public float roundTimer;
    public int battleRound;
    public int battleRoundMax;
    public bool endless;
    private bool maxRoundReached;
    public float timeElapsed = 0f;
    public TMP_Text timeText;
    public TMP_Text waveText;

    public static CrawlerSpawner instance;
    public Battle battleManager;
    public BattleWave currentWave;

    public bool isActive;

    public int portalMaxAllowed;

    public List<Crawler> spawnList = new List<Crawler>();

    public CrawlerSpitter runner;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitCrawlers();
    }

    private void LateUpdate()
    {
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
        if (timeElapsed <= roundTimer - 10f)
        {
            timeText.enabled = false;
        }
        else
        {
            timeText.enabled = true;
        }
        if(timeElapsed >= roundTimer - 5f)
        {
            timeText.color = Color.red;
        }
        else
        {
            timeText.color = Color.white;
        }
        timeText.text = ((roundTimer - timeElapsed)).ToString("0");
    }

    private void InitCrawlers()
    {
        spawnPoint = spawnPoints[0];
        foreach (Crawler crawler in crawlers)
        {
            crawler.Init();
            crawler.crawlerSpawner = this;
            crawler.crawlerType = CrawlerType.Crawler;
        }
        foreach (CrawlerDaddy daddy in crawlerDaddy)
        {
            daddy.Init();
            daddy.crawlerSpawner = this;
            daddy.crawlerType = CrawlerType.Daddy;
        }
        foreach (Crawler daddyCrawler in Daddycrawlers)
        {
            daddyCrawler.Init();
            daddyCrawler.crawlerSpawner = this;
            daddyCrawler.crawlerType = CrawlerType.DaddyCrawler;

        }
        foreach (Crawler albino in albinos)
        {
            albino.Init();
            albino.crawlerSpawner = this;
            albino.crawlerType = CrawlerType.Albino;
        }
        foreach (Crawler spitter in spitters)
        {
            spitter.Init();
            spitter.crawlerSpawner = this;
            spitter.crawlerType = CrawlerType.Spitter;
        }
        foreach (Crawler charger in chargers)
        {
            charger.Init();
            charger.crawlerSpawner = this;
            charger.crawlerType = CrawlerType.Charger;
        }
    }

    private void BeginSpawning()
    {
        if(!isActive)
        {
            return;
        }

        SelectSpawnPoint();
        IncrementSpawnRound();
        StartCoroutine(SpawnDelay());
    }

    private void IncrementSpawnRound()
    {
        battleRound++;

        if (GameManager.instance != null)
        {
            GameManager.instance.ReachedWaveNumber(battleRound);
        }

        currentWave = battleManager.battleWaves[battleRound-1];
        waveText.text = "Wave " + (battleRound).ToString() + "/" + battleManager.battleWaves.Count.ToString();
        roundTimer = currentWave.roundTimer;

        if (battleRound >= battleRoundMax)
        {
            maxRoundReached = true;
            if (endless)
            {
               battleRound = 0;
            }
            else
            {
                battleRound = battleRoundMax;
                isActive = false;
                timeText.text = "";
                waveText.text = "Final Wave";
            }
        }
    }

    private void SelectSpawnPoint()
    {
        int randomSpawnPoint = Random.Range(0, spawnPoints.Count);
        if (randomSpawnPoint == currentSpawnPoint)
        {
            SelectSpawnPoint();
            return;
        }
        spawnPoint = spawnPoints[randomSpawnPoint];
    }

    private IEnumerator SpawnDelay()
    {
        PlaySpawnEffect(0);
        yield return new WaitForSeconds(1f);

        if(isActive)
        {
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        var waveCrawlers = currentWave.crawlersInWave;
        spawnList.Clear();
        for (int i = 0; i < waveCrawlers.Length; i++)
        {
            //SpawnCrawler(waveCrawlers[i].type, waveCrawlers[i].count);
            for (int j = 0; j < waveCrawlers[i].count; j++)
            {
                var list = new List<Crawler>();
                switch (waveCrawlers[i].type)
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
                    case CrawlerType.Charger:
                        list = chargers;
                        break;
                }
                spawnList.Add(list[j]);
            }
        }
        spawnList.Shuffle();
        SpawnCrawlerFromList();
    }

    private void SpawnCrawlerFromList()
    {
        if (spawnList.Count <= 0)
        {
            print("No Crawlers to Spawn");
            return;
        }
        int portalIndex = 0;
        int potalAllowed = 0;
        for (int i = 0; i < spawnList.Count; i++)
        {
            if (potalAllowed >= portalMaxAllowed)
            {
                portalIndex++;
                SelectSpawnPoint();
                PlaySpawnEffect(portalIndex);
                potalAllowed = 0;

            }
            Vector3 randomCircle = Random.insideUnitSphere;
            randomCircle.z = 0;
            Vector3 randomPoint = randomCircle + spawnPoint.position;
            spawnList[i].transform.position = randomPoint;
            spawnList[i].transform.rotation = spawnPoint.rotation * Quaternion.Euler(0, randomCircle.y, 0);
            StartCoroutine(SpawnRandomizer(spawnList[i], i * 0.2f));
            potalAllowed++;
        }
    }

    private IEnumerator SpawnRandomizer(Crawler bug, float delay)
    {
        AddToActiveList(bug);
        yield return new WaitForSeconds(delay);
        bug.Spawn();
    }

    private void PlaySpawnEffect(int index)
    {
        if (portalEffects[index].isActive)
        {
            index++;
            if (index >= portalEffects.Count)
            {
                index = 0;
            }
            PlaySpawnEffect(index);
            return;
        }
        portalEffects[index].transform.position = spawnPoint.position;
        portalEffects[index].transform.rotation = spawnPoint.rotation;
        portalEffects[index].StartEffect();
    }

    public void SpawnAtPoint(Vector3 point, int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 randomCircle = Random.insideUnitSphere;
            randomCircle.y = 1;
            var point1 = point + randomCircle;
            Daddycrawlers[0].transform.position = point1;
            Daddycrawlers[0].transform.rotation = Quaternion.identity;
            StartCoroutine(SpawnRandomizer(Daddycrawlers[0], i * 0.1f));
        }
    }

    public void EndBattle()
    { 
        isActive = false;
        KillAllCrawlers();
    }

    public void KillAllCrawlers()
    {
        for (int i = 0; i < activeCrawlers.Count; i++)
        {
            activeCrawlers[i].DealyedDamage(1000, 0.2f, WeaponType.Default);
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
            case CrawlerType.DaddyCrawler:
                Daddycrawlers.Add(crawler);
                break;
            case CrawlerType.Albino:
                albinos.Add(crawler);
                break;
            case CrawlerType.Spitter:
                spitters.Add(crawler);
                break;
            case CrawlerType.Charger:
                chargers.Add(crawler);
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
            case CrawlerType.DaddyCrawler:
                Daddycrawlers.Remove(crawler);
                break;
            case CrawlerType.Albino:
                albinos.Remove(crawler);
                break;
            case CrawlerType.Spitter:
                spitters.Remove(crawler);
                break;
            case CrawlerType.Charger:
                chargers.Remove(crawler);
                break;
        }
        activeCrawlers.Add(crawler);
    }
}