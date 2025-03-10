using FORGE3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningOrb : Grenade
{
    public List<TargetHealth> targets = new List<TargetHealth>();
    public int chainAmount;
    public float stunTime;
    public float fireRate; 
    public List<GameObject> lightningChains;
    private float timer;
    public AudioClip liveShock;
    public ParticleSystem constantEffect;

    public override void Init(float _damage, float _range)
    {
        base.Init(_damage, _range);
        constantEffect.Play();
        targets.Clear();
        foreach (var item in lightningChains)
        {
            item.GetComponent<F3DLightning>().MaxBeamLength = range;
            item.SetActive(false);
        }
        rb.constraints = RigidbodyConstraints.None;
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public override void Explode()
    {
        live = false;
        meshRenderer.enabled = false;
        col.enabled = false;
        explosionEffect.Play();
        explosionSound.clip = audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
        explosionSound.Play();
        explosionSound.loop = false;
        constantEffect.Stop();
        foreach (var item in targets)
        {
            item.TakeDamage(damage, WeaponType.Lightning, 1);
        }
        targets.Clear();
        foreach (var item in lightningChains)
        {
            item.SetActive(false);
        }
    }

    private void Update()
    {
        if(!live)
        {
            timer = 0;
            targets.Clear();
            foreach (var item in lightningChains)
            {
                item.SetActive(false);
            }
            return;
        }
        timer += Time.deltaTime;
        if (timer >= fireRate)
        {
            Zap();
            timer = 0;
        }
        LightningArc();
        PlayLiveAudio();
    }

    private void PlayLiveAudio()
    {
        if(targets.Count == 0 && explosionSound.clip == liveShock)
        {
            explosionSound.Stop();
            return;
        }
        if (!explosionSound.isPlaying)
        {
            explosionSound.loop = true;
            explosionSound.clip = liveShock;
            explosionSound.Play();
        }
    }

    public void LightningArc()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, layerMask);
        Array.Sort(colliders, (x, y) => Vector3.Distance(transform.position, x.transform.position).CompareTo(Vector3.Distance(transform.position, y.transform.position)));
        targets.Clear();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Enemy"))
            {

                if (targets.Find(x => x.transform == colliders[i].transform))
                {
                    continue;
                }
   
                if (targets.Count >= chainAmount)
                {
                    LightningLinkCrawlers();
                    return;
                }
                targets.Add(colliders[i].gameObject.GetComponent<TargetHealth>());
            }
        }
        LightningLinkCrawlers();
    }

    private void LightningLinkCrawlers()
    {
        for (int i = 0; i < lightningChains.Count; i++)
        {
            if(i >= targets.Count)
            {
                lightningChains[i].SetActive(false);
                continue;
            }
            var newGO = lightningChains[i];
            newGO.SetActive(true);
            newGO.transform.position = transform.position;
            newGO.transform.forward = targets[i].transform.position - newGO.transform.position;
            newGO.GetComponent<F3DLightning>().SetTarget(targets[i].transform);
        }
    }

    private void Zap()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].TakeDamage(damage, WeaponType.Lightning, stunTime);
        }
    }
}
