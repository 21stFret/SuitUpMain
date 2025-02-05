using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : Prop
{
    public WeaponType weaponType;
    public float explosionForce;
    public float explosionRadius;
    public float upwardsModifier;
    public float damage;
    public ParticleSystem explosionEffect;
    public LayerMask layerMask;
    public GameObject prefab;
    public float fuseTime;
    private float _fuseTimer;
    public int fuseCount;
    public GameObject explosionRadiusPrefab;
    public Light warningLight;
    public bool isFuseActive;
    public float flashTime;
    public AudioClip[] audioClips;
    public AudioSource explosionSound;
    public AudioClip warningNoise;
    public BreakableObject breakableObject;
    public DamageArea damageArea;
    private Rigidbody rb;
    private int _currentFuseCount;

    private void Start()
    {
        this.Init();
    }

    public override void Init()
    {
        base.Init();
        isFuseActive = false;
        warningLight.enabled = false;
        explosionRadiusPrefab.SetActive(false);
        explosionEffect.transform.parent = this.transform;
        explosionEffect.Stop();
        explosionSound.Stop();
        breakableObject.transform.parent = this.transform;
        rb = GetComponent<Rigidbody>();
        _fuseTimer = 0;
        _currentFuseCount = fuseCount;
    }

    public override void Die()
    {
        isFuseActive = true;
    }

    private void Update()
    {
        if (isFuseActive)
        {
            TimerDelay();
        }
    }

    private void Explode()
    {
        bool isPlayer = false;
        isFuseActive = false;
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
        foreach (Collider collider in colliders)
        {
            if (isPlayer)
            {
                continue;
            }
            TargetHealth targetHealth = collider.GetComponent<TargetHealth>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage, weaponType);
            }
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
            if (collider.CompareTag("Player"))
            {
                isPlayer = true;
            }
        }
        breakableObject.transform.parent = null;
        breakableObject.Break();
        explosionSound.clip = audioClips[Random.Range(0, audioClips.Length)];
        explosionSound.pitch = Random.Range(0.7f, 1.2f);
        explosionSound.Play();
        explosionEffect.transform.parent = null;
        explosionEffect.transform.position = transform.position;
        explosionEffect.Play();
        prefab.SetActive(false);
        if (damageArea != null)
        {
            damageArea.EnableDamageArea();
        }
    }

    public void TimerDelay()
    {
        if(_currentFuseCount>0)
        {
            if (_fuseTimer < flashTime)
            {
                warningLight.enabled = true;
                explosionRadiusPrefab.SetActive(true);
                if (!explosionSound.isPlaying)
                {
                    explosionSound.clip = warningNoise;
                    explosionSound.Play();
                }
            }
            else
            {
                warningLight.enabled = false;
                explosionRadiusPrefab.SetActive(false);
            }
        }
        else
        {
            warningLight.enabled = false;
            explosionRadiusPrefab.SetActive(false);
        }
        if (_fuseTimer >= fuseTime)
        {
            _currentFuseCount--;
            if (_currentFuseCount <= 0)
            {
                Explode();
                return;
            }
            else
            {
                _fuseTimer = 0;
            }
        }

        _fuseTimer += Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        // Store original gizmo color
        Color originalColor = Gizmos.color;

        // Set the color (you can adjust these values)
        Gizmos.color = Color.magenta;

        // Draw a wire sphere for the radius
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        // Optional: Draw lines for cardinal directions
        Gizmos.DrawLine(transform.position, transform.position + Vector3.forward * explosionRadius);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * explosionRadius);

        // Restore original color
        Gizmos.color = originalColor;
    }

    public override void RefreshProp()
    {
        base.RefreshProp();
        explosionEffect.transform.parent = this.transform;
        explosionEffect.Stop();
        explosionSound.Stop();
        breakableObject.transform.parent = this.transform;
        prefab.SetActive(true);
        warningLight.enabled = false;
        explosionRadiusPrefab.SetActive(false);
        _fuseTimer = 0;
        _currentFuseCount = fuseCount;
    }
}
