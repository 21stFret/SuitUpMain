﻿using UnityEngine;
using System.Collections;

namespace FORGE3D
{
    [RequireComponent(typeof (LineRenderer))]
    public class F3DLightning : MonoBehaviour
    {
        public LayerMask layerMask;

        public Texture[] BeamFrames; // Animation frame sequence
        public float FrameStep; // Animation time
        public bool RandomizeFrames; // Randomization of animation frames

        public int Points; // How many points should be used to construct the beam

        public float MaxBeamLength; // Maximum beam length
        public float beamScale; // Default beam scale to be kept over distance

        public bool AnimateUV; // UV Animation
        public float UVTime; // UV Animation speed

        public bool Oscillate; // Beam oscillation flag
        public float Amplitude; // Beam amplitude
        public float OscillateTime; // Beam oscillation rate

        public Transform rayImpact; // Impact transform
        public Transform rayMuzzle; // Muzzle flash transform

        LineRenderer lineRenderer; // Line rendered component
        RaycastHit hitPoint; // Raycast structure

        int frameNo; // Frame counter
        int FrameTimerID; // Frame timer reference
        int OscillateTimerID; // Beam oscillation timer reference

        float beamLength; // Current beam length
        float _beamLength; // Current beam length
        float initialBeamOffset; // Initial UV offset

        public LightningRodController lightningRodController;

        public Transform target;

        void Awake()
        {
            // Get line renderer component
            lineRenderer = GetComponent<LineRenderer>();

            // Assign first frame texture
            if (!AnimateUV && BeamFrames.Length > 0)
                lineRenderer.material.mainTexture = BeamFrames[0];

            // Randomize uv offset
            initialBeamOffset = Random.Range(0f, 5f);
        }

        // OnSpawned called by pool manager 
        void OnSpawned()
        {
            // Start animation sequence if beam frames array has more than 2 elements
            if (BeamFrames.Length > 1)
                Animate();

            // Start oscillation sequence
            if (Oscillate && Points > 0)
                OscillateTimerID = F3DTime.time.AddTimer(OscillateTime, OnOscillate);

        }

