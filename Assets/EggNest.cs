using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggNest : Prop
{
    public CrawlerBurstSpawner burstSpawner;

    private void Start()
    {
        burstSpawner.Init();
    }

}
