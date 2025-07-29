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
    [SerializeField] private List<Crawler> leapers = new List<Crawler>();
    [SerializeField] private List<Crawler> hunters = new List<Crawler>();
    [SerializeField] private List<Crawler> bombers = new List<Crawler>();
    [SerializeField] private List<Crawler> spore = new List<Crawler>();
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
    private int portalAllowed = 0;
    public float portalSize;
    public List<Crawler> spawnListArmy = new List<Crawler>();

    // Reference to Battle from BattleManager
    public Battle currentBattle;
    
    // Local copies of values we want to modify
    public int localBurstMin;
    public int localBurstMax;
    public float localBurstTimer;
    public bool standardBattle;
    public int burstSpawnAmount;
    private bool hordeBattle;
    public Crawler huntedTarget;
    private List<Coroutine> activeSpawnCoroutines = new List<Coroutine>();

    public void Init()
    {
        InitCrawlers();
        StartCoroutine(PulseCheckCrawlersAlive());
    }

    public void LoadBattle()
    {
        // Keep reference to original battle
        currentBattle = battleManager.currentBattle;
        
        // Store local copies of values you want to modify
        localBurstMin = currentBattle.burstMin;
        localBurstMax = currentBattle.burstMax;
        localBurstTimer = currentBattle.burstTimer;
        
        battleRound = 0;
        totalTimeElapsesd = 0;
        
        // Modify local values only
        MultiplyBurstByDifficulty();
        
        // Use local values for gameplay settings
        burstTimer = localBurstTimer; 
        burstSpawnAmount = localBurstMin;
        
        timeElapsed = burstTimer;
        standardBattle = battleManager._usingBattleType == BattleType.Exterminate;
        endless = !standardBattle;
        hordeBattle = false;
        
        if(battleManager._usingBattleType == BattleType.Survive)
        {
            hordeBattle = true;
        }
        
        battleRoundMax = standardBattle? currentBattle.battleArmy.Count : 0;
        isActive = true;
        spawnListArmy.Clear();
        spawnListArmy = GenerateArmyList();

    }

    private void MultiplyBurstByDifficulty()
    {
        float difficulty = BattleManager.instance.dificultyMultiplier;
        
        // Modify local values instead of battle values
        localBurstMin = Mathf.RoundToInt(localBurstMin + difficulty);
        localBurstMax = Mathf.RoundToInt(localBurstMax + difficulty);
        localBurstTimer -= difficulty - 1;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (!isActive)
        {
            if (activeCrawlerCount > 0)
            {
                // If battle is not active but crawlers are still
                activeCrawlers.Clear();
            }
            return;
        }
        if (hordeBattle || battleManager._usingBattleType == BattleType.Upload)
        {
            BurstTimer();
        }
    }

    private void BurstTimer()
    {
        if (activeCrawlerCount > 50)
        {
            // If there are too many crawlers, stop spawning
            return;
        }
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= burstTimer)
        {
            timeElapsed = 0f;
            
            // Slowly increase the amount of crawlers spawned per burst
            totalTimeElapsesd += 0.5f;
            int min = Mathf.RoundToInt(localBurstMin + totalTimeElapsesd);
            min = Mathf.Clamp(min, localBurstMin, localBurstMax);
            burstSpawnAmount = min;
            
            spawnListArmy.Clear();
            spawnListArmy = GenerateArmyList();
            List<Crawler> spawnList = new List<Crawler>();
            for (int i = 0; i < burstSpawnAmount && i < spawnListArmy.Count; i++)
            {
                spawnList.Add(spawnListArmy[i]);
            }
            SpawnFromArmy(spawnList);
        }
    }

    private void InitCrawlers()
    {
        InitCrawlerList(crawlers, CrawlerType.Crawler);
        InitCrawlerList(crawlerDaddy, CrawlerType.Daddy);
        InitCrawlerList(albinos, CrawlerType.Albino);
        InitCrawlerList(spitters, CrawlerType.Spitter);
        InitCrawlerList(chargers, CrawlerType.Charger);
        InitCrawlerList(leapers, CrawlerType.Leaper);
        InitCrawlerList(hunters, CrawlerType.Hunter);
        InitCrawlerList(bombers, CrawlerType.Bomber);
        InitCrawlerList(spore, CrawlerType.Spore);
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
        SpawnSquad();
    }

    private void SelectSpawnPoint()
    {
        int randomSpawnPoint;
        int check = 0;
        do
        {
            check++;
            randomSpawnPoint = Random.Range(0, spawnPoints.Count);
        } while (randomSpawnPoint == currentSpawnPoint && check < 3);

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
                    amount *= 2f;
                }
            }
            for (int k = 0; k < amount && k < list.Count; k++)
            {
                _spawnList.Add(list[k]);
            }
        }
        _spawnList.Shuffle();
        StartCoroutine(SpawnBurstCrawlerFromList(_spawnList));
    }
    
    public void SpawnFromArmy(List<Crawler> armyList, Transform loc = null)
    {
        if (loc != null)
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
                if(BannedTypes().Contains(type))
                {
                    continue;
                }
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

    public List<CrawlerType> BannedTypes()
    {
        int[] typeCount = new int[System.Enum.GetValues(typeof(CrawlerType)).Length];
        List<CrawlerType> bannedTypes = new List<CrawlerType>();
        foreach (Crawler crawler in activeCrawlers)
        {
            if (crawler.crawlerType == CrawlerType.Crawler)
            {
                continue;
            }
            if (crawler.gameObject.CompareTag("Boss"))
            {
                continue;
            }
            typeCount[(int)crawler.crawlerType]++;
        }
        for (int i = 0; i < typeCount.Length; i++)
        {
            CrawlerType crawlerType = (CrawlerType)i;
            int currentCount = typeCount[i];
            int restriction = 0;
            switch (crawlerType)
            {
                case CrawlerType.Charger:
                    restriction = 3; // Chargers are limited to 3
                    break;
                case CrawlerType.Spitter:
                    restriction = 7; // Spitters are limited to 7
                    break;
                case CrawlerType.Daddy:
                    restriction = 7; // Daddies are limited to 7
                    break;
                case CrawlerType.Leaper:
                    restriction = 4; // Leapers are limited to 4
                    break;
                case CrawlerType.Hunter:
                    restriction = 3; // Hunters are limited to 3
                    break;
                case CrawlerType.Bomber:
                    restriction = 5; // Bombers are limited to 5
                    break;
                case CrawlerType.Spore:
                    restriction = 2; // Spores are limited to 2
                    break;
                default:
                    restriction = 50; // No restrictions for other types
                    break;
            }
            if(hordeBattle)
            {
                restriction *=2; // De-restrict during horde battles
            }
            if (currentCount > restriction)
            {
                bannedTypes.Add(crawlerType);
                //Debug.Log($"Banned {crawlerType} - Current count: {currentCount}, Limit: {restriction}");
            }
        }
        return bannedTypes;
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
            case CrawlerType.Leaper: return leapers;
            case CrawlerType.Hunter: return hunters;
            case CrawlerType.Bomber: return bombers;
            case CrawlerType.Spore: return spore;
            default: return new List<Crawler>();
        }
    }

    public void SetCrawlerSpawnPoint(Transform point)
    {
        spawnPoint = point;
    }

    private IEnumerator SpawnCrawlerFromList(List<Crawler> bugs)
    {
        if (!isActive) yield break;
        foreach (Crawler crawler in bugs)
        {
            AddToActiveList(crawler);
        }
        SelectSpawnPoint();
        PlaySpawnEffect();
        yield return new WaitForSeconds(0.5f);
        var spawnCoroutine = StartCoroutine(SpawnRandomizer(bugs, spawnPoint));
        activeSpawnCoroutines.Add(spawnCoroutine);
    }
    
    private IEnumerator SpawnBurstCrawlerFromList(List<Crawler> bugs, Transform targetLocation)
    {

        if (!isActive) yield break;
        foreach (Crawler crawler in bugs)
        {
            AddToActiveList(crawler);
        }
        if (targetLocation != null)
        {
            spawnPoint = targetLocation;
        }

        var spawnCoroutine = StartCoroutine(SpawnRandomizer(bugs, spawnPoint));
        activeSpawnCoroutines.Add(spawnCoroutine);

    }
    
    private IEnumerator SpawnBurstCrawlerFromList(List<Crawler> bugs)
    {
        if (!isActive) yield break;
        foreach (Crawler crawler in bugs)
        {
            AddToActiveList(crawler);
        }

        if (hordeBattle)
        {
            if (!hordePortal.isActive)
            {
                spawnPoint = spawnPoints[0];
                hordePortal.transform.position = spawnPoint.position;
                hordePortal.transform.rotation = spawnPoint.rotation;
                hordePortal.StartEffect();
                yield return new WaitForSeconds(0.5f);
            }
            StartCoroutine(SpawnRandomizer(bugs, spawnPoint));
        }
        else
        {
            List<Crawler> splitBugs = new List<Crawler>();
            portalAllowed = 0;
            var tempMaxportalMaxAllowed = portalMaxAllowed;
            if (bugs.Count < portalMaxAllowed)
            {
                tempMaxportalMaxAllowed = bugs.Count;
            }
            for (int i = 0; i < bugs.Count; i++)
            {
                splitBugs.Add(bugs[i]);
                portalAllowed++;
                if (portalAllowed >= tempMaxportalMaxAllowed || i == bugs.Count - 1)
                {
                    SelectSpawnPoint();
                    PlaySpawnEffect();
                    yield return new WaitForSeconds(1f);
                    print($"Spawning {splitBugs.Count} crawlers at {spawnPoint.name}");
                    var spawnCoroutine = StartCoroutine(SpawnRandomizer(splitBugs, spawnPoint));
                    activeSpawnCoroutines.Add(spawnCoroutine);
                    splitBugs.Clear();
                    portalAllowed = 0;
                }
            }
        }
    }

    private IEnumerator SpawnRandomizer(List<Crawler> bugs, Transform localSpawnPoint, bool daddy = false)
    {
        List<Crawler> activeBugs = new List<Crawler>(bugs);
        foreach (var bug in activeBugs)
        {
            float random = daddy ? 0 : Random.Range(0.05f, 0.2f);

            yield return new WaitForSeconds(random);

            if (!isActive)
            {
                yield break;
            }

            Vector3 randomCircle = Random.insideUnitSphere * (portalSize / 2);
            randomCircle.z = 0;
            Vector3 randomPoint = randomCircle + localSpawnPoint.position;
            randomPoint.y = Mathf.Clamp(randomPoint.y, 2, 6);
            bug.transform.position = randomPoint;
            bug.transform.rotation = localSpawnPoint.rotation * Quaternion.Euler(0, randomCircle.y, 0);
            bug.gameObject.SetActive(true);
            bug.Spawn(daddy);
            // bugs have a bigger search range at the start of a horde
            if (hordeBattle)
            {
                bug.FindClosestTarget(70);
            }
            if (BattleManager.instance._usingBattleType == BattleType.Upload)
            {
                bug.target = BattleManager.instance.capturePoint.transform;
            }
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
        foreach (Crawler crawler in spawnList)
        {
            AddToActiveList(crawler);
        }
        StartCoroutine(SpawnRandomizer(spawnList, point));
    }

    public void EndBattle()
    {
        if (!isActive)
        {
            return;
        }
        isActive = false;
        if (hordePortal.isActive)
        {
            hordePortal.StopEffect();
        }
        KillAllCrawlers();
    }

    public void KillAllCrawlers()
    {
        // Stop all active spawn coroutines
        foreach (var coroutine in activeSpawnCoroutines)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        activeSpawnCoroutines.Clear();

        // Create a copy of the list to avoid modification during iteration
        List<Crawler> crawlersToKill = new List<Crawler>(activeCrawlers);
        
        foreach (var crawler in crawlersToKill)
        {
            if (crawler != null && crawler.gameObject != null)
            {
                // Immediate death instead of delayed
                crawler.TakeDamage(100,WeaponType.Default);
                
                // Force remove from active list if still present
                if (activeCrawlers.Contains(crawler))
                {
                    activeCrawlers.Remove(crawler);
                }
                
                // Deactivate the game object
                crawler.gameObject.SetActive(false);
            }
        }
        
        // Clear any remaining crawlers
        activeCrawlers.Clear();
        
        // Clear hunted target if it exists
        huntedTarget = null;
        
        Debug.Log($"Killed all crawlers. Active count after cleanup: {activeCrawlers.Count}");
    }

    public void AddtoRespawnList(Crawler crawler, CrawlerType type)
    {
        if (crawler == null) return;
        
        if (activeCrawlers.Contains(crawler))
        {
            activeCrawlers.Remove(crawler);
            
            // If this was the hunted target, clear it
            if (crawler == huntedTarget)
            {
                huntedTarget = null;
            }
            
            // Only add to respawn list if the crawler still exists
            if (crawler != null && crawler.gameObject != null)
            {
                GetCrawlerList(type).Add(crawler);
            }
            
            // Debug logging
            //Debug.Log($"{crawler.name} removed from active list. Active count: {activeCrawlers.Count}");
        }
        else
        {
            Debug.LogWarning($"{crawler.name} not found in active list");
        }
    }

    public void AddToActiveList(Crawler crawler)
    {
        if (GetCrawlerList(crawler.crawlerType).Remove(crawler))
        {
            if (!activeCrawlers.Contains(crawler))
            {
                activeCrawlers.Add(crawler);
                //Debug.Log($"{crawler.name} added to active list. Active count: {activeCrawlers.Count}");
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
            //ValidateActiveCrawlers();
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