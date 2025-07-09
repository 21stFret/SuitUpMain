using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLogSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LogManager logManager;
    [Header("Spawn Settings")]
    [SerializeField] private int maxSpawnAttempts = 30;
    [SerializeField] private float minDistanceFromPlayer = 10f;
    [SerializeField] private float maxDistanceFromPlayer = 30f;
    [SerializeField] private float heightOffset = 0.5f;
    [SerializeField] private LayerMask collisionCheckLayers;
    [SerializeField] private Vector3 boxCastSize = new Vector3(1.5f, 1f, 1.5f);
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = false;
    private List<Vector3> debugSpawnPositions = new List<Vector3>();
    [InspectorButton("SpawnRandomDataLog")]
    public bool spawnDataLogButton; // Button to trigger log spawning in the inspector
    
    
    public GameObject SpawnRandomDataLog()
    {
        if (logManager == null || logManager.logPrefab == null)
        {
            Debug.LogError("LogManager or logPrefab is null. Cannot spawn data log.");
            return null;
        }

        // if all logs found return null
        if (logManager.discoveredLogsCount >= logManager.logDatabase.allLogs.Count)
        {
            Debug.LogWarning("Maximum number of data logs already found.");
            return null;
        }

        Vector3 spawnPosition = FindValidSpawnPosition();
        if (spawnPosition == Vector3.zero)
        {
            Debug.LogWarning("Could not find valid position for data log");
            return null;
        }

        GameObject dataLog = logManager.logPrefab;
        dataLog.transform.position = spawnPosition;
        dataLog.transform.rotation = Quaternion.Euler(0, 0, 0);
        logManager.GenerateGOLog();
        dataLog.GetComponentInChildren<InteractableObject>().ShowPrompt(false);

        if (showDebugGizmos)
        {
            debugSpawnPositions.Add(spawnPosition);
        }

        return dataLog;
    }

    public void HideDataLog()
    {
        if (logManager == null || logManager.logPrefab == null)
        {
            Debug.LogError("LogManager or logPrefab is null. Cannot hide data log.");
            return;
        }

        logManager.logPrefab.SetActive(false);
    }
    
    private Vector3 FindValidSpawnPosition()
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            // Generate random direction from player
            Vector2 randomCircle = Random.insideUnitCircle.normalized * 
                Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
            
            Vector3 potentialPosition = Vector3.zero + 
                new Vector3(randomCircle.x, 0, randomCircle.y);
            
            // Check for ground
            if (Physics.Raycast(potentialPosition + Vector3.up * 10, Vector3.down, out RaycastHit groundHit, 20f, collisionCheckLayers))
            {
                // Set position to ground hit point + offset
                potentialPosition = groundHit.point + Vector3.up * heightOffset;
                
                // Check for collisions at that position with a box cast
                if (!Physics.BoxCast(
                    potentialPosition + Vector3.up, 
                    boxCastSize / 2, 
                    Vector3.down, 
                    out RaycastHit hit, 
                    Quaternion.identity, 
                    1f, 
                    collisionCheckLayers))
                {
                    return potentialPosition;
                }
            }
        }
        
        return Vector3.zero; // No valid position found
    }
    
    private void OnDrawGizmos()
    {
        if (showDebugGizmos)
        {
            Gizmos.color = Color.green;
            foreach (Vector3 pos in debugSpawnPositions)
            {
                Gizmos.DrawSphere(pos, 0.5f);
                Gizmos.DrawWireCube(pos + Vector3.up * 0.5f, boxCastSize);
            }
        }
    }
}