using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDataReader : MonoBehaviour
{
    public static BattleDataReader instance;
    private int IDCatch;
    public List<Battle> battles;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        LoadFromExcell(battles);
    }

    public void LoadFromExcell(List<Battle> battles)
    {
        List<Dictionary<string, object>> data = CSVReader.Read("WeaponBaseData - Battles");
        for (var i = 0; i < data.Count; i++)
        {
            if (IDCatch == (int)data[i]["ID"])
            {
                continue;
            }
            IDCatch = (int)data[i]["ID"];
            Battle battle = battles[IDCatch-1];

            string type = data[i]["Type"].ToString();
            Enum.TryParse(type, out BattleType battleType);
            battle.battleType = battleType;

            battle.ID = (int)data[i]["ID"];

            battle.battleWaves = new List<BattleWave>();
            var waveCount = (int)data[i]["Wave"];



            for (int j = 0; j < waveCount; j++)
            {
                int cralwerTypeCount = (int)data[i+j]["Count"];
                CrawlerWave[] crawlersTypesInWave = new CrawlerWave[cralwerTypeCount];
                int k = 0;
                for (int t = 0; t < 7; t++)
                {
                    if(k == cralwerTypeCount)
                    {
                        break;
                    }
                    switch (t)
                    {
                        case 0:
                            if ((int)data[i + j]["Swarm"] != 0)
                            {
                                crawlersTypesInWave[k] = new CrawlerWave(CrawlerType.Crawler, ((int)data[i + j]["Swarm"]));
                                k++;
                            }
                            break;
                        case 1:
                            if ((int)data[i + j]["Daddy"] != 0)
                            {
                                crawlersTypesInWave[k] = new CrawlerWave(CrawlerType.Daddy, ((int)data[i + j]["Daddy"]));
                                k++;
                            }
                            break;
                        case 2:
                            if ((int)data[i + j]["Spitter"] != 0)
                            {
                                crawlersTypesInWave[k] = new CrawlerWave(CrawlerType.Spitter, ((int)data[i + j]["Spitter"]));
                                k++;
                            }
                            break;
                        case 3:
                            if ((int)data[i + j]["Charger"] != 0)
                            {
                                crawlersTypesInWave[k] = new CrawlerWave(CrawlerType.Charger, ((int)data[i + j]["Charger"]));
                                k++;
                            }
                            break;
                        case 4:
                            if ((int)data[i + j]["Hunter"] != 0)
                            {
                                //crawlersInWave[k] = new CrawlerWave(CrawlerType.Hunter, ((int)data[i + k]["Hunter"]));
                            }
                            break;
                        case 5:
                            if ((int)data[i + j]["Leaper"] != 0)
                            {
                                //crawlersInWave[k] = new CrawlerWave(CrawlerType.Leaper, ((int)data[i + k]["Leaper"]));
                            }
                            break;
                        case 6:
                            if ((int)data[i + j]["Albino"] != 0)
                            {
                                crawlersTypesInWave[k] = new CrawlerWave(CrawlerType.Albino, ((int)data[i + j]["Albino"]));
                                k++;
                            }
                            break;
                        case 7:
                            break;
                    }

                }
                battle.battleWaves.Add(new BattleWave(crawlersTypesInWave, 10));
            }
        }
    }

}
