using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] private GameObject[] brokenPartPrefabs;
    [SerializeField] private float hideDelay = 5f;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 2f;

    private List<GameObject>[] brokenPartPools;
    private List<GameObject> activeparts = new List<GameObject>();
    private bool isBroken = false;
    public float localScale;


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

        // Activate and apply physics to broken parts
        for (int i = 0; i < brokenPartPrefabs.Length; i++)
        {
            GameObject part = GetPooledpart(i);
            Vector3 randomOffset = Random.insideUnitSphere * 0.5f;
            randomOffset.y = 3f;
            Vector3 newPos = transform.position + randomOffset;
            part.transform.position = transform.position + randomOffset;
            part.transform.rotation = transform.rotation;
            part.SetActive(true);
            activeparts.Add(part);
            part.transform.localScale = new Vector3(localScale, localScale, localScale);
            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                // Add random offset to explosion center
                Vector3 randomOffset1 = Random.insideUnitSphere * 0.5f;
                randomOffset1.y = -0.5f;
                Vector3 explosionCenter = newPos + randomOffset1;

                // Add random torque for spin
                rb.AddTorque(Random.insideUnitSphere * Random.Range(10f, 30f), ForceMode.Impulse);

                // Randomize the upwards modifier
                float randomUpwardsModifier = Random.Range(0.5f, 2f) * (explosionForce / 2);

                // Add explosion force with randomized parameters
                rb.AddExplosionForce(
                    Random.Range(explosionForce * 0.7f, explosionForce * 1.3f),
                    explosionCenter,
                    Random.Range(explosionRadius * 0.8f, explosionRadius * 1.2f),
                    randomUpwardsModifier
                );
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
