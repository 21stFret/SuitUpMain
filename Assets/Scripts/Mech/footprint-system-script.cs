using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FootprintSystem : MonoBehaviour
{
    [SerializeField] private GameObject footprintPrefab;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private float footprintLifetime = 5f;
    [SerializeField] private float footprintInterval = 0.5f;

    private List<GameObject> footprintPool;
    private int currentFootprintIndex;
    private float lastFootprintTime;
    private float lastFootprintTime2;
    public bool IsMoving;
    public Transform leftFoot;
    public Transform rightFoot;
    public Transform player;
    public bool footStepLeft;
    private bool blockLeft;
    private bool blockRight;

    private void Start()
    {
        InitializeFootprintPool();
    }

    private void Update()
    {
        /*
        if (Time.time - lastFootprintTime >= footprintInterval && IsMoving)
        {
            PlaceFootprint();
            lastFootprintTime = Time.time;
        }
        */
    }

    private void InitializeFootprintPool()
    {
        transform.parent = null;
        footprintPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject footprint = Instantiate(footprintPrefab, transform);
            footprint.SetActive(false);
            footprintPool.Add(footprint);
        }
    }

    private void PlaceFootprint()
    {
        GameObject footprint = footprintPool[currentFootprintIndex];
        if (footStepLeft)
        {
            footprint.transform.position = leftFoot.position;
            footprint.transform.rotation = player.rotation;
        }
        else
        {
            footprint.transform.position = rightFoot.position;
            footprint.transform.rotation = player.rotation;
        }
        footStepLeft = !footStepLeft;
        footprint.SetActive(true);

        StartCoroutine(DeactivateFootprintAfterDelay(footprint));

        currentFootprintIndex = (currentFootprintIndex + 1) % poolSize;
    }

    public void PlaceLeftFoot()
    {
        if(!gameObject.activeInHierarchy)
        {
            return;
        }
        if (blockLeft)
        {
            return;
        }
        GameObject footprint = footprintPool[currentFootprintIndex];
        footprint.transform.position = leftFoot.position;
        footprint.transform.rotation = player.rotation;
        footprint.SetActive(true);

        StartCoroutine(DeactivateFootprintInputLeft());
        StartCoroutine(DeactivateFootprintAfterDelay(footprint));

        currentFootprintIndex = (currentFootprintIndex + 1) % poolSize;
    }

    public void PlaceRightFoot()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        if (blockRight)
        {
            return;
        }
        GameObject footprint = footprintPool[currentFootprintIndex];
        footprint.transform.position = rightFoot.position;
        footprint.transform.rotation = player.rotation;
        footprint.SetActive(true);

        StartCoroutine(DeactivateFootprintInputRight());
        StartCoroutine(DeactivateFootprintAfterDelay(footprint));

        currentFootprintIndex = (currentFootprintIndex + 1) % poolSize;
    }

    private IEnumerator DeactivateFootprintAfterDelay(GameObject footprint)
    {
        yield return new WaitForSeconds(footprintLifetime);
        footprint.SetActive(false);
    }

    private IEnumerator DeactivateFootprintInputLeft()
    {
        blockLeft = true;
        yield return new WaitForSeconds(footprintInterval);
        blockLeft = false;
    }

    private IEnumerator DeactivateFootprintInputRight()
    { 
        blockRight = true;
        yield return new WaitForSeconds(footprintInterval);
        blockRight = false;
    }
}
