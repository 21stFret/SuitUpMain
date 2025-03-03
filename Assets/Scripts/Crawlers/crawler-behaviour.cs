using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Main behavior controller
public class CrawlerBehavior : MonoBehaviour
{
    [Header("Aggression Settings")]
    [Range(0f, 1f)]
    public float baseAggression = 0.5f;
    public float healthAggressionModifier = 0.3f;
    
    [Header("Distance Settings")]
    public float preferredEngageDistance = 10f;
    public float maxEngageDistance = 20f;
    public float fleeDistance = 15f;
    
    [Header("State Thresholds")]
    public float fleeHealthThreshold = 0.3f;
    public float cautionHealthThreshold = 0.5f;


    private Dictionary<System.Type, CrawlerState> availableStates;
    [SerializeField]
    private CrawlerState currentState;
    private Crawler crawler;
    private CrawlerMovement movement;
    
    public float CurrentAggression { get; private set; }

    private bool isDead;

    private void Awake()
    {
        isDead = true;
    }

    public void Init()
    {
        isDead = false;
        crawler = GetComponent<Crawler>();
        movement = GetComponent<CrawlerMovement>();

        availableStates = new Dictionary<System.Type, CrawlerState>
        {
            { typeof(SpawnedState), new SpawnedState(crawler, movement, this) },
            { typeof(IdleState), new IdleState(crawler, movement, this) },
            { typeof(PursuitState), new PursuitState(crawler, movement, this) },
            { typeof(AttackState), new AttackState(crawler, movement, this) },
            { typeof(FleeState), new FleeState(crawler, movement, this) },
            { typeof(CautiousState), new CautiousState(crawler, movement, this) },
            { typeof(ChargeState), new ChargeState(crawler, movement, this) },
        };

        // Add specialized states for different crawler types
        if (crawler is CrawlerHunter)
        {
            availableStates.Add(typeof(StealthState), new StealthState(crawler, movement, this));
        }
        if (crawler is CrawlerSpitter)
        {
            availableStates.Add(typeof(RangedAttackState), new RangedAttackState(crawler, movement, this));
        }
        if (crawler is CrawlerLeaper)
        {
            availableStates.Add(typeof(LeapState), new LeapState(crawler, movement, this));
        }
        if (crawler is CrawlerAlbino)
        {
            availableStates.Add(typeof(AlbinoChargeState), new AlbinoChargeState(crawler, movement, this));
        }
        // Start in idle state
        TransitionToState(availableStates[typeof(SpawnedState)]);
    }

    private void Update()
    {
        if (isDead) return;
        UpdateAggression();
        currentState?.Update();
    }
    
    public void TransitionToState(System.Type newStateType)
    {
        if (availableStates.TryGetValue(newStateType, out CrawlerState newState))
        {
            TransitionToState(newState);
        }
    }
    
    private void TransitionToState(CrawlerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    
    private void UpdateAggression()
    {
        float healthPercentage = crawler._targetHealth.health / crawler._targetHealth.maxHealth;
        float healthModifier = (1f - healthPercentage) * healthAggressionModifier;
        CurrentAggression = Mathf.Clamp01(baseAggression - healthModifier);
    }

    public float GetEngagementRange()
    {
        UpdateAggression();
        return Mathf.Lerp(preferredEngageDistance, maxEngageDistance, CurrentAggression);
    }

    public void OnDamageTaken()
    {
        currentState?.OnDamaged();

        // Check for state transitions based on health
        
        /*
        // Check for state transitions based on health
        float healthPercentage = crawler._targetHealth.health / crawler._targetHealth.maxHealth;
        
        if (healthPercentage <= fleeHealthThreshold)
        {
            // High aggression crawlers might keep fighting
            if (Random.value > CurrentAggression)
            {
                TransitionToState(typeof(FleeState));
            }
            else
            {
                TransitionToState(typeof(PursuitState));
            }
        }
        else if (healthPercentage <= cautionHealthThreshold && CurrentAggression < 0.7f)
        {
            TransitionToState(typeof(CautiousState));
        }
        else
        {
            TransitionToState(typeof(PursuitState));
        }
        */
    }

    public void OnDeath()
    {
        isDead = true;
        currentState = null;
    }
}

// Individual States
public class IdleState : CrawlerState
{
    public IdleState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior) { }

