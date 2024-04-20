using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerPart : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CashCollector.Instance.AddCrawlerPart();
            Destroy(gameObject);
        }
    }
}
