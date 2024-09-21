using System.Collections.Generic;

public class Battle
{
    public int ID;
    public BattleType battleType;
    public List<BattleWave> battleWaves;
    public BattleDifficulty battleDifficulty;

    public Battle(int id, BattleType type, BattleDifficulty difficulty)
    {
        ID = id;
        battleType = type;
        battleDifficulty = difficulty;
        battleWaves = new List<BattleWave>();
    }
}

// These can remain as they were
public enum BattleDifficulty
{
    Easy,
    Medium,
    Hard
}

public enum BattleType
{
    Survive,
    Defend,
    Kill,
    Capture,
    Default
}

// These structs can remain as they were
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