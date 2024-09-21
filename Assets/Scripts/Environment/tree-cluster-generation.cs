using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeClusterGeneration : MonoBehaviour
{
    public List<GameObject> treePrefabs;
    public GameObject grassPrefab;
    public int totalTrees = 50;
    public int treesPerCluster = 10;
    public float clusterRadius = 10f;
    public float minTreeScale = 0.8f, maxTreeScale = 1.2f;
    public float centerTreeScaleMultiplier = 1.5f;
    public float treeSpacing = 1f;
    public float grassSpacing = 0.5f;
    public float grassScale = 0.5f;
    public int grassPerTree = 5;
    public Vector2 areaSize = new Vector2(100f, 100f);
    public LayerMask obstacleLayer;

    [InspectorButton("GenerateTreeClusters")]
    public bool generateTreeClusters;

    private int gridSizeX, gridSizeY;
    private float cellSize;

    private List<GameObject> pooledTrees = new List<GameObject>();
    private List<GameObject> pooledGrass = new List<GameObject>();

    private void Start()
    {
        CreateObjectPools();
    }

    private void CreateObjectPools()
    {
        // Create tree pool
        for (int i = 0; i < totalTrees; i++)
        {
            GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Count)];
            GameObject tree = Instantiate(treePrefab, Vector3.zero, Quaternion.identity, transform);
            tree.SetActive(false);
            pooledTrees.Add(tree);
        }

        // Create grass pool
        int totalGrass = totalTrees * grassPerTree;
        for (int i = 0; i < totalGrass; i++)
        {
            GameObject grass = Instantiate(grassPrefab, Vector3.zero, Quaternion.identity, transform);
            grass.SetActive(false);
            pooledGrass.Add(grass);
        }
    }

    [ContextMenu("Generate Tree Clusters")]
    public void GenerateTreeClusters()
    {
        CalculateGridSize();
        PositionVegetation();
    }

    private void CalculateGridSize()
    {
        cellSize = clusterRadius * 2;
        gridSizeX = Mathf.CeilToInt(areaSize.x / cellSize);
        gridSizeY = Mathf.CeilToInt(areaSize.y / cellSize);
    }

    private void PositionVegetation()
    {
        List<Vector2Int> availableCells = new List<Vector2Int>();
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                availableCells.Add(new Vector2Int(x, y));
            }
        }

        int treeIndex = 0;
        int grassIndex = 0;

        while (treeIndex < totalTrees && availableCells.Count > 0)
        {
            int index = Random.Range(0, availableCells.Count);
            Vector2Int cell = availableCells[index];
            availableCells.RemoveAt(index);

            Vector3 clusterCenter = GetPositionFromCell(cell);
            int treesInThisCluster = Mathf.Min(totalTrees - treeIndex, treesPerCluster);
            PositionCluster(clusterCenter, treesInThisCluster, ref treeIndex, ref grassIndex);
        }

        // Deactivate any unused objects
        for (int i = treeIndex; i < pooledTrees.Count; i++)
        {
            pooledTrees[i].SetActive(false);
        }
        for (int i = grassIndex; i < pooledGrass.Count; i++)
        {
            pooledGrass[i].SetActive(false);
        }
    }

    private Vector3 GetPositionFromCell(Vector2Int cell)
    {
        float x = (cell.x + 0.5f) * cellSize - areaSize.x / 2;
        float z = (cell.y + 0.5f) * cellSize - areaSize.y / 2;
        return new Vector3(x + Random.Range(-cellSize / 4, cellSize / 4), transform.position.y, z + Random.Range(-cellSize / 4, cellSize / 4));
    }

    private void PositionCluster(Vector3 center, int treeCount, ref int treeIndex, ref int grassIndex)
    {
        PositionTree(center, maxTreeScale * centerTreeScaleMultiplier, ref treeIndex, ref grassIndex);

        for (int i = 1; i < treeCount && treeIndex < totalTrees; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(treeSpacing, clusterRadius);
            Vector3 position = center + Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

            if (!Physics.CheckSphere(position, treeSpacing / 2, obstacleLayer))
            {
                float scale = Mathf.Lerp(maxTreeScale, minTreeScale, distance / clusterRadius);
                PositionTree(position, scale, ref treeIndex, ref grassIndex);
            }
        }
    }

    private void PositionTree(Vector3 position, float scale, ref int treeIndex, ref int grassIndex)
    {
        if (treeIndex < pooledTrees.Count)
        {
            GameObject tree = pooledTrees[treeIndex];
            tree.SetActive(true);
            tree.transform.position = position;
            tree.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            tree.transform.localScale = Vector3.one * scale;
            treeIndex++;

            PositionGrassAroundTree(position, ref grassIndex);
        }
    }

    private void PositionGrassAroundTree(Vector3 treePosition, ref int grassIndex)
    {
        for (int i = 0; i < grassPerTree && grassIndex < pooledGrass.Count; i++)
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(grassSpacing, treeSpacing * 2);
            Vector3 position = treePosition + Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

            if (!Physics.CheckSphere(position, grassSpacing / 2, obstacleLayer))
            {
                GameObject grass = pooledGrass[grassIndex];
                grass.SetActive(true);
                grass.transform.position = position;
                grass.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                grass.transform.localScale = Vector3.one * grassScale;
                grassIndex++;
            }
        }
    }
}