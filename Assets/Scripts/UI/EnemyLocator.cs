using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyLocator : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float stillnessThreshold = 0.1f;
    [SerializeField] private float stillnessRequiredTime = 1.5f;
    [SerializeField] private float detectionRadius = 50f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Visual Settings")]
    [SerializeField] private Transform locatorArrow;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 2f;
    [SerializeField] private Color activeColor = Color.red;

    private Transform player;
    private Vector3 lastPlayerPosition;
    private float stillnessTimer;
    private bool isLocating;
    private Image arrowRenderer;
    private Tween fadeTween;
    private bool shouldTrack;  // New field to control tracking separately from fade

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastPlayerPosition = player.position;
        arrowRenderer = locatorArrow.GetComponentInChildren<Image>();
        
        // Set initial state
        Color startColor = activeColor;
        startColor.a = 0;
        arrowRenderer.color = startColor;
        locatorArrow.gameObject.SetActive(true);
    }

    private void Update()
    {
        // Check if player is still
        float movement = Vector3.Distance(player.position, lastPlayerPosition);
        
        if (movement < stillnessThreshold)
        {
            stillnessTimer += Time.deltaTime;
            if (stillnessTimer >= stillnessRequiredTime && !shouldTrack)
            {
                EnableLocator();
            }
        }
        else
        {
            stillnessTimer = 0;
            if (shouldTrack) // Only call DisableLocator if we're currently tracking
            {
                DisableLocator();
            }
        }

        // Update rotation only when tracking
        if (shouldTrack)
        {
            UpdateLocator();
        }

        lastPlayerPosition = player.position;
    }

    private void EnableLocator()
    {
        shouldTrack = true;
        isLocating = true;
        
        // Kill any existing fade
        fadeTween?.Kill();
        
        // Fade in
        fadeTween = arrowRenderer.DOColor(activeColor, fadeInDuration)
            .SetEase(Ease.OutCubic);
    }

    private void DisableLocator()
    {
        if (!isLocating) return; // Add early return if already disabled
        
        isLocating = false;
        
        // Kill any existing fade
        fadeTween?.Kill();
        
        // Fade out
        Color targetColor = activeColor;
        targetColor.a = 0;
        fadeTween = arrowRenderer.DOColor(targetColor, fadeOutDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() => {
                shouldTrack = false;  // Stop tracking after fade completes
                locatorArrow.gameObject.SetActive(false);
            });
    }

    private void UpdateLocator()
    {
        GameObject nearestTarget = FindNearestEnemy();
        if (nearestTarget != null)
        {
            locatorArrow.gameObject.SetActive(true);
            Vector3 directionToEnemy = nearestTarget.transform.position - player.position;
            directionToEnemy.y = 0;
            
            // Use smooth rotation while tracking
            Quaternion targetRotation = Quaternion.LookRotation(directionToEnemy);
            locatorArrow.rotation = Quaternion.Slerp(
                locatorArrow.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            DisableLocator();
        }
    }

    private void OnDestroy()
    {
        // Clean up DOTween
        fadeTween?.Kill();
    }

    private GameObject FindNearestEnemy()
    {
        GameObject nearestTarget = null;
        float nearestDistance = detectionRadius;

        Collider[] hitColliders = Physics.OverlapSphere(player.position, detectionRadius, enemyLayer);
        
        foreach (var hitCollider in hitColliders)
        {
            Crawler crawler = hitCollider.GetComponent<Crawler>();
            if (crawler != null)
            {
                float distance = Vector3.Distance(player.position, crawler.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTarget = crawler.gameObject;
                }
            }
            else if(hitCollider.GetComponent<Pickup>() != null)
            {
                nearestTarget = hitCollider.gameObject;
            }
        }

        return nearestTarget;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}