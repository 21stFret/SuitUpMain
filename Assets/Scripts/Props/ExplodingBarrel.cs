using System.Collections;
using System.Collections.Generic;
using DTT.AreaOfEffectRegions;
using FORGE3D;
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
    public CircleRegion damageAreaRegion;
    public Light warningLight;
    public bool isFuseActive;
    public float flashTime;
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
        rb = GetComponent<Rigidbody>();
        isFuseActive = false;
        warningLight.enabled = false;
        explosionEffect.transform.parent = this.transform;
        explosionEffect.Stop();
        if (breakableObject != null)
        {
            breakableObject.transform.parent = this.transform;
        }
        _fuseTimer = 0;
        _currentFuseCount = fuseCount;
    }

    public override void Die()
    {
        isFuseActive = true;
        if (damageAreaRegion != null)
        {
            damageAreaRegion.FillProgress = 0;
            damageAreaRegion.gameObject.SetActive(true);
            damageAreaRegion.transform.SetParent(null);
            damageAreaRegion.transform.rotation = Quaternion.Euler(Vector3.up);
        }
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
        base.Die();
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
                float damageByDistance = damage * Mathf.Clamp(1 - Vector3.Distance(transform.position, collider.transform.position) / explosionRadius, 0.5f, 1);
                Crawler crawler = collider.GetComponent<Crawler>();
                if (crawler != null)
                {
                    crawler.DealyedDamage(damageByDistance , 0.5f, WeaponType.AoE);
                    continue;
                }
                targetHealth.TakeDamage(damageByDistance, weaponType);
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
        if (breakableObject != null)
        {
            breakableObject.transform.parent = null;
            breakableObject.transform.position = transform.position;
            breakableObject.transform.rotation = Quaternion.Euler(Vector3.up);
            breakableObject.Break();
        }

        F3DAudioController.instance.BombHit(transform.position);
        explosionEffect.transform.parent = null;
        explosionEffect.transform.position = transform.position;
        explosionEffect.transform.up = Vector3.up;
        explosionEffect.Play();
        ScreenShakeUtility.Instance.ShakeScreenDistance(transform.position);
        prefab.SetActive(false);
        if (damageArea != null)
        {
            damageArea.transform.SetParent(null);
            damageArea.transform.position = transform.position;
            damageArea.transform.rotation = Quaternion.Euler(Vector3.up);
            damageArea.EnableDamageArea();
        }
        if(damageAreaRegion != null)
        {
            damageAreaRegion.gameObject.SetActive(false);
        }
    }

    public void TimerDelay()
    {
        /*
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
        */
        _fuseTimer += Time.deltaTime;
        
        if (damageAreaRegion != null)
        {
            damageAreaRegion.transform.position = transform.position + Vector3.up * 0.5f;
            damageAreaRegion.FillProgress = _fuseTimer / fuseTime;
        }

        if (_fuseTimer >= fuseTime)
        {
            Explode();
        }
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
        if(breakableObject != null)
        {
            breakableObject.transform.parent = this.transform;
        }
        prefab.SetActive(true);
        warningLight.enabled = false;
        _fuseTimer = 0;
        _currentFuseCount = fuseCount;
    }
}
