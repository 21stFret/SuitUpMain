using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Reflection;
using System.Collections.Generic;
using DTT.AreaOfEffectRegions;

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
        public float missileSpeed = 10f;
        public LayerMask layerMask;
        private F3DMissile.MissileType missileType;
        public List<Transform> targets;
        public DroneControllerUI droneController;
        public bool FatMan;

        public Text missileTypeLabel;

        private List<Vector3> targetPositions = new List<Vector3>();

        // Use this for initialization
        private void Start()
        {
            missileType = F3DMissile.MissileType.Guided;
        }

        // Spawns explosion
        public void SpawnExplosion(Vector3 position)
        {
            if(FatMan)
            {
                F3DPoolManager.Pools["GeneratedPool"]
                .Spawn(fatManEffectPrefab, position, Quaternion.identity, null);
                F3DAudioController.instance.RailGunHit(position);
            }
            else
            {
                F3DPoolManager.Pools["GeneratedPool"]
                .Spawn(explosionPrefab, position, Quaternion.identity, null);
                F3DAudioController.instance.RailGunExplosion(position);
            }

        }

        public void LaunchMissiles(int amount)
        {
            targets = SetTargetInRadius(player.position, trackingRaduis, layerMask);
            foreach (var ui in hitUI)
            {
                ui.SetActive(false);
            }
            for (var i = 0; i < amount; i++)
            {
                LaunchMissile(i);
            }
        }

        public void LaunchMissile(int target)
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
                if (target<targets.Count)
                {
                    missile.target = targets[target];
                    missile.targetPosition = targets[target].position;
                }
                else
                {
                    //SpreadInFront();
                    //missile.targetPosition = targetPositions[target];

                    missile.targetPosition = RandomTargetPosition();
                }

                hitUI[target].transform.position = missile.targetPosition;
                hitUI[target].transform.SetParent(null);
                hitUI[target].SetActive(true);
                missile.circleRegion = hitUI[target].GetComponent<CircleRegion>();
                missile.startingDistance = Vector3.Distance(missile.targetPosition, socket[randomSocketId].position);
                missile.velocity =  missileSpeed;
            }
        }

        private Vector3 RandomTargetPosition()
        {
            Vector3 pos = GetRandomPointInSphere(transform.position, trackingRaduis);
            pos.y = 2f;
            return pos;
        }

        private void SpreadInFront()
        {
            targetPositions.Clear(); // Clear previous positions
            // Get number of missiles to spread
            int missileCount = droneController.missileAmount;
            float angleStep = 60f / missileCount; // Spread across 60 degree arc
            float minDistance = 3f; // Minimum distance between targets

            for (int i = 0; i < missileCount; i++)
            {
                // Calculate angle for this target
                float angle = -30f + (angleStep * i); // Start at -30 degrees
                
                // Create rotation for this angle
                Quaternion rotation = transform.rotation * Quaternion.Euler(0, angle, 0);
                
                // Get forward direction with random distance
                float distance = Random.Range(trackingRaduis * 0.5f, trackingRaduis);
                Vector3 targetPos = transform.position + (rotation * Vector3.forward * distance);
                targetPos.y = 2f; // Set consistent height

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
                    targetPos.y = 2f;
                }

                targetPositions.Add(targetPos);

            }
        }

        public List<Transform> SetTargetInRadius(Vector3 center, float radius, LayerMask layerMask)
        {
            List<Transform> targets = new List<Transform>();
            // Find all colliders within the specified radius
            Collider[] colliders = Physics.OverlapSphere(center, radius, layerMask);
            int missleAmount = droneController.missileAmount;
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
            if (missleAmount > 0)
            {
                // If no objects found, set target to a random point in the sphere
                for (int i = 0; i < missleAmount; i++)
                {
                    var newTarget = Instantiate(new GameObject());
                    Vector3 randomPoint = GetRandomPointInSphere(center, radius);
                    randomPoint.y = 1f;
                    newTarget.transform.position = randomPoint;
                    targets.Add(newTarget.transform);
                    Destroy(newTarget, 5f);
                }
            }
            return targets;
        }

        private Vector3 GetRandomPointInSphere(Vector3 center, float radius)
        {
            return center + Random.insideUnitSphere * radius;
        }
    }
}