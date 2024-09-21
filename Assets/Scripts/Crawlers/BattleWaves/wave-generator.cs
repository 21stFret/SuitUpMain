using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WaveGenerator", menuName = "Battle/WaveGenerator")]
public class WaveGenerator : ScriptableObject
{
    [System.Serializable]
    public class EnemyTypeInfo
    {
        public CrawlerType type;
        public int minCount;
        public int maxCount;
        public float difficultyMultiplier = 1f;
    }

    public List<EnemyTypeInfo> allowedEnemyTypes;
    public int minWaves = 3;
    public int maxWaves = 5;
    public float baseRoundTimer = 60f;
    public float timerVariation = 10f;

    public Battle GenerateBattle(int battleId, BattleType type, BattleDifficulty difficulty)
    {
        Battle battle = new Battle(battleId, type, difficulty);

        int waveCount = Random.Range(minWaves, maxWaves + 1);
        float difficultyMultiplier = GetDifficultyMultiplier(difficulty);

        for (int i = 0; i < waveCount; i++)
        {
            BattleWave wave = GenerateWave(i, waveCount, difficultyMultiplier);
            battle.battleWaves.Add(wave);
        }

        return battle;
    }

    private BattleWave GenerateWave(int waveIndex, int totalWaves, float difficultyMultiplier)
    {
        List<CrawlerWave> crawlerWaves = new List<CrawlerWave>();

        foreach (EnemyTypeInfo enemyInfo in allowedEnemyTypes)
        {
            int count = Mathf.RoundToInt(Random.Range(enemyInfo.minCount, enemyInfo.maxCount + 1) * difficultyMultiplier * enemyInfo.difficultyMultiplier);
            count = Mathf.Max(1, count); // Ensure at least one enemy of each type

            // Increase count based on wave progression
            float waveProgression = (float)waveIndex / totalWaves;
            count += Mathf.RoundToInt(count * waveProgression * 0.5f);

            crawlerWaves.Add(new CrawlerWave(enemyInfo.type, count));
        }

        float roundTimer = baseRoundTimer + Random.Range(-timerVariation, timerVariation);
        roundTimer *= 1 + (waveIndex * 0.1f); // Increase timer slightly for later waves

        return new BattleWave(crawlerWaves.ToArray(), roundTimer);
    }

    private float GetDifficultyMultiplier(BattleDifficulty difficulty)
    {
        switch (difficulty)
        {
            case BattleDifficulty.Easy:
                return 0.8f;
            case BattleDifficulty.Medium:
                return 1f;
            case BattleDifficulty.Hard:
                return 1.3f;
            default:
                return 1f;
        }
    }
}