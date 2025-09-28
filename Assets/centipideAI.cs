using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class centipideAI : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    public SimpleOrbit simpleOrbit;
    public bool targetingPlayer = false;
    public bool hunting = false;
    public float huntingTimeMin = 5f;
    public float huntingTimeMax = 10f;
    private float huntingTime;
    private float huntingTimer = 0f;

    private void Start()
    {
        huntingTime = Random.Range(huntingTimeMin, huntingTimeMax);
        targetingPlayer = Random.value > 0.5f; // Randomly start by targeting player or orbiting
    }

    private void Update()
    {
        if (hunting)
        {
            huntingTimer += Time.deltaTime;
            if (huntingTimer >= huntingTime)
            {
                targetingPlayer = !targetingPlayer;
                huntingTimer = 0f;
            }
            if (targetingPlayer)
            {
                if (simpleOrbit != null && simpleOrbit.enabled)
                {
                    simpleOrbit.enabled = false;
                    huntingTime = Random.Range(huntingTimeMin, huntingTimeMax);
                }
                MoveTowardsPlayer();
            }
        }
        else
        {
            if (simpleOrbit != null && !simpleOrbit.enabled)
            {
                simpleOrbit.enabled = true;
                simpleOrbit.ForcePositionInFrontofPlayer();
                huntingTime = Random.Range(huntingTimeMin, huntingTimeMax);
            }
        }

    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, player.position, step);
    }
}
