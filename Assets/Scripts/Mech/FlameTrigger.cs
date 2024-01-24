using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameTrigger : MonoBehaviour
{
    public List<Crawler> crawlers = new List<Crawler>();
    private Collider col;
    public float shotSpeed;
    public int shotDamage;
    private bool isOn;

    private float timer;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
    }

    public void InitFlameTrigger(int damage, float speed)
    {
        shotDamage = damage;
        shotSpeed = speed;
    }

    public void SetCol(bool value)
    {
        col.enabled = value;
        isOn = value;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            crawlers.Add(other.GetComponent<Crawler>());
        }

        if (other.CompareTag("Tree"))
        {
            other.GetComponent<Tree>().TriggerOnFire();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if(crawlers.Count <= 0)
            { return; }
            crawlers.Remove(other.GetComponent<Crawler>());
        }
    }

    private void Update()
    {
        if(crawlers.Count <= 0)
        { return; }

        if (!isOn)
        {
            crawlers.Clear();
            timer = 0;
            return;
        }

        timer += Time.deltaTime;
        if(timer > shotSpeed)
        {
            foreach (Crawler crawler in crawlers)
            {
                crawler.TakeDamage(shotDamage);
            }
            timer = 0;
        }


    }
}
