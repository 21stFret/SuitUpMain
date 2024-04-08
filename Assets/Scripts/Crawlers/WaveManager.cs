using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaveManager : MonoBehaviour
{
    public List<BattleWave> battleWaves = new List<BattleWave>();
}

[System.Serializable]
public struct CrawlerWave
{
    public CrawlerType type;
    public int count;

}

[System.Serializable]
public struct BattleWave
{
    public CrawlerWave[] crawlersInWave;
    public float roundTimer;
}
