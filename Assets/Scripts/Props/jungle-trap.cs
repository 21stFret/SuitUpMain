using UnityEngine;

public class JungleTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    public DamageArea damageArea;
    [SerializeField] private bool canTriggerMultipleTimes = false;
    [SerializeField] private float resetTime = 3f;

    [Header("References")]
    [SerializeField] private Animator trapAnimator;
    [SerializeField] private string triggerAnimationParameter = "Activate";
    [SerializeField] private AudioSource trapSoundEffect;

    public bool isActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.instance != null)
        {
            if (!GameManager.instance.gameActive)
            {
                return; // Exit if the game is not active
            }
        }
        // Check if the trap is active and if the colliding object is the player
        if (isActive)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (BattleMech.instance != null)
                {
                    if (BattleMech.instance.myCharacterController.isDodging)
                    {
                        return;
                    }
                }
            }
            ActivateTrap();
        }
    }

    private void ActivateTrap()
    {
        // Trigger the animation
        if (trapAnimator != null)
        {
            trapAnimator.SetTrigger(triggerAnimationParameter);
        }

        // Play sound effect
        if (trapSoundEffect != null)
        {
            trapSoundEffect.Play();
        }

        if(damageArea != null)
        {
            damageArea.EnableDamageArea();
        }

        isActive = false;


        // Handle trap reset logic
        if (canTriggerMultipleTimes)
        {
            StartCoroutine(ResetTrap());
        }

    }

    private System.Collections.IEnumerator ResetTrap()
    {
        yield return new WaitForSeconds(resetTime);
        isActive = true;
        if(damageArea.targetsInRange.Count>0)
        {
            ActivateTrap();
        }
    }

}
