using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;

namespace FORGE3D
{
    public class F3DProjectile : MonoBehaviour
    {
        public F3DFXType fxType; // Weapon type 
        public LayerMask layerMask;
        public float lifeTime = 5f; // Projectile life time
        public float despawnDelay; // Delay despawn in ms
        public float velocity = 100f; // Projectile velocity
        public float RaycastAdvance = 2f; // Raycast advance multiplier 
        public bool DelayDespawn = false; // Projectile despawn flag 
        public ParticleSystem[] delayedParticles; // Array of delayed particles
        ParticleSystem[] particles; // Array of projectile particles 
        new Transform transform; // Cached transform 
        RaycastHit hitPoint; // Raycast structure 
        bool isHit = false; // Projectile hit flag
        bool isFXSpawned = false; // Hit FX prefab spawned flag 
        float timer = 0f; // Projectile timer
        float fxOffset; // Offset of fxImpact

        public float impactDamage;
        public float impactForce;
        public int pierceCount;
        public float stunTime;
        public float sphereCastRadius =0.2f;

        public ProjectileWeapon _weaponController;
        public WeaponType weaponType;

        public List<GameObject> hitObjects = new List<GameObject>();
        public int bounceCount;

        public bool shockRounds;
        public float shockDamage;

        void Awake()
        {
            // Cache transform and get all particle systems attached
            transform = GetComponent<Transform>();
            particles = GetComponentsInChildren<ParticleSystem>();
        }

        // OnSpawned called by pool manager 
        public void OnSpawned()
        {
            // Reset flags and raycast structure
            isHit = false;
            isFXSpawned = false;
            timer = 0f;
            hitPoint = new RaycastHit();
            hitObjects.Clear();
        }

        // OnDespawned called by pool manager 
        public void OnDespawned()
        {
        }

        // Stop attached particle systems emission and allow them to fade out before despawning
        void Delay()
        {
            if (particles.Length > 0 && delayedParticles.Length > 0)
            {
                bool delayed;
                for (int i = 0; i < particles.Length; i++)
                {
                    delayed = false;
                    for (int y = 0; y < delayedParticles.Length; y++)
                        if (particles[i] == delayedParticles[y])
                        {
                            delayed = true;
                            break;
                        }

                    particles[i].Stop(false);
                    if (!delayed)
                        particles[i].Clear(false);
                }
            }
        }

        // OnDespawned called by pool manager 
        void OnProjectileDestroy()
        {
            F3DPoolManager.Pools["GeneratedPool"].Despawn(transform);
        }

        // Apply hit force on impact
        void ApplyForce(float force, float stunTime)
        {
            TargetHealth targetHealth = hitPoint.collider.GetComponent<TargetHealth>();

            if(targetHealth != null)
            {
                targetHealth.TakeDamage(impactDamage, weaponType, stunTime);
                if (hitPoint.rigidbody != null)
                {
                    hitPoint.rigidbody.AddForceAtPosition(transform.forward * force, hitPoint.point, ForceMode.Impulse);

                }
            }
        }

        private void ShockRounds()
        {
            var colliders = Physics.OverlapSphere(hitPoint.point, 2f, layerMask);
            foreach (var collider in colliders)
            {
                if (collider.GetComponent<TargetHealth>() != null)
                {
                    collider.GetComponent<TargetHealth>().TakeDamage(shockDamage, WeaponType.Lightning, stunTime);
                }
            }
            F3DAudioController.instance.LightningGunHit(hitPoint.point);
            _weaponController.Impact(hitPoint.point + hitPoint.normal * fxOffset, weaponType);
        }

        void Update()
        {
            // If something was hit
            if (isHit)
            {

                // Execute once
                if (!isFXSpawned)
                {
                    _weaponController.Impact(hitPoint.point + hitPoint.normal * fxOffset, weaponType);
                    isFXSpawned = true;

                    if(hitPoint.collider.gameObject.layer == 0)
                    {
                        return;
                    }
                    ApplyForce(impactForce, stunTime);
                    if (shockRounds)
                    {
                        Invoke("ShockRounds", 0.2f);
                    }

                }

                // Despawn current projectile 
                if (!DelayDespawn || (DelayDespawn && (timer >= despawnDelay)))
                    OnProjectileDestroy();
            }

            // No collision occurred yet
            else
            {
                // Projectile step per frame based on velocity and time
                Vector3 step = transform.forward * Time.deltaTime * velocity;
                if(step.magnitude > RaycastAdvance)
                {
                    step = step.normalized * RaycastAdvance;
                }

                SphereCastForColliders(sphereCastRadius, layerMask);

                if (isHit)
                {
                    return;
                }

                if (timer >= lifeTime)
                {
                    OnProjectileDestroy();
                }

                // Advances projectile forward
                transform.position += step;
            }

            // Updates projectile timer
            timer += Time.deltaTime;
        }

        //Set offset
        public void SetOffset(float offset)
        {
            fxOffset = offset;
        }
        void SphereCastForColliders(float radius, LayerMask layerMask)
        {
            if (Physics.SphereCast(transform.position, radius, transform.forward, out hitPoint, RaycastAdvance, layerMask))
            {
                if (pierceCount > 0)
                {
                    if (!hitObjects.Contains(hitPoint.collider.gameObject))
                    {
                        hitObjects.Add(hitPoint.collider.gameObject);
                        _weaponController.Impact(hitPoint.point + hitPoint.normal * fxOffset, weaponType);
                        ApplyForce(impactForce, stunTime);
                        pierceCount--;
                    }
                    return;
                }
                if (bounceCount > 0)
                {
                    if (!hitObjects.Contains(hitPoint.collider.gameObject))
                    {
                        hitObjects.Add(hitPoint.collider.gameObject);
                        _weaponController.Impact(hitPoint.point + hitPoint.normal * fxOffset, weaponType);
                        ApplyForce(impactForce, stunTime);
                        bounceCount--;
                        transform.forward = Vector3.Reflect(transform.forward, hitPoint.normal);
                        Vector3 newDirection = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
                        transform.forward = newDirection;
                    }
                    return;
                }

                isHit = true;
            }
        }
    }
}