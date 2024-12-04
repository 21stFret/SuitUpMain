using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableRock : Prop
{
    public GameObject[] loot;
    public float lootChance;
    public ParticleSystem explosionEffect;
    public AudioClip[] audioClips;
    public AudioSource explosionSound;
    public BreakableObject breakableObject;

    public void Start()
    {
        Init();
        breakableObject.transform.parent = this.transform;
    }

    public override void Die()
    {
        if (Random.value * 100 <= lootChance)
        {
            GameObject randomLoot = loot[Random.Range(0, loot.Length)];
            Instantiate(randomLoot, transform.position, Quaternion.identity);
        }
        breakableObject.transform.parent = null;
        breakableObject.Break();
        explosionSound.clip = audioClips[Random.Range(0, audioClips.Length)];
        explosionSound.Play();
        explosionEffect.Play();
        gameObject.SetActive(false);
    }
}
