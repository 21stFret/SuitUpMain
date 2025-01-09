using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : Prop
{
    public GameObject[] loot;
    public float lootChance;
    public ParticleSystem explosionEffect;
    public AudioClip[] audioClips;
    public AudioSource explosionSound;
    public BreakableObject breakableObject;
    private Vector3 originalPosition;
    private Vector3 originalRoation;
    private Renderer rend;
    private Rigidbody rb;

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
            Instantiate(randomLoot, pos, Quaternion.identity);
        }
        breakableObject.transform.parent = null;
        breakableObject.Break();
        explosionSound.clip = audioClips[Random.Range(0, audioClips.Length)];
        explosionSound.Play();
        explosionEffect.Play();
        rend.enabled = false;
    }

    public override void RefreshProp()
    {
        Init();
        base.RefreshProp();
        rend.enabled = true;
        if(rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
        transform.position = originalPosition;
        transform.eulerAngles = originalRoation;
    }
    

}
