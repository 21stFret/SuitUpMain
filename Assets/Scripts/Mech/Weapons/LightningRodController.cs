using UnityEngine;
using FORGE3D;
using System.Collections.Generic;
using System;
using System.Linq;


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
    public GameObject lightningGO;
    public F3DLightning lightning;
    public GameObject crawlerHit;
    public Vector3 hitOffset;
    public Vector3 raycastOffset;
    public bool arcOverride;
    public float aimAssit;

    public override void Init()
    {
        base.Init();
        lightningGO.SetActive(false);
        lightning.layerMask = crawlerLayer;
        arcOverride = false;
        weaponType = WeaponType.Lightning;
        for (int i = 0; i < lightningChains.Count; i++)
        {
            lightningChains[i].transform.parent = null;
        }
    }


    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
        if(weaponOverride)
        {
            return;
        }
        lightningGO.SetActive(true);
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        if (weaponOverride)
        {
            return;
        }
        lightningGO.SetActive(false);
        UnlinkAllLightning();
    }

    private void Update()
    {
        if (weaponOverride)
        {
            return;
        }
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
            if (targets[i] == null)
            {
                targets.RemoveAt(i);
                continue;
            }
            float dam = damage;
            if(i>0)
            {
                dam = damage / 2;
            }
            targets[i].TakeDamage(dam, WeaponType.Lightning, stunTime);
        }
    }

    public void LightningHit(RaycastHit hitt)
    {
        if(hitt.collider == null)
        {
            UnlinkAllLightning();
            return;
        }

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
            TargetHealth target = crawlerHit.GetComponent<TargetHealth>();
            if(target == null)
            {
                UnlinkAllLightning();
                return;
            }
            targets.Add(target);
            hitSwitch = true;
        }
        if(arcOverride)
        {
            return;
        }
        LightningArc(crawlerHit.transform);
    }

    public void LightningArc(Transform nextHit)
    {
        if(targets.Count == 0)
        {
            return;
        }
        if (nextHit == null)
        {
            return;
        }
        Collider[] colliders = Physics.OverlapSphere(nextHit.position, lightningRange, crawlerLayer);
        List<Collider> list = new List<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.GetComponent<TargetHealth>() != null)
            {
                list.Add(colliders[i]);
            }
        }
        list.Sort((x, y) => Vector3.Distance(nextHit.position, x.transform.position).CompareTo(Vector3.Distance(nextHit.position, y.transform.position)));
        //Array.Sort(colliders, (x, y) => Vector3.Distance(nextHit.position, x.transform.position).CompareTo(Vector3.Distance(nextHit.position, y.transform.position)));
        for(int i = 0; i< list.Count; i++)
        {
            if (targets.Find(x => x.transform == list[i].transform))
            {
                continue;
            }

            crawlerIndex++;
            if (crawlerIndex >= chainAmount)
            {
                LightningLinkCrawlers();
                return;
            }
            targets.Add(list[i].gameObject.GetComponent<TargetHealth>());
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
