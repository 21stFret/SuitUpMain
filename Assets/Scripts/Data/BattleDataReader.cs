using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;

public class BattleDataReader : MonoBehaviour
{
    public static BattleDataReader instance;
    public bool usingTestData;

    public Dictionary<AreaType, List<CrawlerSquad>> LoadSquadsFromExcel()
    {
        //Debug.Log("Loading Squads for all Area Types");
        Dictionary<AreaType, List<CrawlerSquad>> areaSquads = new Dictionary<AreaType, List<CrawlerSquad>>();
        string fileName = usingTestData ? "Suit Up Data - Enemy Squads Test" : "Suit Up Data - Enemy Squads";
        List<Dictionary<string, object>> data = CSVReader.Read(fileName);

        if (data == null || data.Count == 0)
        {
            Debug.LogError("No data was read from the Excel file.");
            return areaSquads;
        }

        foreach (var row in data)
        {
            try
            {
                if (!row.ContainsKey("AreaType") || row["AreaType"] == null || string.IsNullOrEmpty(row["AreaType"].ToString()))
                {
                    Debug.LogWarning($"Row missing AreaType. Skipping this row.");
                    continue;
                }

                string areaTypeStr = row["AreaType"].ToString().Trim();
                if (string.IsNullOrEmpty(areaTypeStr))
                {
                    Debug.LogWarning("Empty AreaType value. Skipping this row.");
                    continue;
                }

                AreaType areaType;
                try
                {
                    areaType = (AreaType)Enum.Parse(typeof(AreaType), areaTypeStr, true); // Case-insensitive parsing
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Invalid AreaType value '{areaTypeStr}'. Error: {ex.Message}. Skipping this row.");
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
            //Debug.Log($"Loaded {kvp.Value.Count} squads for Area Type: {kvp.Key}");
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
        AddCrawlerGroupIfPresent(crawlerGroups, row, "Bomber", CrawlerType.Bomber);
        AddCrawlerGroupIfPresent(crawlerGroups, row, "Spore", CrawlerType.Spore);

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
        if (!row.ContainsKey(key) || row[key] == null)
        {
            return false;
        }

        // Try direct conversion first if it's already a number type
        try
        {
            value = Convert.ToInt32(row[key]);
            return true;
        }
        catch
        {
            // Not directly convertible, try string parsing approaches
        }

        string strValue = row[key].ToString().Trim();
        if (string.IsNullOrEmpty(strValue))
        {
            return false;
        }

        // Try parsing with invariant culture first
        if (int.TryParse(strValue, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
        {
            return true;
        }

        // Try with current culture as fallback
        if (int.TryParse(strValue, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
        {
            Debug.Log($"Parsed '{strValue}' as {value} using current culture");
            return true;
        }

        // Try replacing commas with periods and vice versa as last resort
        string alteredValue = strValue.Replace(',', '.');
        if (alteredValue != strValue)
        {
            if (float.TryParse(alteredValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatVal))
            {
                value = Mathf.RoundToInt(floatVal);
                Debug.Log($"Parsed '{strValue}' as {value} after replacing commas with periods");
                return true;
            }
        }

        alteredValue = strValue.Replace('.', ',');
        if (alteredValue != strValue)
        {
            if (float.TryParse(alteredValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatVal))
            {
                value = Mathf.RoundToInt(floatVal);
                Debug.Log($"Parsed '{strValue}' as {value} after replacing periods with commas");
                return true;
            }
        }

        Debug.LogWarning($"Failed to parse integer value from '{strValue}' for key '{key}'");
        return false;
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