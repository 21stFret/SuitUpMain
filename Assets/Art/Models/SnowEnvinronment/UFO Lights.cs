using System.Collections;
using UnityEngine;

public class MaterialFlicker : MonoBehaviour
{
    public Material lightsOFF;
    public Material lightsON;
    public float minTime;
    public float maxTime;

    private MeshRenderer meshRenderer;
    private int targetSubMeshIndex = 2;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        StartCoroutine(FlickerMaterial(targetSubMeshIndex));
    }

    private IEnumerator FlickerMaterial(int index)
    {
        while (true)
        {
            Material[] materials = meshRenderer.sharedMaterials;

            if (ReferenceEquals(materials[index], lightsOFF))
            {
                materials[index] = lightsON;
            }
            else if (ReferenceEquals(materials[index], lightsON))
            {
                materials[index] = lightsOFF;
            }
            else
            {
                materials[index] = lightsOFF;
            }

            meshRenderer.sharedMaterials = materials;

            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
        }
    }
}
