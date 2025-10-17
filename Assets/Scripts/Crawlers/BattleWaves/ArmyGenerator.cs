using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class ArmyGenerator : MonoBehaviour
{
    public SerializedDictionary<AreaType, List<CrawlerSquad>> areaSquads = new SerializedDictionary<AreaType, List<CrawlerSquad>>();
    public List<CrawlerSquad> battleArmy = new List<CrawlerSquad>();
    public int MaxSquads = 3;
    public BattleDataReader battleDataReader;
    public AreaType currentAreaType;
    public BattleType currentBattleType;

    public void LoadAllSquadsFromExcel()
    {
        if (battleDataReader == null)
        {
            Debug.LogError("BattleDataReader is not assigned to ArmyGenerator.");
            return;
        }

        var loadedSquads = battleDataReader.LoadSquadsFromExcel();
        areaSquads.Clear();
        foreach (var kvp in loadedSquads)
        {
            areaSquads.Add(kvp.Key, kvp.Value);
        }
        //Debug.Log($"Loaded squads for {areaSquads.Count} area types.");
    }

    public List<CrawlerSquad> BuildArmy()
    {
        battleArmy.Clear();
        if (!areaSquads.ContainsKey(currentAreaType))
        {
            Debug.LogWarning($"No squads found for area type: {currentAreaType}");
            return battleArmy;
        }

        List<CrawlerSquad> currentAreaSquads = areaSquads[currentAreaType];
        int _maxSquads = MaxSquads;
        if (currentBattleType == BattleType.Survive)
        {
            _maxSquads = Mathf.Min(MaxSquads*2, currentAreaSquads.Count);
        }

        int squadCount = Mathf.Min(_maxSquads, currentAreaSquads.Count);

        for (int i = 0; i < squadCount; i++)
        {
            int randomIndex = Random.Range(0, currentAreaSquads.Count);
            battleArmy.Add(currentAreaSquads[randomIndex]);
        }

        //Debug.Log($"Built an army with {battleArmy.Count} squads for area type: {currentAreaType}");
        return battleArmy;
    }

    public void SetCurrentAreaType(AreaType areaType)
    {
        currentAreaType = areaType;
    }
}