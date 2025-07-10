using System.Collections.Generic;

[System.Serializable]
public class Battle
{
    public BattleType battleType;
    public List<CrawlerSquad> battleArmy;
    public float burstTimer;
    public int burstMin;
    public int burstMax;

    public Battle(BattleType type)
    {
        battleType = type;
        battleArmy = new List<CrawlerSquad>();
        burstTimer = 0;
        burstMin = 5;
        burstMax = 10;
    }
}

public enum BattleDifficulty
{
    Easy,
    Medium,
    Hard
}

public enum BattleType
{
    Exterminate,
    Survive,
    Hunt,
    Upload,
    MiniBoss,
    Boss,
    Default
}

[System.Serializable]
public struct CrawlerGroup
{
    public CrawlerType type;
    public int amount;

    public CrawlerGroup(CrawlerType type, int count)
    {
        this.type = type;
        this.amount = count;
    }
}

[System.Serializable]
public struct CrawlerSquad
{
    public CrawlerGroup[] crawlerGroups;
    
    public CrawlerSquad(CrawlerGroup[] crawlersInWave)
    {
        this.crawlerGroups = crawlersInWave;
    }
}