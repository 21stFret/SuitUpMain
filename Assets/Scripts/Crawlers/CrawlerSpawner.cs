using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Linq;

public class CrawlerSpawner : MonoBehaviour
{
    [Header("Object Pools")]
    [SerializeField] private List<Crawler> crawlers = new List<Crawler>();
    [SerializeField] private List<Crawler> crawlerDaddy = new List<Crawler>();
    [SerializeField] private List<Crawler> Daddycrawlers = new List<Crawler>();
    [SerializeField] private List<Crawler> albinos = new List<Crawler>();
    [SerializeField] private List<Crawler> spitters = new List<Crawler>();
    [SerializeField] private List<Crawler> chargers = new List<Crawler>();
    [SerializeField] private List<Crawler> activeCrawlers = new List<Crawler>();
    public int activeCrawlerCount { get { return activeCrawlers.Count; } }
    public List<Transform> spawnPoints;

    [Header("Spawn Settings")]
    private Transform spawnPoint;
    private int currentSpawnPoint;
    public List<PortalEffect> portalEffects;
    public float burstTimer;
    public int battleRound;
    public int battleRoundMax;
    public bool endless;
    public float timeElapsed = 0f;
    public TMP_Text timeText;
    public TMP_Text waveText;

    public static CrawlerSpawner instance;
    public Battle battleManager;
    public CrawlerSquad currentSquad;

    public bool isActive;

    public int portalMaxAllowed;

    public List<Crawler> spawnList = new List<Crawler>();

    public CrawlerSpitter runner;

    [SerializeField] private ArmyGenerator ArmyGen;
    public Battle currentBattle;

    private void Start()
    {
        InitCrawlers();
        GenerateNewBattle();
    }

    private void GenerateNewBattle()
    {
        if (ArmyGen == null)
        {
            Debug.LogError("WaveGenerator is not assigned to CrawlerSpawner!");
            return;
        }
        currentBattle = new Battle(BattleType.Exterminate);
        currentBattle.battleArmy = ArmyGen.BuildArmy();
        battleRoundMax = currentBattle.battleArmy.Count;
        endless = false; // Set to true if you want endless mode
        ResetBattle();
    }

