using System;
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
        public Transform player;
        public Vector3 target;
        public Transform[] socket;
        public Transform explosionPrefab;
        public float trackingRaduis = 20f;
        public LayerMask layerMask;
        private F3DMissile.MissileType missileType;
        public List<Transform> targets;
        public DroneController droneController;

        public Text missileTypeLabel;

        // Use this for initialization
        private void Start()
        {
            missileType = F3DMissile.MissileType.Guided;
        }

        // Spawns explosion
        public void SpawnExplosion(Vector3 position)
        {
            F3DPoolManager.Pools["GeneratedPool"]
                .Spawn(explosionPrefab, position, Quaternion.identity, null);
            F3DAudioController.instance.RailGunHit(position);
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
            var randomSocketId = Random.Range(0, socket.Length);
            var tMissile = F3DPoolManager.Pools["GeneratedPool"].Spawn(missilePrefab,
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
                    newTarget.transform.position = GetRandomPointInSphere(center, radius);
                    targets.Add(newTarget.transform);
                    Destroy(newTarget, 3f);
                }
            }
            return targets;
        }

        private Vector3 GetRandomPointInSphere(Vector3 center, float radius)
        {
            return center + Random.insideUnitSphere * radius;
        }
        // Processes input for launching missile
        private void ProcessInput()
        {
            /*
            if (Input.GetMouseButtonDown(0))
            {
                var randomSocketId = Random.Range(0, socket.Length);
                var tMissile = F3DPoolManager.Pools["GeneratedPool"].Spawn(missilePrefab,
                    socket[randomSocketId].position, socket[randomSocketId].rotation, null);

                if (tMissile != null)
                {
                    var missile = tMissile.GetComponent<F3DMissile>();

                    missile.launcher = this;
                    missile.missileType = missileType;

                    if (target != null)
                        missile.target = target;
                }
            }
            

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                missileType = F3DMissile.MissileType.Unguided;
                missileTypeLabel.text = "Missile crateType: Unguided";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                missileType = F3DMissile.MissileType.Guided;
                missileTypeLabel.text = "Missile crateType: Guided";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                missileType = F3DMissile.MissileType.Predictive;
                missileTypeLabel.text = "Missile crateType: Predictive";
            }
            */
        }

        // Update is called once per frame
        private void Update()
        {
            ProcessInput();
        }
    }
}