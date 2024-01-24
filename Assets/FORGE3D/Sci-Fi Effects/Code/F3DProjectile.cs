using UnityEngine;
using System.Collections;

namespace FORGE3D
{
    public class F3DProjectile : MonoBehaviour
    {
        public F3DFXType fxType; // Weapon type 
        public LayerMask layerMask;
        public float lifeTime = 5f; // Projectile life time
        public float despawnDelay; // Delay despawn in ms
        public float velocity = 300f; // Projectile velocity
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

        public int impactDamage;

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
        void ApplyForce(float force)
        {
            if (hitPoint.rigidbody == null)
            { return; }
            
            if(hitPoint.collider.CompareTag("Enemy"))
            {
                var crawler = hitPoint.collider.GetComponent<Crawler>();
                crawler.TakeDamage(impactDamage);
                // for shots that stun the crawler
                //crawler.StartCoroutine(crawler.SwitchOffNavMesh());
                hitPoint.rigidbody.AddForceAtPosition(transform.forward * force, hitPoint.point, ForceMode.Force);
            }

        }

        void Update()
        {
            // If something was hit
            if (isHit)
            {
                // Execute once
                if (!isFXSpawned)
                {
                    // Invoke corresponding method that spawns FX
                    switch (fxType)
                    {
                        case F3DFXType.Vulcan:
                            WeaponController.instance.VulcanImpact(hitPoint.point + hitPoint.normal * fxOffset);
                            ApplyForce(20f);
                            break;

                        case F3DFXType.SoloGun:
                            F3DFXController.instance.SoloGunImpact(hitPoint.point + hitPoint.normal * fxOffset);
                            ApplyForce(25f);
                            break;

                        case F3DFXType.Seeker:
                            F3DFXController.instance.SeekerImpact(hitPoint.point + hitPoint.normal * fxOffset);
                            ApplyForce(30f);
                            break;

                        case F3DFXType.PlasmaGun:
                            F3DFXController.instance.PlasmaGunImpact(hitPoint.point + hitPoint.normal * fxOffset);
                            ApplyForce(25f);
                            break;

                        case F3DFXType.LaserImpulse:
                            F3DFXController.instance.LaserImpulseImpact(hitPoint.point + hitPoint.normal * fxOffset);
                            ApplyForce(25f);
                            break;
                    }

                    isFXSpawned = true;
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

                SphereCastForColliders(step.magnitude, 0.2f, layerMask);

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
        void SphereCastForColliders(float radius, float distance, LayerMask layerMask)
        {
            RaycastHit[] colliders = Physics.SphereCastAll(transform.position, radius, transform.forward, distance, layerMask);
            foreach (RaycastHit collider in colliders)
            {
                isHit = true;
                hitPoint = collider;
                return;
            }
        }
    }
}