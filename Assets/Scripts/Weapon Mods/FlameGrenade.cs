using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameGrenade : MonoBehaviour
{
    public float damage;
    public float range;
    public float explosionForce;
    public float explosionRadius;
    public float explosionUpward;
    public LayerMask layerMask;
    public ParticleSystem explosionEffect;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private Collider col;
    private bool cashed;
    public AudioSource explosionSound;
    public AudioClip[] audioClips;

    private void Awake()
    {
        if (!cashed)
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponentInChildren<Collider>();
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            cashed = true;
        }
    }

    public void Init(float _damage, float _range)
    {
        if (!cashed)
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponentInChildren<Collider>();
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            cashed = true;
        }
        rb.velocity = Vector3.zero;
        meshRenderer.enabled = true;
        col.enabled = true;
        damage = _damage;
        range = _range;
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        rb.AddForce(transform.forward * range, ForceMode.Impulse);
        yield return new WaitForSeconds(3);
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward);
                rb.AddTorque(Vector3.forward * 3f, ForceMode.Impulse);
            }
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<Crawler>().DealyedDamage(damage, 0.5f, WeaponType.Grenade);
            }
        }
        meshRenderer.enabled = false;
        col.enabled = false;
        explosionEffect.Play();
        explosionSound.clip = audioClips[Random.Range(0, audioClips.Length)];
        explosionSound.Play();

    }

}