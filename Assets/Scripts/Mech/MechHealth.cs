using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MechHealth : MonoBehaviour
{
    [Header("UI References")]
    public Image healthBar;
    public Image damageOverlay;
    public TMP_Text healthText;

    [Header("Visual Settings")]
    [ColorUsage(true, true)]
    public Color healthLightColor;
    [ColorUsage(true, true)]
    public Color damageLightColor;
    public float lerpSpeed = 5f;
    public float damageFlashDuration = 1f;

    [Header("Components")]
    public DoTweenFade screenFlash;
    public Cinemachine.CinemachineImpulseSource impulseSource;
    public GameObject deathEffect;
    public GameObject mainObject;
    public GameObject topHalfObject;
    public TargetHealth targetHealth;
    public AudioClip deathClip;

    private SkinnedMeshRenderer meshRenderer;
    private float currentDisplayHealth;
    private float lastDamageTime;
    private float pendingDamage;
    private Rigidbody rb;
    private MYCharacterController characterController;
    private bool isDead;
    private bool hit;

    [InspectorButton("TakeDamage1")]
    public bool TakeDam;

    [InspectorButton("HealTest")]
    public bool Heal;

    private float cachedFillamount;
    private Image flash;

    private bool healthlow;
    public void Init()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<MYCharacterController>();

        // Initialize health display
        currentDisplayHealth = targetHealth.health;
        UpdateHealthUI(targetHealth.health);

        // Setup damage overlay
        healthBar.material.SetColor("_StrongTintTint", healthLightColor);
        healthBar.fillAmount = 1;
        damageOverlay.fillAmount = 1;
        cachedFillamount = 1;

        flash = screenFlash.GetComponent<Image>();
    }

    private void Update()
    {
        if (isDead) return;

        // Handle damage flash fadeout
        if (Time.time - lastDamageTime >= damageFlashDuration)
        {
            // Start health bar lerp
            StartCoroutine(LerpHealthBar());
        }

        // Achievement check
        if (Time.time > 180f && !hit && GameManager.instance != null && GameManager.instance.gameActive)
        {
            PlayerAchievements.instance?.SetAchievement("DODGE_1");
        }
    }

    private void TakeDamage1()
    {
        targetHealth.TakeDamage(10);
    }

    private void HealTest()
    {
        targetHealth.TakeDamage(-10);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        bool isHeal = damage < 0;

        if (!isHeal && characterController.isDodging)
        {
            return;
        }

        BattleMech.instance.droneController.ChargeDroneOnHit(damage);

        // Update actual health
        targetHealth.health = Mathf.Clamp(targetHealth.health - damage, 0, targetHealth.maxHealth);
        UpdateHealthUI(targetHealth.health);

        // Accumulate pending damage for flash effect
        pendingDamage += damage;

        healthBar.fillAmount = Mathf.Clamp01(cachedFillamount -(pendingDamage / targetHealth.maxHealth));

        if (healthBar.fillAmount <= 0.21f)
        {
            if(!healthlow)
            {
                healthlow = true;
                StartCoroutine(HealthBarFlash());
            }
        }
        else
        {
            healthlow = false;
            healthBar.material.SetColor("_StrongTintTint", healthLightColor);
        }

        // Visual effects


        if (!isHeal)
        {
            flash.color = damageLightColor;
            hit = true;
            lastDamageTime = Time.time;
            AudioManager.instance.PlayHurt();
            rb.velocity = rb.velocity/2;
            float damagePercent = Mathf.Clamp(damage / 10f, 0.1f, 0.6f);
            impulseSource.GenerateImpulse(damagePercent);
        }
        else
        {
            flash.color = healthLightColor;
            damageOverlay.fillAmount = healthBar.fillAmount;
            AudioManager.instance.PlayHeal();
        }

        StartCoroutine(DamageFlash());

        if (targetHealth.health <= 0)
        {
            Die();
        }

    }

    private IEnumerator LerpHealthBar()
    {
        while (Mathf.Abs(currentDisplayHealth - targetHealth.health) > 0.01f)
        {
            currentDisplayHealth = Mathf.Lerp(currentDisplayHealth, targetHealth.health, Time.deltaTime * lerpSpeed);
            damageOverlay.fillAmount = currentDisplayHealth / targetHealth.maxHealth;
            yield return null;
        }

        cachedFillamount = healthBar.fillAmount;
        pendingDamage = 0;
    }

    private IEnumerator HealthBarFlash()
    {
        while (healthlow)
        {
            healthBar.material.SetColor("_StrongTintTint", damageLightColor);
            yield return new WaitForSeconds(0.5f);
            healthBar.material.SetColor("_StrongTintTint", healthLightColor);
            yield return new WaitForSeconds(0.5f);
            yield return null;
        }
    }

    private void Die()
    {
        targetHealth.health = 0;
        isDead = true;
        healthBar.fillAmount = 0;
        damageOverlay.fillAmount = 0;
        deathEffect.SetActive(true);
        AudioManager.instance.PlaySFXFromClip(deathClip);
        mainObject.SetActive(false);
        topHalfObject.SetActive(false);
        targetHealth.alive = false;
        BattleMech.instance.OnDie();

        PlayerSavedData.instance._gameStats.totalDeaths++;
        if (PlayerSavedData.instance._gameStats.totalDeaths == 100)
        {
            PlayerAchievements.instance.SetAchievement("DIE_100");
        }
    }

    public void UpdateHealthUI(float health)
    {
        healthText.text = $"{Mathf.Round(health)}/{targetHealth.maxHealth}";
    }


    private IEnumerator DamageFlash()
    {
        screenFlash.FadeIn();
        yield return new WaitForSeconds(0.2f);
        screenFlash.FadeOut();
    }
}