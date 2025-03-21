using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidGrenade : Grenade
{
    public List<TargetHealth> targets = new List<TargetHealth>();
    public ParticleSystem constantEffect;
    public float timer;
    public float fireRate;
    public float attractionStrength;

    public override void Init(float _damage, float _range)
    {
        base.Init(_damage, _range);
        constantEffect.Play();
        targets.Clear();
        rb.constraints = RigidbodyConstraints.None;
        StartCoroutine(GatherEnemies());
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
            item.TakeDamage(damage, WeaponType.Plasma, 1);
        }
        targets.Clear();
    }

    private void Update()
    {
        if(!live)
        {
            timer = 0;
            targets.Clear();
            return;
        }
        timer += Time.deltaTime;
        if (timer >= fireRate)
        {
            foreach (var item in targets)
            {
                if (item == null)
                {
                    targets.Remove(item);
                    continue;
                }
                item.TakeDamage(damage, WeaponType.Plasma, fireRate);
            }
            timer = 0;
        }
        AttractEnemies();
        //PlayLiveAudio();
    }


    private IEnumerator GatherEnemies()
    {
        while (live)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, range*0.2f, layerMask);
            foreach (Collider collider in colliders)
            {
                TargetHealth targetHealth = collider.GetComponent<TargetHealth>();
                if (targetHealth != null && !targets.Contains(targetHealth))
                {
                    targets.Add(targetHealth);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
        targets.Clear();
    }

    private void AttractEnemies()
    {
        if (targets.Count == 0)
        {
            return;
        }
        foreach (var item in targets)
        {
            if (item == null)
            {
                targets.Remove(item);
                continue;
            }
            Vector3 direction = (transform.position - item.transform.position).normalized;
            item.GetComponent<Rigidbody>().AddForce(direction * attractionStrength, ForceMode.Acceleration);
        }
    }

    private void PlayLiveAudio()
    {
        /*
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
        */
    }
}
