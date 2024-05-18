using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerPart : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.CompareTag("Player"))
        {
            CashCollector.Instance.AddCrawlerPart(1);
            Destroy(gameObject);
        }
        */
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CashCollector.Instance.AddCrawlerPart(1);
            Destroy(gameObject);
        }
    }
}
