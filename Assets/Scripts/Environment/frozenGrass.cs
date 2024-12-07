using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frozenGrass : Prop
{
    public DamageArea iceDamage;
    public ParticleSystem explosionEffect;
    public AudioClip[] audioClips;
    public AudioSource explosionSound;
    public BreakableObject breakableObject;
    public GameObject toHide;

    public void Start()
    {
        Init();
        breakableObject.transform.parent = this.transform;
    }

    public override void Die()
    {
        base.Die();
        iceDamage.EnableDamageArea();
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

        if(toHide != null)
        {
            toHide.SetActive(false);
        }
        else
        {
            GetComponent<Renderer>().enabled = false;
        }

        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Die();
        }
    }
}