        // OnDespawned called by pool manager 
        void OnDespawned()
        {
            // Reset frame counter
            frameNo = 0;

            // Clear frame animation timer
            if (FrameTimerID != -1)
            {
                F3DTime.time.RemoveTimer(FrameTimerID);
                FrameTimerID = -1;
            }

            // Clear oscillation timer
            if (OscillateTimerID != -1)
            {
                F3DTime.time.RemoveTimer(OscillateTimerID);
                OscillateTimerID = -1;
            }
        }

        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        // Hit point calculation
        void Raycast()
        {
            // Prepare structure and create ray
            hitPoint = new RaycastHit();
            Ray ray = new Ray(transform.position, transform.forward);
            if(lightningRodController != null)
            {
                ray = new Ray(transform.position + lightningRodController.raycastOffset, transform.forward);
                Debug.DrawRay(transform.position + lightningRodController.raycastOffset, transform.forward * MaxBeamLength, Color.red);
                MaxBeamLength = lightningRodController.range;
            }


            // Calculate default beam proportion multiplier based on default scale and maximum length
            float propMult = MaxBeamLength * (beamScale / 10f);

            if (target != null)
            {
                beamLength = Vector3.Distance(transform.position, target.position);

                lineRenderer.SetPosition(1, new Vector3(0f, 0f, beamLength));
                propMult = beamLength * (beamScale / 10f);
                // Adjust impact effect position
                if (rayImpact)
                    rayImpact.position = target.position - transform.forward + Vector3.up * 0.5f;
            }
            else
            {
                lineRenderer.SetPosition(0, transform.position);
                // Raycast
                if (Physics.Raycast(ray, out hitPoint, MaxBeamLength, layerMask))
                {
                    // Get current beam length
                    beamLength = Vector3.Distance(transform.position, hitPoint.point);

                    // Update line renderer
                    if (!Oscillate)
                    {
                        Vector3 worldPoint = transform.position + transform.forward * beamLength;
                        lineRenderer.SetPosition(1, worldPoint);
                    }

                    // Calculate default beam proportion multiplier based on default scale and current length
                    propMult = beamLength * (beamScale / 10f);

                    // Apply hit force to rigidbody
                    //ApplyForce(0.1f);

                    // Adjust impact effect position
                    if (rayImpact)
                        rayImpact.position = hitPoint.point - transform.forward * 0.5f;
                }
                // Nothing was hit
                else
                {
                    // Set beam to maximum length
                    beamLength = MaxBeamLength;

                    // Update beam length
                    if (!Oscillate)
                    {
                        Vector3 worldPoint = transform.position + transform.forward * beamLength;
                        lineRenderer.SetPosition(1,worldPoint);
                    }


                    // Adjust impact effect position
                    if (rayImpact)
                        rayImpact.position = transform.position + transform.forward * (beamLength - 0.5f);
                }
            }

            if (lightningRodController != null)
            {
                Collider[] colliders = Physics.OverlapSphere(rayImpact.position, lightningRodController.aimAssit, layerMask);
                if(colliders.Length > 0)
                {
                    foreach (var col in colliders)
                    {
                        if (col.gameObject.GetComponent<TargetHealth>() != null)
                        {
                            Vector3 dir = col.transform.position - rayImpact.position;
                            RaycastHit hit;
                            Ray _ray = new Ray(rayImpact.position, dir);
                            if (Physics.Raycast(_ray, out hit, MaxBeamLength, layerMask))
                            {
                                if(hit.collider.gameObject == col.gameObject)
                                {
                                    hitPoint = hit;
                                    lineRenderer.SetPosition(1, hitPoint.point);
                                    rayImpact.position = hitPoint.point;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            lineRenderer.SetPosition(1, rayImpact.position);
                        }
                    }
                }
                beamLength = Vector3.Distance(transform.position, hitPoint.point);
                lightningRodController.LightningHit(hitPoint);
            }

            // Adjust muzzle position
            if (rayMuzzle)
                rayMuzzle.position = transform.position + transform.forward*0.1f;

            // Set beam scaling according to its length
            lineRenderer.material.SetTextureScale("_BaseMap", new Vector2(propMult, 1f));
        }

        // Generate random noise numbers based on amplitude
        float GetRandomNoise()
        {
            return Random.Range(-Amplitude, Amplitude);
        }

        // Advance texture frame
        void OnFrameStep()
        {
            // Randomize frame counter
            if (RandomizeFrames)
                frameNo = Random.Range(0, BeamFrames.Length);

            // Set current texture frame based on frame counter
            lineRenderer.material.mainTexture = BeamFrames[frameNo];
            frameNo++;

            // Reset frame counter
            if (frameNo == BeamFrames.Length)
                frameNo = 0;
        }

        // Oscillate beam
        void OnOscillate()
        {
            _beamLength = beamLength;
            // Calculate number of points based on beam length and default number of points
            int points = (int) ((beamLength/10f)*Points);

            // Update line rendered segments in case number of points less than 2
            if (points < 2)
            {
                //lineRenderer.SetVertexCount(2);
                lineRenderer.positionCount = 2;
                lineRenderer.useWorldSpace = false;
                lineRenderer.SetPosition(0, Vector3.zero);
                //Vector3 dir = transform.forward.normalized*_beamLength;
                //lineRenderer.SetPosition(1, transform.position + dir);
                lineRenderer.SetPosition(1, new Vector3(0, 0, _beamLength));
            }
            // Update line renderer segments
            else
            {
                // Update number of points for line renderer
                lineRenderer.positionCount = points;
                // Set zero point manually
                lineRenderer.SetPosition(0, Vector3.zero);

                // Update each point with random noise based on amplitude
                for (int i = 1; i < points - 1; i++)
                    lineRenderer.SetPosition(i,
                        new Vector3(GetRandomNoise(), GetRandomNoise(), (beamLength/(points - 1))*i));

                // Set last point manually 
                lineRenderer.SetPosition(points - 1, new Vector3(0f, -1f, beamLength));
            }
        }

        // Initialize frame animation
        void Animate()
        {
            // Set current frame
            frameNo = 0;
            lineRenderer.material.mainTexture = BeamFrames[frameNo];

            // Add timer 
            FrameTimerID = F3DTime.time.AddTimer(FrameStep, OnFrameStep);

            frameNo = 1;
        }

        // Apply force to last hit object
        void ApplyForce(float force)
        {
            if (hitPoint.rigidbody != null)
                hitPoint.rigidbody.AddForceAtPosition(transform.forward*force, hitPoint.point, ForceMode.Force);
        }

        void Update()
        {
            // Animate texture UV
            if (AnimateUV)
                lineRenderer.material.SetTextureOffset("_BaseMap", new Vector2(Time.time*UVTime + initialBeamOffset, 0f));

            // Process raycasting 
            Raycast();
        }
    }
}