    float IdleTimer = 0;
    float pulseCheckTime = 1f;

    public override void Enter()
    {
        base.Enter();
        IdleTimer = Random.Range(0,2);
        movement.canMove = false;
        movement.tracking = false;
        crawler.animator.SetBool("Idle", true);
        crawler.FindClosestTarget();
        if(crawler.target != null)
        {
            behavior.TransitionToState(typeof(PursuitState));
        }
    }

    private Vector3 FindIdleRandomPos()
    {
        float attempts = 0;
        float maxAttempts = 15;
        Vector3 randomPos;
        while (attempts < maxAttempts)
        {
            randomPos = crawler.spawnLocation + (Random.insideUnitSphere * movement.wanderRadius);
            randomPos.y = crawler.transform.position.y;

            var colliders = Physics.OverlapSphere(randomPos, 1f, movement.SteeringRaycast);
            if (colliders.Length == 0 && Vector3.Distance(crawler.transform.position, randomPos) >= movement.wanderRadius*0.2 && Vector3.Distance(Vector3.zero, randomPos) < 50)
            {
                return randomPos;
            }
            attempts++;
        }
        //Debug.Log("couldn't find a random position, stayed still");
        return crawler.transform.position;
    }
    
    public override void Update()
    {
        if (crawler.target != null)
        {
            //Debug.Log(crawler.name + " found target during Idle Update, transitioning to pursuit");
            behavior.TransitionToState(typeof(PursuitState));
            return;
        }
        
        IdleTimer += Time.deltaTime;
        if (IdleTimer > 2.5f)
        {
            movement.SetDestination(FindIdleRandomPos());
            behavior.TransitionToState(typeof(PursuitState));
            return;
        }

        pulseCheckTime -= Time.deltaTime;
        if (pulseCheckTime <= 0)
        {
            pulseCheckTime = 1f;
            crawler.FindClosestTarget();
        }
    }

    public override void Exit()
    {
        base.Exit();
        crawler.animator.SetBool("Idle", false);
    }
}

public class PursuitState : CrawlerState
{
    public PursuitState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior) { }
    
    public override void Enter()
    {
        base.Enter();
        movement.tracking = true;
        movement.canMove = true;
    }
    
    public override void Update()
    {
        bool otherBehavior = false;

        if (crawler.target != null)
        {
            movement.SetDestination(crawler.target.transform.position);
        }
        else
        {
            otherBehavior = true;
            if (movement.distanceToTarget < movement.stoppingDistance)
            {
                behavior.TransitionToState(typeof(IdleState));
                return;
            }
        }

        if (otherBehavior) { return; }

        switch (crawler)
        {
            case CrawlerSpitter _spitter:
                otherBehavior = true;
                if (movement.distanceToTarget <= behavior.GetEngagementRange())
                {
                    behavior.TransitionToState(typeof(RangedAttackState));
                }
            break;

            case CrawlerHunter hunter:
                if(hunter.CheckCanStealth())
                {
                    //otherBehavior = true;
                    hunter.EnterStealth();
                }
            break;

            case CrawlerCharger charger:
                if(charger.CheckCanCharge())
                {
                    otherBehavior = true;
                    behavior.TransitionToState(typeof(ChargeState));
                }
            break;

            case CrawlerLeaper leaper:
                if(leaper.CheckCanLeap())
                {
                    otherBehavior = true;
                    behavior.TransitionToState(typeof(LeapState));
                }
            break;
        }

        if (otherBehavior) { return; }

        if (movement.distanceToTarget <= behavior.GetEngagementRange())
        {
            if(crawler.target.GetComponent<TargetHealth>() == null)
            {
                crawler.FindClosestTarget();
                behavior.TransitionToState(typeof(IdleState));
                return;
            }
            behavior.TransitionToState(typeof(AttackState));
        }

    }
}

