using UnityEngine;
using FORGE3D;
using System.Collections.Generic;
using System;


public class LightningRodController : MechWeapon
{

    public RaycastHit hit;
    [Header("Lightning Rod")]
    public List<TargetHealth> targets = new List<TargetHealth>();
    public int crawlerIndex;
    public int chainAmount;
    public float stunTime;
    public float lightningRange;
    public List<GameObject> lightningChains;
    public bool hitSwitch;
    public float timer;
    public LayerMask crawlerLayer;
    public GameObject lightning;
    public GameObject crawlerHit;
    public Vector3 hitOffset;
    public Vector3 raycastOffset;

    public override void Init()
    {
        base.Init();
        lightning.SetActive(false);
        for (int i = 0; i < lightningChains.Count; i++)
        {
            lightningChains[i].transform.parent = null;
        }
    }


    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
        lightning.SetActive(true);
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        lightning.SetActive(false);
        UnlinkAllLightning();
    }

    private void Update()
    {
        if (hitSwitch)
        {
            timer += Time.deltaTime;
            if(timer >= fireRate)
            {
                Zap();
                timer = 0;
            }
        }
    }

    private void Zap()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].TakeDamage(damage, WeaponType.Lightning, stunTime);
        }
    }

    public void LightningHit(RaycastHit hitt)
    {
        if(hitt.collider == null)
        {
            UnlinkAllLightning();
            return;
        }

        if (hitt.collider.CompareTag("Enemy"))
        {
            hit = hitt;
            if (crawlerHit == null)
            {
                crawlerHit = hitt.collider.gameObject;
            }

            if(crawlerHit != hitt.collider.gameObject)
            {
                UnlinkAllLightning();
                crawlerHit = hitt.collider.gameObject;
                return;
            }

            if(!hitSwitch)
            {
                targets.Add(crawlerHit.GetComponent<TargetHealth>());
                hitSwitch = true;
            }
            LightningArc(crawlerHit.transform);
        }
        else
        {
            UnlinkAllLightning();
        }
    }

    public void LightningArc(Transform nextHit)
    {
        Collider[] colliders = Physics.OverlapSphere(nextHit.position, lightningRange, crawlerLayer);
        Array.Sort(colliders, (x, y) => Vector3.Distance(nextHit.position, x.transform.position).CompareTo(Vector3.Distance(nextHit.position, y.transform.position)));
        for(int i = 0; i< colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Enemy"))
            {
                if (targets.Find(x => x.transform == colliders[i].transform))
                {
                    continue;
                }

                crawlerIndex++;
                if (crawlerIndex >= chainAmount)
                {
                    LightningLinkCrawlers();
                    return;
                }
                targets.Add(colliders[i].gameObject.GetComponent<TargetHealth>());
            }
        }
        //print("Reached " + crawlerIndex + " / " + colliders.Length);
        LightningLinkCrawlers();
    }

    private void LightningLinkCrawlers()
    {
        for(int i = 0; i < targets.Count; i++)
        {
            if (i+1< targets.Count)
            {
                var newGO = lightningChains[i];
                newGO.SetActive(true);
                newGO.transform.position = hit.transform.position + hitOffset;
                newGO.transform.forward = targets[i+1].transform.position - newGO.transform.position;
                newGO.GetComponent<F3DLightning>().SetTarget(targets[i+1].transform);
            }
        }
    }

    private void UnlinkAllLightning()
    {
        hitSwitch = false;
        crawlerIndex = 0;
        targets.Clear();
        for (int i = 0; i < lightningChains.Count; i++)
        {
            lightningChains[i].gameObject.SetActive(false);
        }
    }
    
}
