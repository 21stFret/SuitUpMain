using UnityEngine;
using FORGE3D;
using System.Collections.Generic;


public class LightningRodController : MechWeapon
{

    public RaycastHit hit;
    [Header("Lightning Rod")]
    public List<Crawler> crawlers = new List<Crawler>();
    public int crawlerIndex;
    public int chainAmount;
    public float chainRange;
    public List<GameObject> lightningChains;
    public bool hitSwitch;
    public float timer;
    public LayerMask crawlerLayer;
    public GameObject lightning;
    public GameObject crawlerHit;
    public Vector3 hitOffset;

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
            if(timer >= speed)
            {
                Zap();
                timer = 0;
            }
        }
    }

    private void Zap()
    {
        for (int i = 0; i < crawlers.Count; i++)
        {
            crawlers[i].TakeDamage(damage, speed);
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
                crawlers.Add(crawlerHit.GetComponent<Crawler>());
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
        Collider[] colliders = Physics.OverlapSphere(nextHit.position, chainRange*2, crawlerLayer);
        for(int i = 0; i< colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Enemy"))
            {
                if (crawlers.Find(x => x.transform == colliders[i].transform))
                {
                    print("Already Hit");
                    continue;
                }

                crawlerIndex++;
                if (crawlerIndex >= chainAmount)
                {
                    print("Max Chain");
                    LightningLinkCrawlers();
                    return;
                }
                crawlers.Add(colliders[i].gameObject.GetComponent<Crawler>());
            }
        }
        print("Reached " + crawlerIndex + " / " + colliders.Length);
        LightningLinkCrawlers();
        return;
    }

    private void LightningLinkCrawlers()
    {
        print("Linked to Crawlers");
        for(int i = 0; i < crawlers.Count; i++)
        {
            if (i+1< crawlers.Count)
            {
                var newGO = lightningChains[i];
                newGO.SetActive(true);
                newGO.transform.position = hit.transform.position + hitOffset;
                newGO.transform.forward = crawlers[i+1].transform.position - newGO.transform.position;
            }
        }
    }

    private void UnlinkAllLightning()
    {
        print("Unlinked");
        hitSwitch = false;
        crawlerIndex = 0;
        crawlers.Clear();
        for (int i = 0; i < lightningChains.Count; i++)
        {
            lightningChains[i].gameObject.SetActive(false);
            lightningChains[i].transform.forward = Vector3.forward;
        }
    }
    
}
