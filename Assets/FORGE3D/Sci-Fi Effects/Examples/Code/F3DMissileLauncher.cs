using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Reflection;
using System.Collections.Generic;
using DTT.AreaOfEffectRegions;
using Unity.VisualScripting;

public enum Ordanance
{
    Burst,
    Line,
    Area,
    Random,
    Guided

}

namespace FORGE3D
{
    public class F3DMissileLauncher : MonoBehaviour
    {
        public Transform missilePrefab;
        public Transform fatManPrefab;
        public Transform player;
        public Vector3 target;
        public Transform[] socket;
        public GameObject[] hitUI;
        public Transform explosionPrefab;
        public Transform fatManEffectPrefab;
        public float trackingRaduis = 20f;
        public float firingRadius = 10f;
        public float missileSpeed = 10f;
        public LayerMask trackingLayerMask;
        private F3DMissile.MissileType missileType;
        public List<Transform> launcherTargets;
        public List<Transform> reservedTargets;
        public DroneControllerUI droneController;
        public bool FatMan;
        public Ordanance ordananceType;

        public Text missileTypeLabel;

        private List<Vector3> targetPositions = new List<Vector3>();

        public Animator animator;

        // Use this for initialization
        private void Start()
        {
            missileType = F3DMissile.MissileType.Guided;
        }

        // Spawns explosion
        public void SpawnExplosion(Vector3 position)
        {
            Transform t = null;
            if(FatMan)
            {
                t = F3DPoolManager.Pools["GeneratedPool"]
                .Spawn(fatManEffectPrefab, position, Quaternion.identity, null);
                F3DAudioController.instance.NukeHit(position);
            }
            else
            {
                t = F3DPoolManager.Pools["GeneratedPool"]
                .Spawn(explosionPrefab, position, Quaternion.identity, null);
                F3DAudioController.instance.BombHit(position);
            }
            if (t != null)
            {
                StartCoroutine(DelayEffectDeactivate(t));
            }

        }

        private IEnumerator DelayEffectDeactivate(Transform effect)
        {
            if (effect != null)
            {
                yield return new WaitForSeconds(5f);
                F3DPoolManager.Pools["GeneratedPool"].Despawn(effect);
            }
        }

        public void LaunchMissiles(int amount, Ordanance ordanance)
        {
            ordananceType = ordanance;
            launcherTargets = SetTargetInRadius(player.position, trackingRaduis, trackingLayerMask);
            FatMan = false;
            animator.SetTrigger("Launch");
            foreach (var ui in hitUI)
            {
                ui.SetActive(false);
            }
            for (var i = 0; i < amount; i++)
            {
                LaunchMissile(i);
            }
        }

        public void LaunchNuke()
        {
            ordananceType = Ordanance.Guided;
            launcherTargets = SetTargetInRadius(player.position, trackingRaduis, trackingLayerMask);
            FatMan = true;
            animator.SetTrigger("Launch");
            LaunchMissile(1);
        }

        public void LaunchMissile(int targetID)
        {
            Transform _missilePrefab = FatMan ? fatManPrefab : missilePrefab;
            var randomSocketId = Random.Range(0, socket.Length);
            var tMissile = F3DPoolManager.Pools["GeneratedPool"].Spawn(_missilePrefab,
                socket[randomSocketId].position, socket[randomSocketId].rotation, null);

            if (tMissile != null)
            {
                var missile = tMissile.GetComponent<F3DMissile>();

                missile.launcher = this;
                missile.missileType = missileType;
                missile.droneType = droneController.currentDroneType;
                if (targetID < launcherTargets.Count)
                {
                    missile.target = launcherTargets[targetID];
                    missile.targetPosition = launcherTargets[targetID].position;
                }

                hitUI[targetID].transform.position = missile.targetPosition;
                hitUI[targetID].transform.SetParent(null);
                missile.circleRegion = hitUI[targetID].GetComponent<CircleRegion>();
                missile.circleRegion.Radius = missile.explosionRadius;
                missile.startingDistance = Vector3.Distance(missile.targetPosition, socket[randomSocketId].position);
                missile.velocity = missileSpeed;
                hitUI[targetID].SetActive(true);
            }
        }

        private Vector3 RandomTargetPosition()
        {
            Vector3 pos = GetRandomPointInSphere(transform.position, trackingRaduis);
            pos.y = 1f;
            return pos;
        }

