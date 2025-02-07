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

    public void Strike(Vector3 strikeLoc)
    {
        transform.parent = null;
        Collider[] colliders = Physics.OverlapSphere(strikeLoc, explosionRadius, layerMask);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, strikeLoc, explosionRadius, explosionUpward);
                rb.AddTorque(Vector3.forward * 3f, ForceMode.Impulse);
            }
            TargetHealth targetHealth = hit.GetComponent<TargetHealth>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage, WeaponType.Lightning);
            }
        }
        strikeLoc = new Vector3(strikeLoc.x, strikeLoc.y + 10, strikeLoc.z);
        strikeEffect.transform.parent = null;
        strikeEffect.transform.position = strikeLoc;
        strikeEffect.Play();
        thunderSound.clip = audioClips[Random.Range(0, audioClips.Length)];
        thunderSound.pitch = Random.Range(0.7f, 1.2f);
        thunderSound.volume = Random.Range(0.8f, 1.2f);
        thunderSound.Play();
    }
}