public class AttackState : CrawlerState
{
    private float attackTimer;
    public AttackState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior) { }
    
    public override void Enter()
    {
        base.Enter();
        movement.canMove = false;
        crawler.Attack();
        attackTimer = 0;
    }
    
    public override void Update()
    {
        if (crawler.target == null)
        {
            behavior.TransitionToState(typeof(IdleState));
            return;
        }

        movement.SetDestination(crawler.target.transform.position);

        attackTimer += Time.deltaTime;
        if (attackTimer <= 1.5f)
        {
            //return;
        }

        if (!crawler.triggeredAttack)
        {
            if (movement.distanceToTarget > behavior.GetEngagementRange())
            {
                behavior.TransitionToState(typeof(PursuitState));
                return;
            }
            crawler.Attack();
        }
        else
        {
            behavior.TransitionToState(typeof(PursuitState));
        }
    }

    public override void Exit()
    {
        base.Exit();
        crawler.animator.SetBool("InRange", false);
        //crawler.triggeredAttack = false;
    }
}

public class SpawnedState : CrawlerState
{
    public SpawnedState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior)
        : base(crawler, movement, behavior) { }

    public override void Enter()
    {
        base.Enter();
        movement.enabled = true;
        movement.canMove = true;
    }

    float spawnTimer = 0;

    public override void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= 0.5f)
        {
            if(crawler.target == null)
            {
                behavior.TransitionToState(typeof(IdleState));
            }
            else
            {
                behavior.TransitionToState(typeof(PursuitState));
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class ChargeState : CrawlerState
{
    public ChargeState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior)
        : base(crawler, movement, behavior) { }

    public CrawlerCharger charger;

    private float chargePulseTime = 0.1f;
    private float chargeTargetresetTime = 1f;
    private float chargePulseTimer;

    public override void Enter()
    {
        charger = crawler as CrawlerCharger;
        base.Enter();
        charger.StartCoroutine(charger.Charge());
        chargePulseTimer = chargePulseTime;
        movement.SetRaycastSteering(charger.chargeLookLayerMask);
    }

    public override void Update()
    {
        movement.SetDestination(crawler.target.transform.position); 
        if (charger.charging)
        {
            chargeTargetresetTime -= Time.deltaTime;
            chargePulseTimer += Time.deltaTime;
            if(chargePulseTimer >= chargePulseTime)
            {
                charger.Charging();
                chargePulseTimer = 0;
            }
            if (chargeTargetresetTime <= 0)
            {
                charger.collidersHit.Clear();
            }
        }
        else
        {
            behavior.TransitionToState(typeof(PursuitState));
        }
    }

    public override void Exit()
    {
        base.Exit();
        movement.SetRaycastSteering(charger.normalLookLayerMask);
    }
}

public class FleeState : CrawlerState
{
    private Vector3 fleePosition;
    private bool isCornered;
    private float corneredCheckInterval = 0.5f;
    private float lastCorneredCheck;
    private Vector3 lastPosition;
    private float stuckThreshold = 0.1f; // Minimum distance that should be moved
    private int stuckCounter = 0;
    private int maxStuckChecks = 3; // How many checks before considering truly stuck
    
    public FleeState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior) { }
    
    public override void Enter()
    {
        base.Enter();
        movement.canMove = true;
        movement.tracking = false;
        isCornered = false;
        CalculateFleePosition();
    }

    public override void Update()
    {
        if (Time.time - lastCorneredCheck > corneredCheckInterval)
        {
            CheckIfCornered();
            lastCorneredCheck = Time.time;
        }

        if (isCornered)
        {
            HandleCorneredBehavior();
            return;
        }
        
        movement.SetDestination(fleePosition);

        float distanceToTarget = Vector3.Distance(crawler.transform.position, crawler.target.transform.position);

        // Exit if target is too far away
        if (distanceToTarget > behavior.fleeDistance)
        {
            behavior.TransitionToState(typeof(IdleState));
            return;
        }
        
        if (Vector3.Distance(crawler.transform.position, fleePosition) < 1f || 
            !IsPositionReachable(fleePosition))
        {
            CalculateFleePosition();
        }
    }

    private void CheckIfCornered()
    {
        float distanceMoved = Vector3.Distance(lastPosition, crawler.transform.position);
        
        if (distanceMoved < stuckThreshold)
        {
            stuckCounter++;
            if (stuckCounter >= maxStuckChecks)
            {
                isCornered = true;
                return;
            }
        }
        else
        {
            stuckCounter = 0;
        }

        lastPosition = crawler.transform.position;
    }

    private void HandleCorneredBehavior()
    {
        // If cornered and health is lower than a certain threshold, fight back
        float healthPercentage = crawler._targetHealth.health / crawler._targetHealth.maxHealth;
        if (healthPercentage < 0.5f)
        {
            behavior.TransitionToState(typeof(AttackState));
            return;
        }

        // Otherwise, try to find an escape route
        Vector3 escapeDirection = FindBestEscapeDirection();
        if (escapeDirection != Vector3.zero)
        {
            fleePosition = crawler.transform.position + escapeDirection * behavior.fleeDistance;
            isCornered = false;
        }
    }

    private Vector3 FindBestEscapeDirection()
    {
        float[] testAngles = new float[] { 0, 45, 90, 135, 180, 225, 270, 315 };
        float bestDistance = 0;
        Vector3 bestDirection = Vector3.zero;

        foreach (float angle in testAngles)
        {
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            RaycastHit hit;
            if (!Physics.Raycast(crawler.transform.position, direction, out hit, behavior.fleeDistance, movement.SteeringRaycast))
            {
                Vector3 potentialPosition = crawler.transform.position + direction * behavior.fleeDistance;
                float distanceFromTarget = Vector3.Distance(potentialPosition, crawler.target.transform.position);
                
                if (distanceFromTarget > bestDistance)
                {
                    bestDistance = distanceFromTarget;
                    bestDirection = direction;
                }
            }
        }

        return bestDirection;
    }

    private bool IsPositionReachable(Vector3 position)
    {
        Vector3 direction = (position - crawler.transform.position).normalized;
        float distance = Vector3.Distance(crawler.transform.position, position);
        return !Physics.Raycast(crawler.transform.position, direction, distance, movement.SteeringRaycast);
    }

    private void CalculateFleePosition()
    {
        if (crawler.target != null)
        {
            Vector3 directionFromTarget = (crawler.transform.position - crawler.target.transform.position).normalized;
            Vector3 proposedFleePosition = crawler.transform.position + directionFromTarget * behavior.fleeDistance;

            // Check if the proposed position is valid within map bounds
            Vector3 clampedPosition = ClampPositionToMapBounds(proposedFleePosition);

            // If the clamped position is too close to the target, find alternative flee direction
            if (Vector3.Distance(clampedPosition, crawler.target.transform.position) < behavior.fleeDistance * 0.5f)
            {
                // Try to find an alternative direction by rotating the flee vector
                Vector3 alternativeDirection = FindAlternativeFleeDirection(directionFromTarget);
                fleePosition = ClampPositionToMapBounds(crawler.transform.position + alternativeDirection * behavior.fleeDistance);
            }
            else
            {
                fleePosition = clampedPosition;
            }
        }
    }

    private Vector3 ClampPositionToMapBounds(Vector3 position)
    {
        // Replace these with your actual map bounds
        float minX = -50;
        float maxX = 50;
        float minZ = -50;
        float maxZ = 50;
        float y = position.y; // Preserve the y position

        return new Vector3(
            Mathf.Clamp(position.x, minX, maxX),
            y,
            Mathf.Clamp(position.z, minZ, maxZ)
        );
    }

    private Vector3 FindAlternativeFleeDirection(Vector3 originalDirection)
    {
        // Try different angles to find a valid escape route
        float[] testAngles = new float[] { 45f, -45f, 90f, -90f, 135f, -135f };

        foreach (float angle in testAngles)
        {
            // Rotate the original direction around the Y axis
            Vector3 rotatedDirection = Quaternion.Euler(0, angle, 0) * originalDirection;
            Vector3 testPosition = crawler.transform.position + rotatedDirection * behavior.fleeDistance;

            // Check if this position would be valid
            Vector3 clampedTestPosition = ClampPositionToMapBounds(testPosition);

            if (Vector3.Distance(clampedTestPosition, crawler.target.transform.position) >= behavior.fleeDistance * 0.5f)
            {
                return rotatedDirection;
            }
        }

        // If no good alternative is found, return the original direction
        return originalDirection;
    }
}

