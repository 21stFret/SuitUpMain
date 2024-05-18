using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrike : MonoBehaviour
{
    public float damage;
    public ParticleSystem strikeEffect;
    public LayerMask layerMask;
    public AudioClip[] audioClips;
    public AudioSource thunderSound;
    public float explosionForce;
    public float explosionRadius;
    public float explosionUpward;

    public void Strike(Transform strikeLoc)
    {
        transform.parent = null;
        Collider[] colliders = Physics.OverlapSphere(strikeLoc.position, explosionRadius, layerMask);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, strikeLoc.position, explosionRadius, explosionUpward);
                rb.AddTorque(Vector3.forward * 3f, ForceMode.Impulse);
            }
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<Crawler>().DealyedDamage(damage, 0.2f, WeaponType.Lightning);
            }
        }
        strikeLoc.position = new Vector3(strikeLoc.position.x, strikeLoc.position.y + 10, strikeLoc.position.z);
        strikeEffect.transform.position = strikeLoc.position;
        strikeEffect.Play();
        thunderSound.clip = audioClips[Random.Range(0, audioClips.Length)];
        thunderSound.Play();
    }
}
