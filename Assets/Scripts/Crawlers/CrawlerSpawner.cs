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
    public int spawnRound;
    public int spawnRoundMax;

    public float timeElapsed = 0f;
    public TMP_Text timeText;
    public TMP_Text waveText;

    public static CrawlerSpawner instance;
    public WaveManager waveManager;
    public BattleWave currentWave;

    public bool isActive;

    public int portalMaxAllowed;

    public List<Crawler> spawnList = new List<Crawler>();

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
        foreach (Crawler charger in chargers)
        {
            charger.Init();
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
        spawnRound++;

        if (GameManager.instance != null)
        {
            GameManager.instance.ReachedWaveNumber(spawnRound);
        }

        currentWave = waveManager.battleWaves[spawnRound-1];
        waveText.text = "Wave " + (spawnRound).ToString() + "/" + waveManager.battleWaves.Count.ToString();
        roundTimer = currentWave.roundTimer;

        if (spawnRound >= spawnRoundMax)
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
        SpawnWave();
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
                print("Max Crawlers Reached for this portal");
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
            AddToActiveList(spawnList[i]);
            potalAllowed++;
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
            case CrawlerType.Charger:
                list = chargers;
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
        int portalIndex = 0;
        int potalAllowed = 0;
        for (int i = 0; i < amount; i++)
        {
            if(potalAllowed >= portalMaxAllowed)
            {         
                print("Max Crawlers Reached for this portal");
                SelectSpawnPoint();
                PlaySpawnEffect(portalIndex);
                potalAllowed = 0;
                portalIndex++;
            }
            Vector3 randomCircle = Random.insideUnitSphere;
            randomCircle.z = 0;
            Vector3 randomPoint = randomCircle+ spawnPoint.position;
            list[0].transform.position = randomPoint;
            list[0].transform.rotation = spawnPoint.rotation * Quaternion.Euler(0, randomCircle.y, 0);
            StartCoroutine(SpawnRandomizer(list[0], i * 0.2f));
            AddToActiveList(list[0]);
            potalAllowed++;
        }

    }

    private IEnumerator SpawnRandomizer(Crawler bug, float delay)
    {
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