public class CautiousState : CrawlerState
{
    private float circlingRadius = 8f;
    private float circlingSpeed = 2f;
    private float circlingTime;
    
    public CautiousState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior) { }
    
    public override void Enter()
    {
        base.Enter();
        movement.canMove = true;
        movement.tracking = true;
        circlingTime = 0f;
    }
    
    public override void Update()
    {
        if (crawler.target == null)
        {
            behavior.TransitionToState(typeof(IdleState));
            return;
        }

        circlingTime += Time.deltaTime * circlingSpeed;
        behavior.baseAggression += Time.deltaTime * 0.1f;
        
        // Circle around the target while maintaining distance
        Vector3 circlePosition = crawler.target.transform.position + 
            new Vector3(Mathf.Cos(circlingTime), 0, Mathf.Sin(circlingTime)) * circlingRadius;
        
        movement.SetDestination(circlePosition);
        
        // Transition based on aggression and distance
        if (behavior.CurrentAggression > 0.8f && movement.distanceToTarget < movement.stoppingDistance)
        {
            behavior.TransitionToState(typeof(AttackState));
        }
    }
}

// Specialized States for Different Crawler Types
public class StealthState : CrawlerState
{
    private CrawlerHunter hunter;
    public StealthState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior)
    {
        hunter = crawler as CrawlerHunter;
    }
    
    public override void Enter()
    {
        if (hunter != null)
        {
            movement.canMove = true;

        }
    }
    
    public override void Update()
    {

        if (movement.distanceToTarget <= behavior.GetEngagementRange())
        {
            //hunter.StealthAttack();
            behavior.TransitionToState(typeof(AttackState));
        }
    }
}

