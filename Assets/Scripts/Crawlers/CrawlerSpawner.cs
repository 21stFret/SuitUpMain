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
    public float totalTimeElapsesd = 0f;
    public TMP_Text timeText;
    public TMP_Text waveText;

    public static CrawlerSpawner instance;
    public BattleManager battleManager;
    public CrawlerSquad currentSquad;

    public bool isActive;

    public int portalMaxAllowed;

    public List<Crawler> spawnListArmy = new List<Crawler>();

    public CrawlerSpitter runner;

    public Battle currentBattle;
    private bool standardBattle;
    private int burstSpawnAmount;

    public void Init()
    {
        InitCrawlers();
    }

    public void LoadBattle()
    {
        currentBattle = battleManager.currentBattle;
        battleRound = 0;
        timeElapsed = 0;
        totalTimeElapsesd = 0;
        burstTimer = currentBattle.burstTimer;
        standardBattle = currentBattle.battleType == BattleType.Exterminate;
        endless = !standardBattle;
        battleRoundMax = standardBattle? currentBattle.battleArmy.Count : 0;
        isActive = true;
        GenerateArmyList();
        burstSpawnAmount = currentBattle.burstMin;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (isActive && !standardBattle)
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
            timeElapsed = 0f;
            
            // slowly increase the amount of crawlers spawned per burst
            totalTimeElapsesd += 0.1f;
            int min = Mathf.RoundToInt(currentBattle.burstMin + totalTimeElapsesd);
            min = Mathf.Clamp(min, currentBattle.burstMin, currentBattle.burstMax);
            burstSpawnAmount = Random.Range(min, currentBattle.burstMax);
            GenerateArmyList();
            SpawnFromArmy();
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

    public void BeginSpawningSquads()
    {
        IncrementSpawnRound();
        SpawnCheck();
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

        if (battleRound > battleRoundMax)
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
    }

    private void SpawnCheck()
    {
        if (isActive)
        {
            SpawnSquad();
        }
        else
        {
            print("Crawler tried to spawn a squad but was inactive");
        }
    }

    private void SpawnSquad()
    {
        var _spawnList = new List<Crawler>();
        for (int j = 0; j < currentSquad.crawlerGroups.Length; j++)
        {
            var list = GetCrawlerList(currentSquad.crawlerGroups[j].type);
            float amount = currentSquad.crawlerGroups[j].amount * BattleManager.instance.dificultyMultiplier;
            for (int k = 0; k < amount && k < GetCrawlerList(currentSquad.crawlerGroups[j].type).Count; k++)
            {
                _spawnList.Add(list[k]);
            }
        }
        _spawnList.Shuffle();
        StartCoroutine(SpawnCrawlerFromList(_spawnList));
    }
    
    private void SpawnFromArmy()
    {
        StartCoroutine(SpawnBurstCrawlerFromList(spawnListArmy));
    }

    private void GenerateArmyList()
    {
        spawnListArmy.Clear();
        foreach (CrawlerSquad squad in currentBattle.battleArmy)
        {
            for (int i = 0; i < squad.crawlerGroups.Length; i++)
            {
                CrawlerType type = squad.crawlerGroups[i].type;
                int amount = squad.crawlerGroups[i].amount;

                for (int j = 0; j < amount && j < GetCrawlerList(type).Count; j++)
                {
                    spawnListArmy.Add(GetCrawlerList(type)[j]);
                }
            }
        }
        spawnListArmy.Shuffle();
    }

    private List<Crawler> GetCrawlerList(CrawlerType type)
    {
        switch (type)
        {
            case CrawlerType.Crawler: return crawlers;
            case CrawlerType.Daddy: return crawlerDaddy;
            case CrawlerType.Albino: return albinos;
            case CrawlerType.Spitter: return spitters;
            case CrawlerType.Charger: return chargers;
            default: return new List<Crawler>();
        }
    }

    private IEnumerator SpawnCrawlerFromList(List<Crawler> bugs, bool FromDaddy = false)
    {
        int portalAllowed = 0;

        if (!FromDaddy)
        {
            SelectSpawnPoint();
            PlaySpawnEffect();
            yield return new WaitForSeconds(0.5f);
        }


        for (int i = 0; i < bugs.Count; i++)
        {
            if (portalAllowed >= portalMaxAllowed)
            {
                SelectSpawnPoint();
                PlaySpawnEffect();
                portalAllowed = 0;
                yield return new WaitForSeconds(0.5f);
            }

            Vector3 randomCircle = Random.insideUnitSphere;

            if (!FromDaddy)
            {
                randomCircle *= 5;
                randomCircle.z = 0;
            }
            else
            {
                //randomCircle = Vector3.zero;
                randomCircle.y = spawnPoint.position.y + 2;
            }

            Vector3 randomPoint = randomCircle + spawnPoint.position;
            bugs[i].transform.position = randomPoint;
            float randomY;
            if (FromDaddy)
            {
                randomY = Random.Range(0, 360);
            }
            else
            {
                randomY = randomCircle.y;
            }

            bugs[i].transform.rotation = spawnPoint.rotation * Quaternion.Euler(0, randomY, 0);

            float delay = FromDaddy? 0 : i * 0.2f;
            StartCoroutine(SpawnRandomizer(bugs[i], delay));
            portalAllowed++;
        }
    }

    private IEnumerator SpawnBurstCrawlerFromList(List<Crawler> bugs)
    {
        int portalAllowed = 0;
        SelectSpawnPoint();
        PlaySpawnEffect();
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < burstSpawnAmount; i++)
        {
            if (portalAllowed >= portalMaxAllowed)
            {
                SelectSpawnPoint();
                PlaySpawnEffect();
                portalAllowed = 0;
                yield return new WaitForSeconds(0.5f);
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
        yield return new WaitForSeconds(delay);

        if (!isActive) yield break;
        bug.gameObject.SetActive(true);
        bug.Spawn();
        AddToActiveList(bug);

        //Debug.Log($"Spawned and activated: {bug.name}");
    }

    private void PlaySpawnEffect()
    {
        portalEffects[currentSpawnPoint].transform.position = spawnPoint.position;
        portalEffects[currentSpawnPoint].transform.rotation = spawnPoint.rotation;
        portalEffects[currentSpawnPoint].StartEffect();
    }

    public void SpawnAtPoint(Transform point, int spawnAmount)
    {
        List<Crawler> spawnList = new List<Crawler>();
        for (int i = 0; i < spawnAmount && i < crawlers.Count; i++)
        {
            spawnList.Add(crawlers[i]);
        }
        spawnPoint = point;
        StartCoroutine(SpawnCrawlerFromList(spawnList, true));
    }

    public void EndBattle()
    {
        isActive = false;
        //KillAllCrawlers();
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
            //Debug.Log($"Crawler {crawler.name} returned to pool and deactivated.");
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
                //Debug.Log($"Crawler {crawler.name} added to active list and activated.");
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