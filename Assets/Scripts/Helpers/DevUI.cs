using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DevUI : MonoBehaviour
{
    public CrawlerSpawner crawlerSpawner;
    public TMP_Text crawlerCountText;

    // Update is called once per frame
    void Update()
    {
        crawlerCountText.text = "Crawler Count: " + crawlerSpawner.activeCrawlerCount;
    }
}
