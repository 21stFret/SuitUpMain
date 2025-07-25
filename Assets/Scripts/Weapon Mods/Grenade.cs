using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float damage;
    public float range;
    public float explosionForce;
    public float explosionRadius;
    public float explosionUpward;
    public float fuseTime;
    public bool explodeOnImpact;
    public WeaponType weaponType;
    public LayerMask layerMask;
    public ParticleSystem explosionEffect;
    protected Rigidbody rb;
    protected MeshRenderer meshRenderer;
    protected Collider col;
    private bool cashed;
    public AudioSource explosionSound;
    public AudioClip[] audioClips;
    protected bool live;

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

    public virtual void Init(float _damage, float _range)
    {
        if (!cashed)
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponentInChildren<Collider>();
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            cashed = true;
        }
        live = true;
        rb.velocity = Vector3.zero;
        if(meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }
        col.enabled = true;
        damage = _damage;
        range = _range;
        rb.AddForce(transform.forward * range, ForceMode.VelocityChange);
        if (!explodeOnImpact)
        {
            StartCoroutine(ExplodeDelay());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (explodeOnImpact)
        {
            Explode();
        }
    }

    private IEnumerator ExplodeDelay()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    public virtual void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
        int crawlersHit = 0;
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpward, ForceMode.Impulse);
                rb.AddTorque(Vector3.forward * 3f, ForceMode.Impulse);
            }
            hit.GetComponent<TargetHealth>()?.TakeDamage(damage, weaponType);
            if (hit.GetComponent<Crawler>() != null)
            {
                crawlersHit++;
            }
        }
        PlayerProgressManager.instance?.CheckMultiKill(crawlersHit);
        live = false;
        meshRenderer.enabled = false;
        col.enabled = false;
        explosionEffect.transform.position = transform.position;
        explosionEffect.transform.SetParent(null);
        explosionEffect.transform.up = Vector3.up;
        explosionEffect.Play();
        explosionSound.clip = audioClips[Random.Range(0, audioClips.Length)];
        explosionSound.Play();
    }
}
