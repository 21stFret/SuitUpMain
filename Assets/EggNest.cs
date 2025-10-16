using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggNest : Prop
{
    public CrawlerBurstSpawner burstSpawner;
    public GameObject EggNestModel;
    public ParticleSystem DeathEffect;


    private void Start()
    {
        Init();
        burstSpawner.Init();
    }

    public override void Die()
    {
        burstSpawner.isActive = false;
        base.Die();
        EggNestModel.SetActive(false);
        DeathEffect.transform.position = transform.position;
        DeathEffect.Play();
    }



}
