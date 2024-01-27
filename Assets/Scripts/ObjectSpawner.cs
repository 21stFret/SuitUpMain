using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private List<Crawler> crawlers = new List<Crawler>();
    [SerializeField] private List<CrawlerDaddy> crawlerDaddy =  new List<CrawlerDaddy>();
    [SerializeField] private List<Transform> spawnPoints;
    private Transform spawnPoint;
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
        spawnRound = 0;
        Spawn();
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
            Spawn();
            timeElapsed = 0f;
            if(spawnRound > PlayerSavedData.instance._waveScore)
            {
                PlayerSavedData.instance._waveScore = spawnRound;
            }
        }
        timeText.text = ((roundTimer-timeElapsed)).ToString("0");
    }

    private void SelectSpawnPoint()
    {
        int randomSpawnPoint = Random.Range(0, spawnPoints.Count);
        spawnPoint = spawnPoints[randomSpawnPoint];
    }

    private void Spawn()
    {
        SpawnRoundMultiplier();
        SelectSpawnPoint();

        for(int i = 0; i < spawnAmmount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * 3;
            Vector3 randomPoint = new Vector3(randomCircle.x, 0, randomCircle.y) + spawnPoint.position;
            crawlers[i].transform.position = randomPoint;
            crawlers[i].transform.rotation = Quaternion.identity;
            crawlers[i].GetComponent<Crawler>().Respawn();
            crawlers.Remove(crawlers[i]);
        }
        if (spawnRound > 3)
        {
            for (int i = 0; i < spawnRound / 2; i++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * 3;
                Vector3 randomPoint = new Vector3(randomCircle.x, 0, randomCircle.y) + spawnPoint.position;
                crawlerDaddy[i].transform.position = randomPoint;
                crawlerDaddy[i].transform.rotation = Quaternion.identity;
                crawlerDaddy[i].Respawn();
                crawlerDaddy.Remove(crawlerDaddy[i]);
            }
        }
    }

    public void SpawnAtPoint(Vector3 point, int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            crawlers[i].transform.position = point;
            crawlers[i].transform.rotation = Quaternion.identity;
            crawlers[i].Respawn();
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