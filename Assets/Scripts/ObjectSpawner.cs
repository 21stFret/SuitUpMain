using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Object Pools")]
    [SerializeField] private List<Crawler> crawlers = new List<Crawler>();
    [SerializeField] private List<CrawlerDaddy> crawlerDaddy =  new List<CrawlerDaddy>();
    [SerializeField] private List<Transform> spawnPoints;
    [Header("Spawn Settings")]
    private Transform spawnPoint;
    public PortalEffect portalEffect;
    [SerializeField] private float roundTimer;
    [SerializeField] private int spawnAmmount;
    public int baseSpawnAmmount;
    public int spawnRound;

    public float timeElapsed = 0f;
    public TMP_Text timeText;
    public TMP_Text waveText;

    public static ObjectSpawner instance;

    public bool isActive;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //spawnRound = 0;
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        timeElapsed += Time.deltaTime;
        if (timeElapsed >= roundTimer)
        {
            BeginSpawning();
            timeElapsed = 0f;
            if(spawnRound > PlayerSavedData.instance._waveScore)
            {
                PlayerSavedData.instance._waveScore = spawnRound;
            }
        }
        timeText.text = ((roundTimer-timeElapsed)).ToString("00:00");
    }

    private void SelectSpawnPoint()
    {
        int randomSpawnPoint = Random.Range(0, spawnPoints.Count);
        spawnPoint = spawnPoints[randomSpawnPoint];
    }

    private void BeginSpawning()
    {
        SpawnRoundMultiplier();
        SelectSpawnPoint();
        StartCoroutine(SpawnDelay());
    }

    private void SpawnCrawlers()
    {
        for (int i = 0; i < spawnAmmount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * 2;
            Vector3 randomPoint = new Vector3(randomCircle.x, 0, randomCircle.y) + spawnPoint.position;
            crawlers[i].transform.position = randomPoint;
            crawlers[i].transform.rotation = Quaternion.identity;
            StartCoroutine(SpawnRandomizer(crawlers[i], Random.Range(0, 1.3f)));
        }
        if (spawnRound > 3)
        {
            for (int i = 0; i < spawnRound / 2; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * 3;
                Vector3 randomPoint = new Vector3(randomCircle.x, 0, randomCircle.y) + spawnPoint.position;
                crawlerDaddy[i].transform.position = randomPoint;
                crawlerDaddy[i].transform.rotation = Quaternion.identity;
                crawlerDaddy[i].Spawn();
                crawlerDaddy.Remove(crawlerDaddy[i]);
            }
        }
    }

    private IEnumerator SpawnDelay()
    {
        PLaySpawnEffect();
        yield return new WaitForSeconds(1f);
        SpawnCrawlers();
    }


    private IEnumerator SpawnRandomizer(Crawler bug, float delay)
    {
        yield return new WaitForSeconds(delay);
        bug.Spawn();
        crawlers.Remove(bug);
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
            crawlers[i].transform.position = point;
            crawlers[i].transform.rotation = Quaternion.identity;
            crawlers[i].Spawn();
            crawlers.Remove(crawlers[i]);
        }
    }   

    private void SpawnRoundMultiplier()
    {
        spawnRound++;
        waveText.text = "Wave " + spawnRound.ToString();
        spawnAmmount = baseSpawnAmmount * ((spawnRound /2) +1);
        roundTimer += 5;

    }

    public void AddtoRespawnList(Crawler crawler)
    {
        if (crawler.GetComponent<CrawlerDaddy>() != null)
        {
            crawlerDaddy.Add((CrawlerDaddy)crawler);
            return;
        }
        crawlers.Add(crawler);
    }
}