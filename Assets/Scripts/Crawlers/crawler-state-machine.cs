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
    
    private void Awake()
    {
        crawler = GetComponent<Crawler>();
        movement = GetComponent<CrawlerMovement>();
        
        // Initialize states
        availableStates = new Dictionary<System.Type, CrawlerState>
        {
            { typeof(IdleState), new IdleState(crawler, movement, this) },
            { typeof(PursuitState), new PursuitState(crawler, movement, this) },
            { typeof(AttackState), new AttackState(crawler, movement, this) },
            { typeof(FleeState), new FleeState(crawler, movement, this) },
            { typeof(CautiousState), new CautiousState(crawler, movement, this) },
            { typeof(ChargeState), new ChargeState(crawler, movement, this) }
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
    }

    private void Start()
    {
        // Start in idle state
        TransitionToState(availableStates[typeof(IdleState)]);
    }

    private void Update()
    {
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
        float healthPercentage = crawler.health / crawler.healthMax;
        float healthModifier = (1f - healthPercentage) * healthAggressionModifier;
        CurrentAggression = Mathf.Clamp01(baseAggression - healthModifier);
    }

    public float GetEngagementRange()
    {
        return Mathf.Lerp(preferredEngageDistance, maxEngageDistance, CurrentAggression);
    }

    public void OnDamageTaken()
    {
        currentState?.OnDamaged();
        
        // Check for state transitions based on health
        float healthPercentage = crawler.health / crawler.healthMax;
        
        if (healthPercentage <= fleeHealthThreshold)
        {
            // High aggression crawlers might keep fighting
            if (Random.value > CurrentAggression)
            {
                TransitionToState(typeof(FleeState));
            }
        }
        else if (healthPercentage <= cautionHealthThreshold && CurrentAggression < 0.7f)
        {
            TransitionToState(typeof(CautiousState));
        }
    }
}

// Individual States
public class IdleState : CrawlerState
{
    public IdleState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior) { }
    
    public override void Enter()
    {
        base.Enter();
        movement.canMove = false;
    }
    
    public override void Update()
    {
        if (crawler.target != null)
        {
            behavior.TransitionToState(typeof(PursuitState));
        }
    }
}

public class PursuitState : CrawlerState
{
    public PursuitState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior) { }
    
    public override void Enter()
    {
        base.Enter();
        movement.canMove = true;
        movement.tracking = true;
    }
    
    public override void Update()
    {
        if (crawler.target == null)
        {
            behavior.TransitionToState(typeof(IdleState));
            return;
        }

        float engagementRange = behavior.GetEngagementRange();
        
        if (movement.distanceToTarget <= movement.stoppingDistance)
        {
            behavior.TransitionToState(typeof(AttackState));
        }
        // Consider stealth for hunter type
        else if (crawler is CrawlerHunter && behavior.CurrentAggression < 0.7f)
        {
            CrawlerHunter hunter = crawler as CrawlerHunter;
            if (movement.distanceToTarget <= hunter.stealthRange)
            {
                behavior.TransitionToState(typeof(StealthState));
            }
        }
        // Consider charge for charge type
        else if (crawler is CrawlerCharger)
        {
            CrawlerCharger charger = crawler as CrawlerCharger;
            charger.CheckDistance();
        }
    }
}

public class AttackState : CrawlerState
{
    public AttackState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior) { }
    
    public override void Enter()
    {
        base.Enter();
        crawler.Attack();
    }
    
    public override void Update()
    {

        if(!crawler.triggeredAttack)
        {
            if (movement.distanceToTarget > crawler.attackDistacne)
            {
                crawler.animator.SetBool("InRange", false);
                behavior.TransitionToState(typeof(PursuitState));
                return;
            }
            crawler.Attack();
        }
    }

    public override void Exit()
    {
        base.Exit();
        crawler.triggeredAttack = false;
    }
}

public class ChargeState : CrawlerState
{
    public ChargeState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior)
        : base(crawler, movement, behavior) { }

    public CrawlerCharger charger;

    public override void Enter()
    {
        charger = crawler as CrawlerCharger;
        base.Enter();
        charger.StartCoroutine(charger.Charge());
    }

    public override void Update()
    {
        if(charger.charging)
        {
            charger.Charging();
        }
        else
        {
            behavior.TransitionToState(typeof(PursuitState));
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class FleeState : CrawlerState
{
    private Vector3 fleePosition;
    
    public FleeState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior) { }
    
    public override void Enter()
    {
        base.Enter();
        movement.canMove = true;
        movement.tracking = false;
        CalculateFleePosition();
    }
    
    private void CalculateFleePosition()
    {
        if (crawler.target != null)
        {
            Vector3 directionFromTarget = (crawler.transform.position - crawler.target.transform.position).normalized;
            fleePosition = crawler.transform.position + directionFromTarget * behavior.fleeDistance;
        }
    }
    
    public override void Update()
    {
        movement.SetDestination(fleePosition);
        
        if (Vector3.Distance(crawler.transform.position, fleePosition) < 1f)
        {
            if (behavior.CurrentAggression > 0.7f)
            {
                behavior.TransitionToState(typeof(PursuitState));
            }
            else
            {
                behavior.TransitionToState(typeof(CautiousState));
            }
        }
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
        movement.tracking = false;
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
        if (behavior.CurrentAggression > 0.8f && movement.distanceToTarget < crawler.attackDistacne)
        {
            behavior.TransitionToState(typeof(AttackState));
        }
    }
}

// Specialized States for Different Crawler Types
public class StealthState : CrawlerState
{
    private CrawlerHunter hunterCrawler;
    
    public StealthState(Crawler crawler, CrawlerMovement movement, CrawlerBehavior behavior) 
        : base(crawler, movement, behavior)
    {
        hunterCrawler = crawler as CrawlerHunter;
    }
    
    public override void Enter()
    {
        if (hunterCrawler != null)
        {
            movement.canMove = true;
            hunterCrawler.SendMessage("EnterStealth", SendMessageOptions.DontRequireReceiver);
        }
    }
    
    public override void Update()
    {
        if (movement.distanceToTarget <= hunterCrawler.stealthAttackRange)
        {
            hunterCrawler.SendMessage("StealthAttack", SendMessageOptions.DontRequireReceiver);
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
        spitterCrawler.Attack();
    }
    
    public override void Update()
    {
        if (movement.distanceToTarget < spitterCrawler.escapeDistance)
        {
            behavior.TransitionToState(typeof(FleeState));
        }
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
        Debug.Log("Entering " + GetType().Name);
    }
    public virtual void Exit() { }
    public abstract void Update();
    public virtual void OnDamaged() { }
}
