using System.Collections;
using System.Collections.Generic;
using FORGE3D;
using UnityEngine;

public class Crate : Prop
{
    public GameObject[] loot;
    public float lootChance;
    public ParticleSystem explosionEffect;
    public BreakableObject breakableObject;
    private Vector3 originalPosition;
    private Vector3 originalRoation;
    private Renderer rend;
    public Rigidbody rb;

    public bool isBoulder;

    public override void Init()
    {
        base.Init();
        originalPosition = transform.position;
        originalRoation = transform.eulerAngles;
        breakableObject.transform.parent = this.transform;
        rend = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
    }

    public override void Die()
    {
        base.Die();
        if(rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        if (Random.value * 100 <= lootChance)
        {
            GameObject randomLoot = loot[Random.Range(0, loot.Length)];
            Vector3 pos = transform.position;
            pos.y += 0.5f;
            GameObject go = Instantiate(randomLoot, pos, Quaternion.identity);
            go.transform.SetParent(CashCollector.instance.crawlerPartParent.transform);
        }
        breakableObject.transform.parent = null;
        breakableObject.Break();
        if (!isBoulder)
        {
            F3DAudioController.instance.BoxHit(transform.position);
        }
        else
        {
            F3DAudioController.instance.BoulderHit(transform.position);
        }
        explosionEffect.Play();
        explosionEffect.transform.parent = null;
        if(rend != null)
        {
            Invoke("DelaySetActive", 1f);
            rend.enabled = false;
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    private void DelaySetActive()
    {
        gameObject.SetActive(false);
    }

    public override void RefreshProp()
    {
        Init();
        base.RefreshProp();
        rend.enabled = true;
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.velocity = Vector3.zero;
        }
        transform.position = originalPosition;
        transform.eulerAngles = originalRoation;
        explosionEffect.transform.parent = transform;
        explosionEffect.transform.localPosition = Vector3.zero;
        _targetHealth.alive = true;
    }
    

}
