using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerSpore : Crawler
{
    public GameObject sporePrefab;
    public ParticleSystem sporeEffect;
    public DamageArea damageArea;
    public float sporeTimer = 5f;
    private float timer;
    public GameObject LargeDeathEffect;

    private bool damamgeInitialized = false;

    public override void Spawn(bool daddy = false)
    {
        base.Spawn(daddy);
        timer = sporeTimer / 2;
    }


    public override void Die(WeaponType killedBy)
    {
        sporePrefab.transform.parent = null;
        sporePrefab.transform.position = transform.position;
        sporePrefab.SetActive(true);
        sporeEffect.Play();
        damageArea.EnableDamageArea();
        StartCoroutine(SpawnSpores());
        LargeDeathEffect.transform.SetParent(null);
        LargeDeathEffect.SetActive(true);
        base.Die(killedBy);
    }

    private void Update()
    {
        if(dead)
        {
            return;
        }
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            if (!damamgeInitialized)
            {
                damageArea.Init();
                damamgeInitialized = true;
            }
            StartCoroutine(SpawnSpores());
            timer = sporeTimer;
        }
    }

    private IEnumerator SpawnSpores()
    {
        animator.SetTrigger("Spit");
        yield return new WaitForSeconds(0.1f);
        sporePrefab.transform.parent = null;
        sporePrefab.transform.position = transform.position;
        sporePrefab.SetActive(true);
        sporeEffect.Play();
        damageArea.EnableDamageArea();
    }

}
