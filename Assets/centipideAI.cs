using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class centipideAI : MonoBehaviour
{
    public Transform player;
    public float speed = 5f;
    public bool targetingPlayer = false;
    public float huntingTimeMin = 5f;
    public float huntingTimeMax = 10f;
    private float huntingTime;
    private float huntingTimer = 0f;

    // Serpentine settings (always active)
    public float zigzagAmplitude = 2f;
    public float zigzagFrequency = 2f;
    private float timeOffset;

    // Wander prediction settings
    public float predictionTime = 1f; // How far ahead to predict player position
    public float directionChangeInterval = 3f;
    private Vector3 targetDirection;
    private float nextDirectionChange;
    private float groundY = 0.2f;
    private bool isBuried = false;
    public float buryChance = 0.1f; // Chance to bury when switching to hunting mode
    public float buryCheckInterval = 5f; // Check every 5 seconds
    private float buryCheckTimer = 0f;
    private bool emerging = false;
    public float buryDepth;
    public float emergeHeight;

    public CentapideHead centapideHead;

    private void Start()
    {
        huntingTime = Random.Range(huntingTimeMin, huntingTimeMax);
        targetingPlayer = Random.value > 0.5f;
        groundY = 0.2f;

        // Initialize
        timeOffset = Random.Range(0f, 100f);
        ChooseNewWanderDirection();
    }

    private void Update()
    {
        AboveGroundMovement();
        BelowGroundMovement();
        BuryCheck();
    }

    private void RandomiseStats()
    {
        zigzagAmplitude = Random.Range(15f, 20f);
        zigzagFrequency = Random.Range(5f, 10f);
        predictionTime = Random.Range(5f, 10f);
        directionChangeInterval = Random.Range(1f, 4f);
        buryChance = Random.Range(0.4f, 1f);
        buryCheckInterval = Random.Range(4f, 6f);
    }

    private void BuryCheck()
    {
        buryCheckTimer += Time.deltaTime;
        if (buryCheckTimer >= buryCheckInterval)
        {
            buryCheckTimer = 0f;
            if(isBuried)
            {
                EmergeFromGround();
            }
            else
            {
                if (Random.value < buryChance)
                {
                    BuryInGround();
                }
            }
        }
    }

    private void AboveGroundMovement()
    {
        if (isBuried)
        {
            return;
        }

        if(emerging)
        {
            if(centapideHead.transform.position.y > groundY)
            {
                emerging = false;
                speed /= 2f;
                centapideHead.speed /= 2f;
            }
            return;
        }
        
        groundY = 0.2f;
        huntingTimer += Time.deltaTime;
        if (huntingTimer >= huntingTime)
        {
            targetingPlayer = !targetingPlayer;
            huntingTimer = 0f;
            huntingTime = Random.Range(huntingTimeMin, huntingTimeMax);
        }

        if (targetingPlayer)
        {
            MoveTowardsPlayer();
        }
        else
        {
            SerpentineWander();
        }
    }
    
    private void BelowGroundMovement()
    {
        if (!isBuried)
        {
            return;
        }
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        // Move directly towards player with serpentine motion
        targetDirection = (player.position - transform.position).normalized;
        ApplySerpentineMovement();
    }

    private void SerpentineWander()
    {
        if (player == null)
        {
            // No player, just wander randomly
            if (Time.time >= nextDirectionChange)
            {
                ChooseNewWanderDirection();
            }
        }
        else
        {
            // Predict where player will be
            if (Time.time >= nextDirectionChange)
            {
                Vector3 predictedPosition = PredictPlayerPosition();

                targetDirection = (predictedPosition - transform.position).normalized;
                nextDirectionChange = Time.time + directionChangeInterval;
            }
        }

        ApplySerpentineMovement();
    }
    
    private Vector3 PredictPlayerPosition()
    {
        if (player == null) return Vector3.zero;

        Vector3 playerDirection = (player.position - transform.position).normalized;
        Vector3 predictedPosition = player.position + playerDirection * predictionTime;
        return predictedPosition;
    }

    private void ApplySerpentineMovement()
    {
        // Calculate perpendicular direction for zigzag
        Vector3 perpendicular = new Vector3(-targetDirection.z, 0, targetDirection.x);
        
        // Apply sine wave for serpentine movement
        float zigzag = Mathf.Sin((Time.time + timeOffset) * zigzagFrequency) * zigzagAmplitude;
        
        // Move forward with zigzag offset
        Vector3 movement = (targetDirection * speed + perpendicular * zigzag) * Time.deltaTime;
        transform.position += movement;
        transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
        
        // Rotate to face movement direction
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 5f);
        }
    }

    private void ChooseNewWanderDirection()
    {
        if (player == null)
        {
            targetDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        }
        else
        {
            Vector3 predictedPosition = PredictPlayerPosition();
            targetDirection = (predictedPosition - transform.position).normalized;
        }

        nextDirectionChange = Time.time + directionChangeInterval;
    }

    private void BuryInGround()
    {
        // Logic to bury in ground
        isBuried = true;
        groundY = buryDepth;
    }

    private void EmergeFromGround()
    {
        // Logic to emerge from ground
        isBuried = false;
        emerging = true;
        groundY = emergeHeight-0.2f;
        transform.position = PredictPlayerPosition() + Random.insideUnitSphere * 5f;
        transform.position = new Vector3(transform.position.x, emergeHeight, transform.position.z);
        speed *= 2f;
        centapideHead.speed *= 2f;    
        RandomiseStats();
    }
}