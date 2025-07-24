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

public enum MissilePayload
{
    Standard,
    FatMan,
    Mine,
    Napalm
}

namespace FORGE3D
{
    public class F3DMissileLauncher : MonoBehaviour
    {
        public Transform missilePrefab;
        public Transform fatManPrefab;
        public Transform minePrefab;
        public Transform player;
        public Vector3 target;
        public Transform[] socket;
        public GameObject[] hitUI;
        private int hitUICount;
        private int reseveredCount;
        public Transform explosionPrefab;
        public Transform fatManEffectPrefab;
        public Transform napalmPrefab;
        public float trackingRaduis = 20f;
        public float firingRadius = 10f;
        public float missileSpeed = 10f;
        public LayerMask trackingLayerMask;
        public LayerMask groundLayerMask;
        private F3DMissile.MissileType missileType;
        public List<Transform> launcherTargets;
        public List<Transform> reservedTargets;
        public DroneControllerUI droneController;

        public Ordanance ordananceType;

        public Text missileTypeLabel;

        private List<Vector3> targetPositions = new List<Vector3>();

        public Animator animator;
        public GameObject emptyParent;

        public MissilePayload missilePayload;

        // Use this for initialization
        private void Start()
        {
            missileType = F3DMissile.MissileType.Guided;
        }

        // Spawns explosion
        public void SpawnExplosion(Vector3 position)
        {
            Transform t = null;
            if (missilePayload == MissilePayload.FatMan)
            {
                t = F3DPoolManager.Pools["GeneratedPool"]
                .Spawn(fatManEffectPrefab, position, Quaternion.identity, null);
                F3DAudioController.instance.NukeHit(position);
            }
            if (missilePayload == MissilePayload.Standard)
            {
                t = F3DPoolManager.Pools["GeneratedPool"]
                .Spawn(explosionPrefab, position, Quaternion.identity, null);
                F3DAudioController.instance.BombHit(position);
            }
            if (missilePayload == MissilePayload.Napalm)
            {
                t = F3DPoolManager.Pools["GeneratedPool"]
                .Spawn(napalmPrefab, position, Quaternion.identity, null);
                F3DAudioController.instance.NapalmHit(position);
            }
            if (t != null)
            {
                if(t.GetComponent<F3DDespawn>() != null)
                {
                    return;
                }
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

        public void LaunchMines(int amount)
        {
            animator.SetTrigger("Launch");
            for (var i = 0; i < amount; i++)
            {
                var randomSocketId = Random.Range(0, socket.Length);
                var _mine = F3DPoolManager.Pools["GeneratedPool"].Spawn(minePrefab,
                socket[randomSocketId].position, socket[randomSocketId].rotation, emptyParent.transform);
                Vector3 randomUpDirection = Random.onUnitSphere;
                randomUpDirection.y = Mathf.Abs(randomUpDirection.y);
                GameObject mine = _mine.gameObject;
                mine.GetComponent<Rigidbody>().AddForce(randomUpDirection * Random.Range(10, 20), ForceMode.Impulse);
                mine.GetComponent<Landmine>().Init();
            }

        }

        public void LaunchNuke()
        {
            ordananceType = Ordanance.Guided;
            launcherTargets = SetTargetInRadius(player.position, trackingRaduis, trackingLayerMask);
            animator.SetTrigger("Launch");
            LaunchMissile(1);
        }

        public void LaunchMissile(int targetID)
        {
            Transform _missilePrefab;
            switch (missilePayload)
            {
                case MissilePayload.Standard:
                    _missilePrefab = missilePrefab;
                    break;
                case MissilePayload.FatMan:
                    _missilePrefab = fatManPrefab;
                    break;
                case MissilePayload.Mine:
                    _missilePrefab = minePrefab;
                    break;
                case MissilePayload.Napalm:
                    _missilePrefab = missilePrefab;
                    break;
                default:
                    _missilePrefab = missilePrefab;
                    break;
            }
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

                hitUI[hitUICount].transform.position = missile.targetPosition;
                hitUI[hitUICount].transform.position += Vector3.up * 0.5f;
                hitUI[hitUICount].transform.SetParent(null);
                missile.circleRegion = hitUI[hitUICount].GetComponent<CircleRegion>();
                missile.circleRegion.Radius = missile.explosionRadius;
                missile.startingDistance = Vector3.Distance(missile.targetPosition, socket[randomSocketId].position);
                missile.velocity = missileSpeed;
                hitUI[hitUICount].SetActive(true);
                hitUICount++;
                if (hitUICount >= hitUI.Length)
                {
                    hitUICount = 0;
                }
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

            if (ordananceType == Ordanance.Guided)
            {
                Collider[] colliders = Physics.OverlapSphere(center, radius * 3, layerMask);
                if (colliders.Length > 0)
                {
                    foreach (var collider in colliders)
                    {
                        if (missleAmount <= 0) break;
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
                    Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, 20f, groundLayerMask);
                    if (hit.collider != null)
                    {
                        randomPoint.y = hit.point.y;
                        randomPoint.y += 0.5f;
                    }
                    else
                    {
                        randomPoint.y = 1f;
                    }

                    reservedTargets[reseveredCount].transform.position = randomPoint;
                    reservedTargets[reseveredCount].transform.SetParent(null);
                    targets.Add(reservedTargets[reseveredCount]);
                    reseveredCount++;
                    if (reseveredCount >= reservedTargets.Count)
                    {
                        reseveredCount = 0;
                    }
                }
            }
            return targets;
        }

        private Vector3 SpreadInFront(int i)
        {
            int missileCount = droneController.missileAmount;
            float angleStep = 60f / missileCount; // Spread across 60 degree arc
            float minDistance = 10f; // Minimum distance between targets

            // Calculate angle for this target
            float angle = angleStep * i;

            // Create rotation for this angle
            Quaternion rotation = transform.rotation * Quaternion.Euler(0, angle, 0);

            // Get forward direction with random distance
            float distance = firingRadius * (1f + Random.Range(-0.2f, 0.2f)); // Randomize distance slightly
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
                targetPos = target + (rotation * Vector3.forward * (distance + minDistance));
            }

            return targetPos;
        }

        private Vector3 FireInLine(int i)
        {
            // Create a new target position in front of the player
            Vector3 targetPos = transform.position + (transform.forward * (i * firingRadius * 0.8f));
            return targetPos;
        }

        private Vector3 GetRandomPointOnSphereEdge(Vector3 center, float radius, int i)
        {
            int ringamount = 1;
            if (droneController.missileAmount / 6 > 1)
            {
                ringamount = droneController.missileAmount / 6;
            }
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

        public void RefreshMines()
        {
            var mines = emptyParent.GetComponentsInChildren<Landmine>();
            foreach (var mine in mines)
            {
                F3DPoolManager.Pools["GeneratedPool"].Despawn(mine.gameObject.transform);
            }
        }
    }
}