        public List<Transform> SetTargetInRadius(Vector3 center, float radius, LayerMask layerMask)
        {
            List<Transform> targets = new List<Transform>();
            int missleAmount = droneController.missileAmount;

            if(ordananceType == Ordanance.Guided)
            {
                Collider[] colliders = Physics.OverlapSphere(center, radius, layerMask);
                if (colliders.Length > 0)
                {
                    foreach (var collider in colliders)
                    {
                        if(missleAmount <= 0) break;
                        if (collider.CompareTag("Untagged")) continue;
                        targets.Add(collider.transform);
                        missleAmount--;
                    }
                }
            }
            if (missleAmount > 0)
            {
                // If no objects found, set target to a random point in the sphere
                for (int i = 0; i < missleAmount; i++)
                {
                    Vector3 randomPoint = Vector3.zero;
                    switch (ordananceType)
                    {
                        case Ordanance.Burst:
                            float radiusMultiplier = 1f;
                            if (i > 5)
                            {
                                radiusMultiplier = 1.5f;
                            }
                            if (i > 11)
                            {
                                radiusMultiplier = 2f;
                            }
                            randomPoint = GetRandomPointOnSphereEdge(center, firingRadius * radiusMultiplier, i);
                            break;
                        case Ordanance.Line:
                            randomPoint = FireInLine(i);
                            break;
                        case Ordanance.Area:
                            randomPoint = SpreadInFront(i);
                            break;
                        case Ordanance.Random:
                            randomPoint = GetRandomPointInSphere(center, firingRadius);
                            break;
                        case Ordanance.Guided:
                            randomPoint = GetRandomPointInSphere(center, firingRadius);
                            break;
                    }
                    randomPoint.y += 10f;
                    Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, 20f, layerMask);
                    if (hit.collider != null)
                    {
                        randomPoint.y = hit.point.y;
                        randomPoint.y += 0.5f;
                    }
                    else
                    {
                        randomPoint.y = 1f;
                    }

                    reservedTargets[i].transform.position = randomPoint;
                    reservedTargets[i].transform.SetParent(null);
                    targets.Add(reservedTargets[i]);
                }
            }
            return targets;
        }

        private Vector3 SpreadInFront(int i)
        {
            int missileCount = droneController.missileAmount;
            float angleStep = 60f / missileCount; // Spread across 60 degree arc
            float minDistance = 3f; // Minimum distance between targets

            // Calculate angle for this target
            float angle = -30f + (angleStep * i); // Start at -30 degrees
            
            // Create rotation for this angle
            Quaternion rotation = transform.rotation * Quaternion.Euler(0, angle, 0);
            
            // Get forward direction with random distance
            float distance = Random.Range(firingRadius * 0.5f, firingRadius);
            Vector3 targetPos = transform.position + (rotation * Vector3.forward * distance);

            // Check for overlaps with existing positions
            bool validPosition = true;
            foreach (Vector3 existingPos in targetPositions)
            {
                if (Vector3.Distance(targetPos, existingPos) < minDistance)
                {
                    validPosition = false;
                    break;
                }
            }

            // If position is too close to another, try to adjust it
            if (!validPosition)
            {
                // Try moving it further out
                targetPos = transform.position + (rotation * Vector3.forward * (distance + minDistance));
            }

            return targetPos;        
        }

        private Vector3 FireInLine(int i)
        {
            // Create a new target position in front of the player
            Vector3 targetPos = transform.position + (transform.forward * (i * firingRadius * 0.5f));
            return targetPos;
        }

        private Vector3 GetRandomPointOnSphereEdge(Vector3 center, float radius, int i)
        {
            int ringamount = droneController.missileAmount / 6;
            float angleStep = 360f / (droneController.missileAmount / ringamount); // Total angle divided by number of missiles
            float angle = angleStep * i * Mathf.Deg2Rad;

            // if ring amount is 2 add an offset to the angle
            if (i >= 6 && i < 12)
            {
                angle += Mathf.PI / 2; // 90 degrees offset
            }
            
            // Calculate position using equal spacing
            Vector3 evenlySpacedPoint = new Vector3(
                Mathf.Cos(angle) * radius,
                1f, // Consistent height
                Mathf.Sin(angle) * radius
            );
            
            // Add to target positions for tracking
            targetPositions.Add(center + evenlySpacedPoint);
            
            return center + evenlySpacedPoint;
        }

        private Vector3 GetRandomPointInSphere(Vector3 center, float radius)
        {
            return center + Random.insideUnitSphere * radius;
        }
    }
}