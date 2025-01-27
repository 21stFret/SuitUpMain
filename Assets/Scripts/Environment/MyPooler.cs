using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPooler : MonoBehaviour
{
    public static MyPooler Instance;

    public GameObject burningPatchPrefab;
    public GameObject fracturePrefab;
    public int poolSize = 10;
    public List<GameObject> burningPatches;
    public List<GameObject> fractureEffects;

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

            GameObject fractureEffect = Instantiate(fracturePrefab, transform);
            fractureEffect.SetActive(false);
            fractureEffects.Add(fractureEffect);
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

    public GameObject GetFractureEffect()
    {
        foreach (GameObject fractureEffect in fractureEffects)
        {
            if (!fractureEffect.activeInHierarchy)
            {
                return fractureEffect;
            }
        }

        GameObject newFractureEffect = Instantiate(fracturePrefab, transform);
        newFractureEffect.SetActive(false);
        fractureEffects.Add(newFractureEffect);
        return newFractureEffect;
    }
}
