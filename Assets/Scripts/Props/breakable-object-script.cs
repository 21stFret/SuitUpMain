using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private GameObject[] brokenPartPrefabs;
    [SerializeField] private float hideDelay = 5f;
    [SerializeField] private float explosionForce = 1500f; // Increased force
    [SerializeField] private float explosionRadius = 3f;   // Increased radius
    [SerializeField] private float upwardModifier = 2f;    // Dedicated upward modifier
    [SerializeField] private float torqueMultiplier = 50f; // Control rotation separately

    private List<GameObject>[] brokenPartPools;
    private List<GameObject> activeparts = new List<GameObject>();
    private bool isBroken = false;
    public float localScale;
    public int layer = 21;

    private void Awake()
    {
        InitializeObjectPools();
    }

    private void InitializeObjectPools()
    {
        brokenPartPools = new List<GameObject>[brokenPartPrefabs.Length];
        for (int i = 0; i < brokenPartPrefabs.Length; i++)
        {
            brokenPartPools[i] = new List<GameObject>();
        }
        layer = 21;
    }

    private GameObject GetPooledpart(int partIndex)
    {
        foreach (GameObject part in brokenPartPools[partIndex])
        {
            if (!part.activeInHierarchy)
            {
                return part;
            }
        }

        GameObject newpart = Instantiate(brokenPartPrefabs[partIndex], transform.position, Quaternion.identity);
        newpart.SetActive(false);
        brokenPartPools[partIndex].Add(newpart);
        return newpart;
    }

    public void Break()
    {
        if (isBroken) return;
        isBroken = true;

        // Common explosion center
        Vector3 explosionCenter = transform.position;

        // Activate and apply physics to broken parts
        for (int i = 0; i < brokenPartPrefabs.Length; i++)
        {
            GameObject part = GetPooledpart(i);

            // Reset renderer alpha
            var rend = part.GetComponent<Renderer>();
            if (rend != null)
            {
                if (rend.material.HasProperty("_Color"))
                {
                    Color color = rend.material.color;
                    color.a = 1;
                    rend.material.color = color;
                }
            }

            // Position part initially at object center with slight offset
            Vector3 randomOffset = Random.insideUnitSphere * 0.2f;
            part.transform.position = transform.position + randomOffset;

            // Random rotation for variety
            part.transform.rotation = Random.rotation;
            part.SetActive(true);
            part.layer = layer;
            activeparts.Add(part);
            part.transform.localScale = new Vector3(localScale, localScale, localScale);

            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Reset velocity
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // Ensure mass is reasonable
                rb.mass = Mathf.Clamp(rb.mass, 0.1f, 2f);

                // Adjust drag for more realistic movement
                rb.drag = 0.2f;
                rb.angularDrag = 0.1f;

                // Strong random torque for dramatic spinning
                rb.AddTorque(Random.insideUnitSphere * Random.Range(torqueMultiplier * 0.8f, torqueMultiplier * 1.5f),
                             ForceMode.Impulse);

                // Apply explosion force from slight offset for better dispersion
                Vector3 forceDir = (part.transform.position - explosionCenter).normalized;

                // Add significant explosion force
                rb.AddExplosionForce(
                    Random.Range(explosionForce * 0.9f, explosionForce * 1.5f),
                    explosionCenter - forceDir * 0.5f,  // Offset the center for better dispersion
                    explosionRadius,
                    upwardModifier * Random.Range(0.8f, 1.5f),
                    ForceMode.Impulse
                );

                // Add additional random impulse for more chaotic movement
                rb.AddForce(Random.insideUnitSphere * explosionForce * 0.3f, ForceMode.Impulse);
            }
        }

        // Start the coroutine to hide the parts after a delay
        StartCoroutine(HideBrokenParts());
    }

    private IEnumerator HideBrokenParts()
    {
        yield return new WaitForSeconds(hideDelay);
        foreach (GameObject part in activeparts)
        {
            MeshRenderer renderer = part.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                print("No renderer found on broken part");
                continue;
            }
            Collider collider = part.GetComponent<Collider>();
            if (collider == null)
            {
                print("No collider found on broken part");
                continue;
            }
            part.GetComponent<Rigidbody>().isKinematic = true;
            collider.enabled = false;
            renderer.material.DOFade(0, 1f);
        }
        yield return new WaitForSeconds(1);
        foreach (GameObject part in activeparts)
        {
            part.SetActive(false);
        }
        activeparts.Clear();
        isBroken = false;
    }

    public void Respawn()
    {
        if (!isBroken)
        {
            // If it's not broken, just make sure it's visible
            GetComponent<Renderer>().enabled = true;
            GetComponent<Collider>().enabled = true;
        }
        else
        {
            // If it's broken, hide all active parts immediately
            StopAllCoroutines();
            foreach (GameObject part in activeparts)
            {
                part.SetActive(false);
            }
            activeparts.Clear();

            // Reset the main object
            GetComponent<Renderer>().enabled = true;
            GetComponent<Collider>().enabled = true;
            isBroken = false;
        }
    }
}
