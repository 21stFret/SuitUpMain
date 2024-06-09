using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
    public int ID;
    public BattleType battleType;
    public List<BattleWave> battleWaves = new List<BattleWave>();
}

[System.Serializable]
public struct CrawlerWave
{
    public CrawlerType type;
    public int count;

    public CrawlerWave(CrawlerType type, int count)
    {
        this.type = type;
        this.count = count;
    }

}

[System.Serializable]
public struct BattleWave
{
    public CrawlerWave[] crawlersInWave;
    public float roundTimer;

    public BattleWave(CrawlerWave[] crawlersInWave, float roundTimer)
    {
        this.crawlersInWave = crawlersInWave;
        this.roundTimer = roundTimer;
    }
}
