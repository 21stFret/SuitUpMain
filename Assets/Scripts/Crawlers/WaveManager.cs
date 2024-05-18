using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveManager", menuName = "WaveManager")]
public class WaveManager : ScriptableObject
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
