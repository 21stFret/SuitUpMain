using UnityEngine;
using System.Collections;

public class CrawlerAggression : MonoBehaviour 
{
    [Range(0f, 1f)]
    public float baseAggression = 0.5f;            // Base aggression level
    public float healthAggressionModifier = 0.3f;  // How much health affects aggression
    public float maxEngagementRange = 20f;         // Maximum range for engagement
    public float minEngagementRange = 5f;          // Minimum range for engagement
    public float fleeHealthThreshold = 0.3f;       // When to consider fleeing
    public float fleeDistance = 15f;               // How far to flee
    public float stealthPreference = 0.7f;         // Above this aggression, prefer direct attacks
    
    private Crawler crawler;
    private CrawlerMovement crawlerMovement;
    private bool isFleeing;
    private Vector3 fleePosition;
    private float currentAggression;

    private void Awake()
    {
        crawler = GetComponent<Crawler>();
        crawlerMovement = GetComponent<CrawlerMovement>();
    }

    // Call this from crawler's CheckDistance method
    public bool HandleAggressionBehavior()
    {
        if (crawler.dead || crawler.target == null)
            return false;

        UpdateAggression();
        
        if (ShouldFlee())
        {
            HandleFleeing();
            return true; // Handled behavior
        }

        // Get engagement range based on current aggression
        float engagementRange = GetEngagementRange();
        
        // If we're a stealth-capable crawler (like Hunter)
        var hunterCrawler = crawler as CrawlerHunter;
        if (hunterCrawler != null)
        {
            return HandleStealthBehavior(hunterCrawler, engagementRange);
        }
        
        // If we're a ranged crawler (like Spitter)
        var spitterCrawler = crawler as CrawlerSpitter;
        if (spitterCrawler != null)
        {
            return HandleRangedBehavior(spitterCrawler, engagementRange);
        }

        // Default melee behavior
        return HandleMeleeBehavior(engagementRange);
    }

    private void UpdateAggression()
    {
        // Modify aggression based on health
        float healthPercentage = crawler.health / crawler.healthMax;
        float healthModifier = (1f - healthPercentage) * healthAggressionModifier;
        
        // Lower health can either make them more desperate (aggressive) or more cautious
        // Here we make them more cautious as health drops
        currentAggression = Mathf.Clamp01(baseAggression - healthModifier);
    }

    private bool ShouldFlee()
    {
        if (isFleeing)
            return true;

        float healthPercentage = crawler.health / crawler.healthMax;
        float fleeThreshold = Mathf.Lerp(fleeHealthThreshold * 2f, fleeHealthThreshold * 0.5f, currentAggression);
        
        return healthPercentage <= fleeThreshold;
    }

    private void HandleFleeing()
    {
        if (!isFleeing)
        {
            StartFleeing();
            return;
        }

        if (Vector3.Distance(transform.position, fleePosition) < 1f)
        {
            isFleeing = false;
            return;
        }

        crawlerMovement.SetDestination(fleePosition);
        
        // Chance to turn and fight based on aggression
        if (crawlerMovement.distanceToTarget < minEngagementRange)
        {
            if (Random.value < currentAggression)
            {
                isFleeing = false;
            }
        }
    }

    private void StartFleeing()
    {
        isFleeing = true;
        Vector3 directionFromTarget = (transform.position - crawler.target.position).normalized;
        fleePosition = transform.position + directionFromTarget * fleeDistance;
        
        // If we're a stealth crawler, try to stealth while fleeing
        var hunterCrawler = crawler as CrawlerHunter;
        if (hunterCrawler != null)
        {
            // Attempt to call EnterStealth through reflection or interface
            hunterCrawler.SendMessage("EnterStealth", SendMessageOptions.DontRequireReceiver);
        }
    }

    private float GetEngagementRange()
    {
        return Mathf.Lerp(minEngagementRange, maxEngagementRange, currentAggression);
    }

    private bool HandleStealthBehavior(CrawlerHunter hunter, float engagementRange)
    {
        // More aggressive hunters are less likely to use stealth
        if (currentAggression < stealthPreference && crawlerMovement.distanceToTarget <= hunter.stealthRange)
        {
            hunter.SendMessage("EnterStealth", SendMessageOptions.DontRequireReceiver);
            return true;
        }
        
        return false;
    }

    private bool HandleRangedBehavior(CrawlerSpitter spitter, float engagementRange)
    {
        // More aggressive spitters will get closer
        float preferredRange = Mathf.Lerp(spitter.escapeDistance * 1.5f, spitter.escapeDistance * 0.8f, currentAggression);
        
        if (crawlerMovement.distanceToTarget < preferredRange)
        {
            crawlerMovement.tracking = false;
            crawlerMovement.SetDestination(transform.position + (transform.position - crawler.target.position).normalized * spitter.escapeDistance);
            return true;
        }
        
        return false;
    }

    private bool HandleMeleeBehavior(float engagementRange)
    {
        // More aggressive crawlers will chase longer distances
        if (crawlerMovement.distanceToTarget <= engagementRange)
        {
            crawlerMovement.tracking = true;
            return false; // Let normal crawler behavior handle the attack
        }
        
        return true; // Stay passive
    }
}
