using UnityEngine;
using System.Collections;
using DTT.AreaOfEffectRegions;

namespace FORGE3D
{
    public class F3DMissile : MonoBehaviour
    {
        public enum MissileType
        {
            Unguided,
            Guided,
            Predictive
        }

        public MissileType missileType;
        public Transform target;
        public LayerMask layerMask;

        public float detonationDistance;

        public float lifeTime = 5f; // Missile life time
        public float despawnDelay; // Delay despawn in ms
        public float velocity = 300f; // Missile velocity
        public float alignSpeed = 1f;
        public float RaycastAdvance = 2f; // Raycast advance multiplier

        public bool DelayDespawn = false; // Missile despawn flag

        public ParticleSystem[] delayedParticles; // Array of delayed particles
        private ParticleSystem[] particles; // Array of Missile particles

        private new Transform transform; // Cached transform

        private bool isHit = false; // Missile hit flag
        private bool isFXSpawned = false; // Hit FX prefab spawned flag

        private float timer = 0f; // Missile timer
        private Vector3 targetLastPos;
        private Vector3 step;
        private MeshRenderer meshRenderer;
        public F3DMissileLauncher launcher;
        public float upLaunchTimer;
        public float upLaunchTime;
        public float explosionRadius;
        public LayerMask crawlerLayer;
        public float forceStrength;
        public float impactDamage;

        private Vector3 randomOffset;
        public float randomOffsetStrength;

        public Vector3 targetPosition;

        public CircleRegion circleRegion;
        public float startingDistance;
        public DroneType droneType;

        private float fuseDelay = 0.2f;

        private void Awake()
        {
            // Cache transform and get all particle systems attached
            transform = GetComponent<Transform>();
            particles = GetComponentsInChildren<ParticleSystem>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        // OnSpawned called by pool manager 
        public void OnSpawned()
        {
            isHit = false;
            isFXSpawned = false;
            timer = 0f;
            targetLastPos = Vector3.zero;
            step = Vector3.zero;
            meshRenderer.enabled = true;
            transform.forward = Vector3.up;
            upLaunchTimer = 0f;
            fuseDelay = 0.2f;
        }

        // OnDespawned called by pool manager 
        public void OnDespawned()
        {
        }

        // Stop attached particle systems emission and allow them to fade out before despawning
        private void Delay()
        {
            if (particles.Length > 0 && delayedParticles.Length > 0)
            {
                bool delayed;

                for (var i = 0; i < particles.Length; i++)
                {
                    delayed = false;

                    for (var y = 0; y < delayedParticles.Length; y++)
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

        // Despawn routine
        private void OnMissileDestroy()
        {
            F3DPoolManager.Pools["GeneratedPool"].Despawn(transform);
        }

        private void OnHit()
        {
            meshRenderer.enabled = false;
            isHit = true;

            // Invoke delay routine if required
            if (DelayDespawn)
            {
                // Reset missile timer and let particles systems stop emitting and fade out correctly
                timer = 0f;
                Delay();
            }
        }

        private void ApplyForceToCrawlers()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, crawlerLayer);
            foreach (Collider collider in colliders)
            {
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if(rb != null)
                {
                    Vector3 forceDirection = (collider.transform.position - transform.position).normalized;
                    forceDirection.y = 0.5f;
                    rb.AddForce(forceDirection * forceStrength, ForceMode.Impulse);
                    rb.AddTorque(Vector3.forward * 3f, ForceMode.Impulse);
                }
                var targetHealth  = collider.GetComponent<TargetHealth>();
                if(targetHealth != null)
                {
                    Crawler crawler = collider.GetComponent<Crawler>();
                    if (crawler != null)
                    {
                        crawler.DealyedDamage(impactDamage , 1f, WeaponType.AoE);
                        continue;
                    }
                    targetHealth.TakeDamage(impactDamage, WeaponType.AoE);

                }
            }
        }

        private void Update()
        {
            if (fuseDelay > 0f)
            {
                fuseDelay -= Time.deltaTime;
                return;
            }
            // If something was hit
            if (isHit)
            {
                // Execute once
                if (!isFXSpawned)
                {
                    ApplyForceToCrawlers();
                    launcher.SpawnExplosion(circleRegion.transform.position);
                    circleRegion.gameObject.SetActive(false);
                    isFXSpawned = true;
                    if (droneType == DroneType.LittleBoy)
                    {
                        PostProcessController.instance.NukeEffect();
                        return;
                    }
                    ScreenShakeUtility.Instance.ShakeScreen(0.6f);

                }

                if (!DelayDespawn || (DelayDespawn && (timer >= despawnDelay)))
                    OnMissileDestroy();
            }
            // No collision occurred yet
            else
            {

                // Navigate
                if (missileType == MissileType.Guided)
                {
                    if (target != null)
                    {
                        targetPosition = target.position;
                        circleRegion.transform.position = targetPosition;
                    }
                    upLaunchTimer += Time.deltaTime;
                    if (upLaunchTimer > upLaunchTime)
                    {
                        Vector3 direction = targetPosition - transform.position;
                        // Align missile to forward direction
                        transform.rotation = Quaternion.Lerp(transform.rotation,
                        Quaternion.LookRotation(direction), Time.deltaTime * alignSpeed);

                        circleRegion.FillProgress = Vector3.Distance(transform.position, targetPosition) / startingDistance;
                    }
                }

                // Missile step per frame based on velocity and time
                step = transform.forward * Time.deltaTime * velocity;

                if (target != null && Vector3.SqrMagnitude(transform.position - target.position) <= detonationDistance)
                {
                    OnHit();
                }
                if (circleRegion != null && Vector3.Distance(transform.position, targetPosition) <= detonationDistance)
                {
                    OnHit();
                }
                if (Physics.Raycast(transform.position, transform.forward, step.magnitude * RaycastAdvance, layerMask))
                {
                    OnHit();
                }
                // Nothing hit
                else
                {
                    // Despawn missile at the end of life cycle
                    if (timer >= lifeTime)
                    {
                        // Do not detonate
                        //isFXSpawned = true;
                        OnHit();
                    }
                }

                // Advances missile forward
                transform.position += step;
            }

            // Updates missile timer
            timer += Time.deltaTime;
        }
    }
}