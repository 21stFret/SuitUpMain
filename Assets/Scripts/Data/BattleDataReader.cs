using UnityEngine;
using System;
using System.Collections.Generic;

public class BattleDataReader : MonoBehaviour
{
    public static BattleDataReader instance;

    public Dictionary<AreaType, List<CrawlerSquad>> LoadSquadsFromExcel()
    {
        Debug.Log("Loading Squads for all Area Types");
        Dictionary<AreaType, List<CrawlerSquad>> areaSquads = new Dictionary<AreaType, List<CrawlerSquad>>();
        List<Dictionary<string, object>> data = CSVReader.Read("Suit Up Data - Enemy Squads");

        if (data == null || data.Count == 0)
        {
            Debug.LogError("No data was read from the Excel file.");
            return areaSquads;
        }

        foreach (var row in data)
        {
            try
            {
                if (!row.ContainsKey("AreaType") || string.IsNullOrEmpty(row["AreaType"].ToString()))
                {
                    Debug.LogWarning($"Row missing AreaType. Skipping this row.");
                    continue;
                }

                if (!Enum.TryParse(row["AreaType"].ToString(), out AreaType areaType))
                {
                    Debug.LogWarning($"Invalid AreaType value '{row["AreaType"]}'. Skipping this row.");
                    continue;
                }

                CrawlerSquad squad = CreateSquadFromRow(row);
                if (squad.crawlerGroups.Length > 0)
                {
                    if (!areaSquads.ContainsKey(areaType))
                    {
                        areaSquads[areaType] = new List<CrawlerSquad>();
                    }
                    areaSquads[areaType].Add(squad);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing row: {e.Message}");
            }
        }

        foreach (var kvp in areaSquads)
        {
            Debug.Log($"Loaded {kvp.Value.Count} squads for Area Type: {kvp.Key}");
        }

        return areaSquads;
    }

    private CrawlerSquad CreateSquadFromRow(Dictionary<string, object> row)
    {
        List<CrawlerGroup> crawlerGroups = new List<CrawlerGroup>();

        AddCrawlerGroupIfPresent(crawlerGroups, row, "Swarm", CrawlerType.Crawler);
        AddCrawlerGroupIfPresent(crawlerGroups, row, "Daddy", CrawlerType.Daddy);
        AddCrawlerGroupIfPresent(crawlerGroups, row, "Spitter", CrawlerType.Spitter);
        AddCrawlerGroupIfPresent(crawlerGroups, row, "Leaper", CrawlerType.Leaper);
        AddCrawlerGroupIfPresent(crawlerGroups, row, "Charger", CrawlerType.Charger);
        AddCrawlerGroupIfPresent(crawlerGroups, row, "Hunter", CrawlerType.Hunter);

        return new CrawlerSquad(crawlerGroups.ToArray());
    }

    private void AddCrawlerGroupIfPresent(List<CrawlerGroup> groups, Dictionary<string, object> row, string key, CrawlerType type)
    {
        if (TryGetIntValue(row, key, out int count) && count > 0)
        {
            groups.Add(new CrawlerGroup(type, count));
        }
    }

    private bool TryGetIntValue(Dictionary<string, object> row, string key, out int value)
    {
        value = 0;
        if (!row.ContainsKey(key) || string.IsNullOrEmpty(row[key].ToString()))
        {
            return false;
        }

        return int.TryParse(row[key].ToString(), out value);
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Custom/Load Squads From Excel")]
    public static void LoadSquadsFromExcelMenu()
    {
        BattleDataReader instance = FindObjectOfType<BattleDataReader>();
        if (instance != null)
        {
            Dictionary<AreaType, List<CrawlerSquad>> loadedSquads = instance.LoadSquadsFromExcel();
            Debug.Log($"Loaded squads for {loadedSquads.Count} area types from Excel.");
        }
        else
        {
            Debug.LogError("BattleDataReader instance not found in the scene.");
        }
    }
#endif
}