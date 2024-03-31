using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTIme : MonoBehaviour
{
    public float damage = 1;
    public float damageTime = 1;
    public WeaponType weaponType;

    private List<Crawler> crawlers = new List<Crawler>();
    private TargetHealth playerHealth;

    private void OnTriggerEnter(Collider other)
    {
        if(enabled == false)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<TargetHealth>();
            StartCoroutine(DamagePlayer());
        }
        if(other.CompareTag("Enemy"))
        {
            crawlers.Add(other.GetComponent<Crawler>());
            StartCoroutine(DamageEnemy());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = null;
        }
        if(other.CompareTag("Enemy"))
        {
            crawlers.Remove(other.GetComponent<Crawler>());
        }
    }

    private IEnumerator DamagePlayer()
    {
        while (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            yield return new WaitForSeconds(damageTime);
        }
    }

    private IEnumerator DamageEnemy()
    {
        while (crawlers.Count>1)
        {
            foreach (Crawler crawler in crawlers)
            {
                crawler.TakeDamage(damage, weaponType);
            }
            yield return new WaitForSeconds(damageTime);
        }
    }
}