public class RangedAttackState : CrawlerState
{
    private CrawlerSpitter spitterCrawler;
    
    public RangedAttackState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior)
    {
        spitterCrawler = crawler as CrawlerSpitter;
    }
    
    public override void Enter()
    {
        movement.canMove = false;
        crawler.animator.SetBool("InRange", true);
        base.Enter();
    }
    
    public override void Update()
    {
        if (crawler.target == null)
        {
            behavior.TransitionToState(typeof(IdleState));
            return;
        }

        movement.SetDestination(crawler.target.transform.position);


        if (movement.distanceToTarget < behavior.fleeDistance)
        {
            behavior.TransitionToState(typeof(FleeState));
        }
        if (movement.distanceToTarget > behavior.GetEngagementRange())
        {
            behavior.TransitionToState(typeof(PursuitState));
        }

        spitterCrawler.Attack();    
    }

    public override void Exit()
    {
        base.Exit();
        crawler.animator.SetBool("InRange", false);
    }
}

public class LeapState : CrawlerState
{
    private CrawlerLeaper leaper;

    public LeapState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior)
    {
        leaper = crawler as CrawlerLeaper;
    }

    public override void Enter()
    {
        base.Enter();
        movement.canMove = false;
        leaper.StartLeapPreparation();
    }

    public override void Update()
    {
        if (crawler.target == null)
        {
            behavior.TransitionToState(typeof(IdleState));
            return;
        }

        // After leap completes
        if (!leaper.IsLeaping)
        {
            // Return to pursuit if leap is done
            behavior.TransitionToState(typeof(PursuitState));
        }
    }

    public override void Exit()
    {
        base.Exit();
        movement.canMove = true;
    }
}

