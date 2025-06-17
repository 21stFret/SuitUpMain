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
    public ParticleSystem blastParticles;
    private Vector3 _sphereCastPos;
    public LayerMask layerMask;
    public LayerMask groundLayer;

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
        beamActive = true;
        _audioSource.Play();
        blastParticles.Play();
    }



    private void DeactivateBeam()
    {
        beamActive = false;
        beamParticles.Stop();
        blastParticles.Stop();
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

            RaycastHit _hit;
            if (Physics.Raycast(transform.position, Vector3.down, out _hit, Mathf.Infinity, groundLayer))
            {
                blastParticles.transform.position = _hit.point + Vector3.up * 0.1f; // Adjust position to be slightly above the ground
            }

            beamDamageRateT -= Time.deltaTime;
            if (beamDamageRateT <= 0)
            {
                RaycastHit[] hits = Physics.SphereCastAll(_hit.point, blastRadius, Vector3.down, 0.1f, layerMask);
                foreach (RaycastHit hit in hits)
                {
                    TargetHealth target = hit.collider.GetComponent<TargetHealth>();
                    if (target != null)
                    {
                        target.TakeDamage(beamDamage, WeaponType.AoE);
                    }
                    
                    Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddExplosionForce(blastForce, _hit.point, blastRadius, -0.5f, ForceMode.Impulse);
                    }

                }
                beamDamageRateT = beamDamageRate;
            }

        }
    }
}
