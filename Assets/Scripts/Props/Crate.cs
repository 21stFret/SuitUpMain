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

    public void Start()
    {
        Init();
        breakableObject.transform.parent = this.transform;
    }

    public override void Die()
    {
        base.Die();
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
        StartCoroutine(DelayedDestroy(1.0f));
    }

    private IEnumerator DelayedDestroy(float delay)
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

}
