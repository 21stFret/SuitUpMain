using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalStrike : MonoBehaviour
{
    public bool beamActive;
    public float beamDamage;
    public float beamDuration;
    private float beamDurationT;
    public float beamDamageRate;
    private float beamDamageRateT;
    public float blastForce;
    public float blastRadius;
    public ParticleSystem beamParticles;
    public List<TargetHealth> targets;
    private Vector3 _sphereCastPos;
    public LayerMask layerMask;

    private AudioSource _audioSource;

    private DroneSystem _droneSystem;

    [InspectorButton("Init")]
    public bool init;

    public void Init(DroneSystem system)
    {
        _droneSystem = system;
        _audioSource = GetComponent<AudioSource>();
        beamActive = false;
        beamDurationT = beamDuration;
        beamDamageRateT = beamDamageRate;
        ActivateBeam();
    }

    public void ActivateBeam()
    {
        beamParticles.Play();
        StartCoroutine(DelayDamage());
        _audioSource.Play();
    }

    private IEnumerator DelayDamage()
    {
        yield return new WaitForSeconds(1);
        beamActive = true;
    }

    private void DeactivateBeam()
    {
        beamActive = false;
        beamParticles.Stop();
        _audioSource.Stop();
        _droneSystem.finishedAbility = true;
    }

    private void Update()
    {
        if (beamActive)
        {
            beamDurationT -= Time.deltaTime;
            if (beamDurationT <= 0)
            {
                DeactivateBeam();
                return;
            }

            beamDamageRateT -= Time.deltaTime;
            if (beamDamageRateT <= 0)
            {
                targets.Clear();
                Vector3 pos = transform.position;
                pos.y =0;
                _sphereCastPos = pos;
                RaycastHit[] hits = Physics.SphereCastAll(_sphereCastPos, blastRadius, Vector3.down, 0.1f, layerMask);
                foreach (RaycastHit hit in hits)
                {
                    TargetHealth target = hit.collider.GetComponent<TargetHealth>();
                    if (target != null)
                    {
                        if (!targets.Contains(target))
                        {
                            targets.Add(target);
                        }
                    }
                }
                foreach (TargetHealth target in targets)
                {
                    target.TakeDamage(beamDamage, WeaponType.AoE);
                    Rigidbody rb = target.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddExplosionForce(blastForce, _sphereCastPos, blastRadius,-0.5f,ForceMode.Impulse);
                    }
                }
                beamDamageRateT = beamDamageRate;
            }

        }
    }
}
