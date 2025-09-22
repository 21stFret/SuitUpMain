using Micosmo.SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChainSaw : MechWeapon
{
    private Animator _animator;
    public ProjectileWeapon weaponController;
    public MeshRenderer meshRenderer;
    private List<TargetHealth> targetHealths = new List<TargetHealth>();
    public ParticleSystem sparks;
    private float _timer;
    public AudioSource audioSource;
    public AudioClip lowEngineHum, revingEngine;
    private Coroutine _stopEngineCoroutine;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        //meshRenderer.materials[1].SetColor("_EmissionColor", Color.white);
        bounces = 0;
    }

    public override void Init()
    {
        base.Init();
        _animator.SetBool("Enabled", false);
        weaponType = WeaponType.Chainsaw;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            return;
        }
        var health = other.GetComponent<TargetHealth>();
        if (health != null && !targetHealths.Contains(health))
        {
            targetHealths.Add(health);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            return;
        }
        var health = other.GetComponent<TargetHealth>();
        if (health != null && targetHealths.Contains(health))
        {
            targetHealths.Remove(health);
        }
    }

    void Update()
    {
        if (isFiring)
        {
            // Validate damage before using it
            if (float.IsNaN(damage) || float.IsInfinity(damage))
            {
                Debug.LogError($"ChainSaw damage is NaN/Infinity: {damage}");
                return;
            }
            
            _timer += Time.deltaTime;
            if (_timer > fireRate)
            {
                //Debug.Log($"ChainSaw dealing {damage} damage to {targetHealths.Count} targets");
                
                for (int i = targetHealths.Count - 1; i >= 0; i--)
                {
                    var health = targetHealths[i];
                    if (health == null || !health.alive)
                    {
                        targetHealths.RemoveAt(i);
                        continue;
                    }
                    
                    // Log before damage
                    float healthBefore = health.health;
                    health.TakeDamage(damage, WeaponType.Chainsaw);
                    
                    // Check if health became NaN after our damage call
                    if (float.IsNaN(health.health))
                    {
                        Debug.LogError($"Health became NaN after chainsaw damage! Before: {healthBefore}, Damage: {damage}, Target: {health.gameObject.name}");
                    }
                }
                _timer = 0.0f;
            }
        }
        else
        {
            _timer = 0.0f;
        }
    }
    // Fire Weapon
    public override void Fire()
    {
        base.Fire();
        sparks.Play();
        _animator.SetBool("Enabled", true);
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.clip = revingEngine;
            audioSource.Play();
        }
        if (_stopEngineCoroutine != null)
        {
            StopCoroutine(_stopEngineCoroutine);
            _stopEngineCoroutine = null;
        }
    }

    // Stop firing 
    public override void Stop()
    {
        base.Stop();
        sparks.Stop();
        if (audioSource != null)
        {
            audioSource.clip = lowEngineHum;
            audioSource.Play();
        }
        _stopEngineCoroutine = StartCoroutine(StopEngine());
    }

    private IEnumerator StopEngine()
    {
        yield return new WaitForSeconds(3f);
        _animator.SetBool("Enabled", false);
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        
    }
}