public class AlbinoChargeState : CrawlerState
{
    private CrawlerAlbino albino;
    private float chargeTimer;
    private bool _isCharging;

    private float damageTickRate = 0.2f;

    public AlbinoChargeState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior)
    {
        albino = crawler as CrawlerAlbino;
    }

    public override void Enter()
    {
        base.Enter();
        StartCharge();
    }

    private void StartCharge()
    {
        movement.canMove = false;
        chargeTimer = 0;         
        // Start charge animation
        crawler.animator.SetBool("Charging", true);
        crawler.animator.speed = 1.5f;
    }

    public void BuildCharge()
    {
        _isCharging = true;
        movement.canRotate = false;
        albino.buildChargeTimer = albino.buildChargeTime;
        albino.chargeEffect.gameObject.SetActive(true);
        albino.chargeEffect.Play();
        Vector3 targetVelocity = crawler.target.GetComponent<Rigidbody>()?.velocity ?? Vector3.zero;
        Vector3 predictedPosition = crawler.target.position + (targetVelocity * 0.5f);
        movement.SetDestination(predictedPosition, true);
    }
    
    public override void Update()
    {
        if(!_isCharging)
        {
            movement.SetDestination(crawler.target.transform.position);
            albino.buildChargeTimer -= Time.deltaTime;
            if (albino.buildChargeTimer <= 0)
            {
                BuildCharge();
            }
            return;
        }

        crawler.animator.SetBool("Charging", false);
        crawler.animator.speed = 1.5f;
        chargeTimer += Time.deltaTime;
        
        // Move forward at charge speed
        movement.ChargeMovement(albino.chargeSpeed);

        // Check for collision with player
        if(damageTickRate <= 0)
        {
            Collider[] hits = Physics.OverlapSphere(crawler.transform.position, albino.chargeRadius);
            foreach (Collider hit in hits)
            {
                var health = hit.GetComponent<TargetHealth>();
                if (health != null)
                {
                    health.TakeDamage(albino.attackDamage * 1.5f, WeaponType.Cralwer);
                }
                if(hit.CompareTag("BigEnvironment"))
                {
                    hit.GetComponent<BreakableEnvironment>().Hit();
                }
            }
            damageTickRate = 0.2f;
        }
        else
        {
            damageTickRate -= Time.deltaTime;
        }

        // End charge after duration
        if (chargeTimer >= albino.chargeDuration)
        {
            EndCharge();
        }
    }

    private void EndCharge()
    {
        _isCharging = false;
        movement.canMove = true;
        movement.canRotate = true;
        crawler.animator.speed = 1f;
        crawler.meshRenderer.material.SetColor("_Emmission", albino.originalColor);
        crawler.animator.SetBool("Charging", false);
        albino.chargeEffect.Stop();
        albino.charged = false;
        behavior.TransitionToState(typeof(PursuitState));

    }

    public override void Exit()
    {
        base.Exit();
        _isCharging = false;
        crawler.animator.speed = 1f;
        crawler.meshRenderer.material.SetColor("_Emmission", albino.originalColor);
        crawler.animator.SetBool("Charging", false);
        albino.chargeEffect.Stop();
        movement.canMove = true;
        movement.canRotate = true;
        albino.charged = false;
    }
}

// Base state class for all crawler states
[System.Serializable]
public abstract class CrawlerState
{
    protected Crawler crawler;
    protected CrawlerMovement movement;
    protected CrawlerBehavior behavior;

    public CrawlerState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior)
    {
        this.crawler = crawler;
        this.movement = movement;
        this.behavior = behavior;
    }

    public virtual void Enter() 
    {
        //Debug.Log(crawler.name +" entering " + GetType().Name);
    }
    public virtual void Exit() 
    { 
        //Debug.Log(crawler.name + " exiting " + GetType().Name);
    }
    public abstract void Update();
    public virtual void OnDamaged() { }
}
