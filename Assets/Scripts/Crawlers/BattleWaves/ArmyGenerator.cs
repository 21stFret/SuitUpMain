using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ArmyGenerator : MonoBehaviour
{
    public List<CrawlerSquad> currentAreaSquads = new List<CrawlerSquad>();
    public List<CrawlerSquad> batttleArmy = new List<CrawlerSquad>();
    public int MaxSquads = 3;

    public List<CrawlerSquad> BuildArmy()
    {
        batttleArmy.Clear();
        int spreader = 0;
        for (int i = 0; i < MaxSquads; i++)
        {
            int value;
            if (spreader == 0)
            {
                value = Random.Range(0, currentAreaSquads.Count /2);
            }
            else
            {
                value = Random.Range(spreader, currentAreaSquads.Count);
            }
            batttleArmy.Add(currentAreaSquads[value]);
            spreader = value;
        }
        return batttleArmy;
    }
}