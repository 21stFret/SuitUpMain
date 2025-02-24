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
    public PortalEffect hordePortal;
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
    public float portalSize;

    public List<Crawler> spawnListArmy = new List<Crawler>();

    public CrawlerSpitter runner;

    public Battle currentBattle;
    private bool standardBattle;
    private int burstSpawnAmount;

    private bool hordeBattle;

    public Crawler huntedTarget;

    public void Init()
    {
        InitCrawlers();
        StartCoroutine(PulseCheckCrawlersAlive());
    }

    public void LoadBattle()
    {
        currentBattle = battleManager.currentBattle;
        battleRound = 0;
        totalTimeElapsesd = 0;
        MultiplyBurstByDifficulty();
        burstTimer = currentBattle.burstTimer;
        burstSpawnAmount = currentBattle.burstMin;       
        timeElapsed = burstTimer;
        standardBattle = currentBattle.battleType == BattleType.Exterminate;
        endless = !standardBattle;
        hordeBattle = false;
        if(currentBattle.battleType == BattleType.Survive)
        {
            hordeBattle = true;
        }
        battleRoundMax = standardBattle? currentBattle.battleArmy.Count : 0;
        isActive = true;
        spawnListArmy = GenerateArmyList();

    }

    private void MultiplyBurstByDifficulty()
    {
        float difficulty = BattleManager.instance.dificultyMultiplier;
        currentBattle.burstMin = Mathf.RoundToInt(currentBattle.burstMin * difficulty);
        currentBattle.burstMax = Mathf.RoundToInt(currentBattle.burstMax * difficulty);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(!isActive)
        {
            return;
        }
        if (hordeBattle || currentBattle.battleType == BattleType.Upload)
        {
            BurstTimer();
        }
        //ValidateActiveCrawlers();
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
            spawnListArmy = GenerateArmyList();
            SpawnFromArmy(spawnListArmy, Vector3.zero );
        }
        //UpdateTimerDisplay();
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

            float difficulty = BattleManager.instance.dificultyMultiplier;
            if (difficulty > 1)
            {
                crawler.eliteChance *= difficulty;
            }
            else
            {
                crawler.eliteChance = 0;
            }
        }
    }

    public void BeginSpawningSquads()
    {
        if(!isActive)
        {
            return;
        }
        IncrementSpawnRound();
        SpawnCheck();
    }

    private void SelectSpawnPoint()
    {
        int randomSpawnPoint;
        int check = 0;
        do
        {
            check ++;
            randomSpawnPoint = Random.Range(0, spawnPoints.Count);
        } while (randomSpawnPoint == currentSpawnPoint && check<3);

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

        currentSquad = currentBattle.battleArmy[Mathf.Clamp(battleRound - 1, 0, 10)];
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
            float difficulty = BattleManager.instance.dificultyMultiplier;
            float amount = currentSquad.crawlerGroups[j].amount * difficulty;
            if(difficulty > 1)
            {
                if (currentSquad.crawlerGroups[j].type == CrawlerType.Crawler)
                {
                    amount *= 1.2f;
                }
            }
            for (int k = 0; k < amount && k < GetCrawlerList(currentSquad.crawlerGroups[j].type).Count; k++)
            {
                _spawnList.Add(list[k]);
            }
        }
        _spawnList.Shuffle();
        StartCoroutine(SpawnCrawlerFromList(_spawnList));
    }
    
    public void SpawnFromArmy(List<Crawler> armyList, Vector3 loc)
    {
        if (loc != Vector3.zero)    
        {
            StartCoroutine(SpawnBurstCrawlerFromList(armyList, loc));
        }
        else
        {
            StartCoroutine(SpawnBurstCrawlerFromList(armyList));
        }
    }

    public List<Crawler> GenerateArmyList()
    {
        List<Crawler> _spawnListArmy = new List<Crawler>();
        foreach (CrawlerSquad squad in currentBattle.battleArmy)
        {
            for (int i = 0; i < squad.crawlerGroups.Length; i++)
            {
                CrawlerType type = squad.crawlerGroups[i].type;
                int amount = squad.crawlerGroups[i].amount;

                for (int j = 0; j < amount && j < GetCrawlerList(type).Count; j++)
                {
                    _spawnListArmy.Add(GetCrawlerList(type)[j]);
                }
            }
        }
        _spawnListArmy.Shuffle();
        return _spawnListArmy;
    }

    public List<Crawler> GenerateNewSquad(CrawlerType crawlerType, int amount)
    {
        List<Crawler> _spawnListArmy = new List<Crawler>();
        for (int i = 0; i < amount && i < GetCrawlerList(crawlerType).Count; i++)
        {
            _spawnListArmy.Add(GetCrawlerList(crawlerType)[i]);
        }
        return _spawnListArmy;
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
            if (!isActive) yield break;
            foreach(Crawler crawler in bugs)
            {
                AddToActiveList(crawler);
            }
            SelectSpawnPoint();
            PlaySpawnEffect();
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            foreach(Crawler crawler in bugs)
            {
                AddToActiveList(crawler);
            }
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
            StartCoroutine(SpawnRandomizer(bugs[i], delay, FromDaddy));
            portalAllowed++;
        }
    }
    private IEnumerator SpawnBurstCrawlerFromList(List<Crawler> bugs, Vector3 location)
    {
        
        if (!isActive) yield break;
        foreach(Crawler crawler in bugs)
        {
            AddToActiveList(crawler);
        }
        int portalAllowed = 0;
        for (int i = 0; i < bugs.Count; i++)
        {
            Vector3 randomCircle = Random.insideUnitSphere * portalSize;
            randomCircle.z = 0;
            Vector3 randomPoint = randomCircle + location;
            bugs[i].transform.position = randomPoint;
            bugs[i].transform.rotation = Quaternion.Euler(0, randomCircle.y, 0);


            StartCoroutine(SpawnRandomizer(bugs[i], i * 0.2f));
            portalAllowed++;
        }
        
    }

    private IEnumerator SpawnBurstCrawlerFromList(List<Crawler> bugs)
    {
        if (!isActive) yield break;
        int portalAllowed = 0;
        foreach(Crawler crawler in bugs)
        {
            AddToActiveList(crawler);
        }
        if (!hordeBattle)
        {
            SelectSpawnPoint();
            PlaySpawnEffect();
        }
        else if (!hordePortal.isActive)
        {
            spawnPoint = spawnPoints[0];
            hordePortal.transform.position = spawnPoint.position;
            hordePortal.transform.rotation = spawnPoint.rotation;
            hordePortal.StartEffect();
            yield return new WaitForSeconds(0.5f);
        }

        for (int i = 0; i < burstSpawnAmount && i<bugs.Count-1; i++)
        {
            if(!hordeBattle)
            {
                if (portalAllowed >= portalMaxAllowed)
                {
                    SelectSpawnPoint();
                    PlaySpawnEffect();
                    portalAllowed = 0;
                    yield return new WaitForSeconds(0.5f);
                }
            }
            else
            {
                spawnPoint = spawnPoints[0];
            }

            Vector3 randomCircle = Random.insideUnitSphere * portalSize;
            randomCircle.z = 0;
            Vector3 randomPoint = randomCircle + spawnPoint.position;
            bugs[i].transform.position = randomPoint;
            bugs[i].transform.rotation = spawnPoint.rotation * Quaternion.Euler(0, randomCircle.y, 0);


            StartCoroutine(SpawnRandomizer(bugs[i], i * 0.2f));
            portalAllowed++;
        }
        
    }

    private IEnumerator SpawnRandomizer(Crawler bug, float delay, bool daddy = false)
    {
        yield return new WaitForSeconds(delay);
        bug.gameObject.SetActive(true);
        bug.Spawn(daddy);
        // bugs have a bigger search range at the start of a horde
        if (hordeBattle)
        {
            bug.FindClosestTarget(70);
        }
        if(BattleManager.instance.currentBattle.battleType == BattleType.Upload)
        {
            bug.target = BattleManager.instance.capturePoint.transform;
        }
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
        if(!isActive)
        {
            return;
        }
        isActive = false;
        if(hordePortal.isActive)
        {
            hordePortal.StopEffect();
        }
        //KillAllCrawlers();
    }

    public void KillAllCrawlers()
    {
        foreach (var crawler in activeCrawlers.ToList())
        {
            crawler.DealyedDamage(1000, 0.2f, WeaponType.AoE);
        }
    }

    public void AddtoRespawnList(Crawler crawler, CrawlerType type)
    {
        if (activeCrawlers.Remove(crawler))
        {
            GetCrawlerList(type).Add(crawler);
            //crawler.gameObject.SetActive(false);
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
                if(crawler.gameObject.CompareTag("Boss"))
                {
                    huntedTarget = crawler;
                }
            }
        }
    }

    private IEnumerator PulseCheckCrawlersAlive()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            ValidateActiveCrawlers();
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

    public void SpawnBoss()
    {
        //TODO add list of bosses to pick from depending on area
        List<Crawler> spawnList = new List<Crawler>
        {
            albinos[0]
        };
        StartCoroutine(SpawnCrawlerFromList(spawnList));
    }
}