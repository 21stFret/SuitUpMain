﻿using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Reflection;
using System.Collections.Generic;

namespace FORGE3D
{
    public class F3DMissileLauncher : MonoBehaviour
    {
        public Transform missilePrefab;
        public Transform fatManPrefab;
        public Transform player;
        public Vector3 target;
        public Transform[] socket;
        public Transform explosionPrefab;
        public Transform fatManEffectPrefab;
        public float trackingRaduis = 20f;
        public LayerMask layerMask;
        private F3DMissile.MissileType missileType;
        public List<Transform> targets;
        public DroneControllerUI droneController;
        public bool FatMan;

        public Text missileTypeLabel;

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
                if (target>targets.Count)
                {
                    target = Random.Range(0, targets.Count);
                }
                missile.target = targets[target];
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
                    randomPoint.y = 0;
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