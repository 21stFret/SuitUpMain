using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningPatchPooler : MonoBehaviour
{
    public static BurningPatchPooler Instance;

    public GameObject burningPatchPrefab;
    public int poolSize = 10;
    public List<GameObject> burningPatches;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        burningPatches = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject burningPatch = Instantiate(burningPatchPrefab, transform);
            burningPatch.SetActive(false);
            burningPatches.Add(burningPatch);
        }
    }

    public GameObject GetBurningPatch()
    {
        foreach (GameObject burningPatch in burningPatches)
        {
            if (!burningPatch.activeInHierarchy)
            {
                return burningPatch;
            }
        }

        GameObject newBurningPatch = Instantiate(burningPatchPrefab, transform);
        newBurningPatch.SetActive(false);
        burningPatches.Add(newBurningPatch);
        return newBurningPatch;
    }
}
