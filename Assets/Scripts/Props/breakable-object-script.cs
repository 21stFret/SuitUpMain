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
            part.transform.position = transform.position;
            part.transform.rotation = transform.rotation;
            part.SetActive(true);
            activeparts.Add(part);
            part.transform.localScale = new Vector3(localScale, localScale, localScale);

            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.AddExplosionForce(Random.Range(explosionForce * 0.8f, explosionForce * 1.2f), transform.position, explosionRadius, explosionForce / 2);
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
