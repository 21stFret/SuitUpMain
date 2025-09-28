using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRocksController : MonoBehaviour
{

    public List<GameObject> rocks;
    public float fallIntervalMax = 2f; // Time between falls
    public float fallIntervalMin = 0.5f;
    private float fallInterval = 2f;
    private float fallTimer = 0f;
    public Transform playerTransform;
    private int rockIndex = 0;
    public float fallDistance = 30f; // Distance above the player where rocks will fall
    public bool falling = true;
    public float targetingRadius = 10f; // Radius within which rocks will target the player


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartFalling()
    {
        falling = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!falling) return;

        fallTimer += Time.deltaTime;
        if (fallTimer >= fallInterval)
        {
            fallTimer = 0f;
            fallInterval = Random.Range(fallIntervalMin, fallIntervalMax);
            if (rocks.Count == 0) return;
            rockIndex = (rockIndex + 1) % rocks.Count;
            Vector3 spawnPosition = playerTransform.position + Vector3.up * fallDistance;

            // Randomize position within targeting radius
            Vector2 randomOffset = Random.insideUnitCircle * targetingRadius;
            spawnPosition += new Vector3(randomOffset.x, 0, randomOffset.y);

            var rock = rocks[rockIndex];
            rock.transform.position = spawnPosition;
            rock.SetActive(true);
            rock.GetComponentInChildren<FallingRock>(true).Fall(spawnPosition);
        }
    }
}
