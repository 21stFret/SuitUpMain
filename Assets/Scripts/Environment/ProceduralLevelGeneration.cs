using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProceduralLevelGeneration : MonoBehaviour
{
    public List<GameObject> parentHolders; // List of items to place
    public List<GameObject> items = new List<GameObject>();
    public List<GameObject> spawnedItems = new List<GameObject>();
    private List<GameObject> spawnedItemsTotal = new List<GameObject>();
    private List<Vector3> scales = new List<Vector3>();
    public int amount; // Amount of items to place
    public float scaleMin, scaleMax; // Min and Max size of items
    public float rotationY; // Y rotation of items
    public float spacing; // Spacing between items
    public LayerMask layerMask; // Layer mask to check for collisions
    public Vector2 areaSize; // Size of the area in which to place items

    private void Start()
    {
        foreach (GameObject parentHolder in parentHolders)
        {
            foreach (Transform child in parentHolder.transform)
            {
                items.Add(child.gameObject);
                scales.Add(child.localScale);
            }
        }
        foreach (GameObject item in spawnedItems)
        {
            for (int i = 0; i < amount; i++)
            {
                var go = Instantiate(item, Vector3.zero, Quaternion.identity);
                spawnedItemsTotal.Add(go);
            }
        }
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        GenerateLevel(items, false);
        GenerateLevel(spawnedItemsTotal, true);
    }

    public void GenerateLevel(List<GameObject> items, bool instaniated)
    {


        for (int i = 0; i < items.Count; i++)
        {
            // Get item from the list
            GameObject item = items[i];
            float scale = Random.Range(scaleMin, scaleMax);
            Vector3 scaleVector = new Vector3(scale, scale, scale);
            if (!instaniated)
            {
                scaleVector.x = scale * scales[i].x;
                scaleVector.y = scale * scales[i].y;
                scaleVector.z = scale * scales[i].z;
            }

            rotationY = Random.Range(0, 360);

            // Set a random position within the area
            Vector3 position = Vector3.zero;
            bool positionFound = false;
            while (!positionFound)
            {
                float x = Random.Range(-areaSize.x / 2, areaSize.x / 2);
                float z = Random.Range(-areaSize.y / 2, areaSize.y / 2);
                position = new Vector3(x, item.transform.position.y, z);

                // Check if the space is already occupied
                if (!Physics.CheckSphere(position, scale / 2 + spacing, layerMask))
                {
                    positionFound = true;
                }
            }

            // Set the position, size, and rotation
            item.transform.position = position;

            item.transform.localScale = scaleVector;
            item.transform.rotation = Quaternion.Euler(item.transform.rotation.eulerAngles.x, rotationY, item.transform.rotation.eulerAngles.z);
        }
    }
}