    private void ResetBattle()
    {
        battleRound = 0;
        timeElapsed = 0f;
        burstTimer = currentBattle.burstTimer;
        isActive = true;
        IncrementSpawnRound();
    }


    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (isActive)
        {
            BurstTimer();

        }
        ValidateActiveCrawlers();
    }

    private void BurstTimer()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= burstTimer)
        {
            BeginSpawning();
            timeElapsed = 0f;
        }
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        if (timeElapsed <= burstTimer - 10f)
        {
            timeText.enabled = false;
        }
        else
        {
            timeText.enabled = true;
            timeText.color = timeElapsed >= burstTimer - 5f ? Color.red : Color.white;
        }
        timeText.text = (burstTimer - timeElapsed).ToString("0");
    }

    private void InitCrawlers()
    {
        InitCrawlerList(crawlers, CrawlerType.Crawler);
        InitCrawlerList(crawlerDaddy, CrawlerType.Daddy);
        InitCrawlerList(Daddycrawlers, CrawlerType.DaddyCrawler);
        InitCrawlerList(albinos, CrawlerType.Albino);
        InitCrawlerList(spitters, CrawlerType.Spitter);
        InitCrawlerList(chargers, CrawlerType.Charger);
    }

    private void InitCrawlerList(List<Crawler> crawlerList, CrawlerType type)
    {
        foreach (Crawler crawler in crawlerList)
        {
            crawler.Init();
            crawler.crawlerSpawner = this;
            crawler.crawlerType = type;
            crawler.gameObject.SetActive(false);
        }
    }

    private void BeginSpawning()
    {
        if (!isActive) return;

        SelectSpawnPoint();
        IncrementSpawnRound();
        StartCoroutine(SpawnDelay());
    }

    private void SelectSpawnPoint()
    {
        int randomSpawnPoint;
        do
        {
            randomSpawnPoint = Random.Range(0, spawnPoints.Count);
        } while (randomSpawnPoint == currentSpawnPoint);

        currentSpawnPoint = randomSpawnPoint;
        spawnPoint = spawnPoints[currentSpawnPoint];
    }

    private void IncrementSpawnRound()
    {
        battleRound++;

        if (battleRound >= battleRoundMax)
        {
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
                return;
            }
        }

        currentSquad = currentBattle.battleArmy[battleRound - 1];
        waveText.text = $"Wave {battleRound}/{currentBattle.battleArmy.Count}";
        burstTimer = currentBattle.burstTimer;
    }

    private IEnumerator SpawnDelay()
    {
        PlaySpawnEffect(currentSpawnPoint);
        yield return new WaitForSeconds(1f);

        if (isActive)
        {
            SpawnSquad();
        }
    }

    private void SpawnSquad()
    {
        spawnList.Clear();
        var waveInfo = currentSquad.crawlerGroups[battleRound];
        for (int j = 0; j < waveInfo.amount && j < GetCrawlerList(waveInfo.type).Count; j++)
        {
            spawnList.Add(GetCrawlerList(waveInfo.type)[j]);
        }
        spawnList.Shuffle();
        SpawnCrawlerFromList(spawnList);
    }    
    
    private void SpawnFromArmy()
    {
        spawnList.Clear();

        for(int i = 0; i < currentSquad.crawlerGroups.Length; i++)
        {
            var waveInfo = currentSquad.crawlerGroups[i];
            for (int j = 0; j < waveInfo.amount && j < GetCrawlerList(waveInfo.type).Count; j++)
            {
                spawnList.Add(GetCrawlerList(waveInfo.type)[j]);
            }
        }

        spawnList.Shuffle();
        SpawnCrawlerFromList(spawnList);
    }

    private List<Crawler> GetCrawlerList(CrawlerType type)
    {
        switch (type)
        {
            case CrawlerType.Crawler: return crawlers;
            case CrawlerType.Daddy: return crawlerDaddy;
            case CrawlerType.DaddyCrawler: return Daddycrawlers;
            case CrawlerType.Albino: return albinos;
            case CrawlerType.Spitter: return spitters;
            case CrawlerType.Charger: return chargers;
            default: return new List<Crawler>();
        }
    }

    private void SpawnCrawlerFromList(List<Crawler> bugs)
    {
        if (bugs.Count == 0)
        {
            Debug.LogWarning("No Crawlers to Spawn");
            return;
        }

        int portalIndex = 0;
        int portalAllowed = 0;

        for (int i = 0; i < bugs.Count; i++)
        {
            if (portalAllowed >= portalMaxAllowed)
            {
                portalIndex++;
                SelectSpawnPoint();
                PlaySpawnEffect(portalIndex);
                portalAllowed = 0;
            }

            Vector3 randomCircle = Random.insideUnitSphere;
            randomCircle.z = 0;
            Vector3 randomPoint = randomCircle + spawnPoint.position;
            bugs[i].transform.position = randomPoint;
            bugs[i].transform.rotation = spawnPoint.rotation * Quaternion.Euler(0, randomCircle.y, 0);

            StartCoroutine(SpawnRandomizer(bugs[i], i * 0.2f));
            portalAllowed++;
        }
    }

    private IEnumerator SpawnRandomizer(Crawler bug, float delay)
    {
        if (!isActive) yield break;

        yield return new WaitForSeconds(delay);

        bug.gameObject.SetActive(true);
        bug.Spawn();
        AddToActiveList(bug);

        Debug.Log($"Spawned and activated: {bug.name}");
    }

    private void PlaySpawnEffect(int index)
    {
        index = index % portalEffects.Count;

        if (portalEffects[index].isActive)
        {
            PlaySpawnEffect(index + 1);
            return;
        }

        portalEffects[index].transform.position = spawnPoint.position;
        portalEffects[index].transform.rotation = spawnPoint.rotation;
        portalEffects[index].StartEffect();
    }

    public void SpawnAtPoint(Transform point, int spawnAmount)
    {
        List<Crawler> spawnList = new List<Crawler>();
        for (int i = 0; i < spawnAmount && i < crawlers.Count; i++)
        {
            spawnList.Add(crawlers[i]);
        }
        spawnPoint = point;
        SpawnCrawlerFromList(spawnList);
    }

    public void EndBattle()
    {
        isActive = false;
        KillAllCrawlers();
    }

    public void KillAllCrawlers()
    {
        foreach (var crawler in activeCrawlers.ToList())
        {
            crawler.DealyedDamage(1000, 0.2f, WeaponType.Default);
        }
    }

    public void AddtoRespawnList(Crawler crawler, CrawlerType type)
    {
        if (activeCrawlers.Remove(crawler))
        {
            GetCrawlerList(type).Add(crawler);
            crawler.gameObject.SetActive(false);
            Debug.Log($"Crawler {crawler.name} returned to pool and deactivated.");
        }
    }

    public void AddToActiveList(Crawler crawler)
    {
        if (GetCrawlerList(crawler.crawlerType).Remove(crawler))
        {
            if (!activeCrawlers.Contains(crawler))
            {
                activeCrawlers.Add(crawler);
                crawler.gameObject.SetActive(true);
                Debug.Log($"Crawler {crawler.name} added to active list and activated.");
            }
        }
    }

    private void ValidateActiveCrawlers()
    {
        for (int i = activeCrawlers.Count - 1; i >= 0; i--)
        {
            Crawler crawler = activeCrawlers[i];
            if (crawler == null)
            {
                activeCrawlers.RemoveAt(i);
                Debug.LogWarning($"Removed null crawler at index {i}");
            }
            else if (!crawler.gameObject.activeSelf)
            {
                crawler.gameObject.SetActive(true);
                Debug.LogWarning($"Reactivated inactive crawler {crawler.name} at index {i}");
            }
        }
    }
}