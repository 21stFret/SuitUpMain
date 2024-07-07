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

    public void Start()
    {
        Init();
    }

    public override void Die()
    {
        if (Random.value * 100 <= lootChance)
        {
            GameObject randomLoot = loot[Random.Range(0, loot.Length)];
            Instantiate(randomLoot, transform.position, Quaternion.identity);
        }
        explosionSound.clip = audioClips[Random.Range(0, audioClips.Length)];
        explosionSound.Play();
        explosionEffect.Play();
        gameObject.SetActive(false);
    }
}
