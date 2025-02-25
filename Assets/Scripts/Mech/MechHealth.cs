using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

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
    public bool heal;

    private float cachedFillamount;
    private Image flash;

    private bool healthlow;

    public float shieldHealth;
    public float shieldHealthMax;
    public Image shieldBar;
    public Material shieldMaterial;
    [ColorUsage(true, true)]
    public Color shieldColor;
    private bool isFlashing;
    private Coroutine currentFlashRoutine;

    public void OnDisable()
    {
        // Clean up any running flash
        if (currentFlashRoutine != null)
        {
            StopCoroutine(currentFlashRoutine);
            currentFlashRoutine = null;
        }
        isFlashing = false;
        if (screenFlash != null)
        {
            screenFlash.KillTween();
        }
    }

    public void Init()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        shieldMaterial = meshRenderer.sharedMaterial;
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
        SetShieldBar(100);
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
        if(GameManager.instance != null)
        {
            if(PlayerAchievements.instance != null)
            {
                if(PlayerAchievements.instance.IsAchieved("DODGE_1"))
                {
                    return;
                }
                if (Time.time > 180f && !hit)
                {
                    PlayerAchievements.instance?.SetAchievement("DODGE_1");
                }
            }
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

    public void Heal(float amount)
    {
        targetHealth.TakeDamage(-amount);
    }

    public void TakeDamage(float damage, Crawler crawler = null)
    {
        if (isDead) return;

        bool isHeal = damage < 0;

        if (!isHeal && characterController.isDodging)
        {
            return;
        }

        BattleMech.instance.droneController.ChargeDroneOnHit(damage);


        if(shieldHealth >0 && !isHeal)
        {
            SetShieldBar(damage);
        }
        else
        {
            // Update actual health
            targetHealth.health = Mathf.Clamp(targetHealth.health - damage, 0, targetHealth.maxHealth);
            UpdateHealthUI(targetHealth.health);

            // Accumulate pending damage for flash effect
            pendingDamage += damage;

            healthBar.fillAmount = Mathf.Clamp01(cachedFillamount - (pendingDamage / targetHealth.maxHealth));

            if (healthBar.fillAmount <= 0.21f)
            {
                if (!healthlow)
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
        }

        Color flashColor = isHeal ? healthLightColor : damageLightColor;

        if (!isHeal)
        {
            hit = true;
            lastDamageTime = Time.time;
            AudioManager.instance.PlayHurt();
            rb.velocity = rb.velocity*0.75f;
            float damagePercent = Mathf.Clamp(damage / 10f, 0.1f, 0.6f);
            impulseSource.GenerateImpulse(damagePercent);
            if(GameManager.instance != null)
            {
                RunMod __selectMod = GameManager.instance.runUpgradeManager.HasModByName("Feedback");
                if (__selectMod != null)
                {
                    float percent = (Mathf.Abs(__selectMod.modifiers[0].statValue) / 100);
                    float feedbackDamage = BattleMech.instance.statMultiplierManager.GetCurrentValue(StatType.Tech_Damage) * percent;
                    print("feedback damage is "+feedbackDamage);
                    crawler?.TakeDamage(feedbackDamage, WeaponType.Lightning);
                }
            }

        }
        else
        {
            damageOverlay.fillAmount = healthBar.fillAmount;
            AudioManager.instance.PlayHeal();
        }

        if (!isFlashing)
        {
            currentFlashRoutine = StartCoroutine(DamageFlash(flashColor));
        }
        SetEmmsiveLights();

        if (targetHealth.health <= 0)
        {
            Die();
        }

    }

    private void SetEmmsiveLights()
    {
        float lerpValue = targetHealth.health / targetHealth.maxHealth;
        float emmsiveStrength = Mathf.Lerp(0.5f, 2, lerpValue);
        shieldMaterial.SetFloat("_Emmssive_Strength", emmsiveStrength);
        shieldMaterial.SetColor("_Emmssive_Color", Color.Lerp(healthLightColor, damageLightColor, lerpValue));
    }

    public void SetShieldBar(float dam)
    {
        if (shieldBar == null)
        {
            return;
        }

        shieldHealth = Mathf.Clamp(shieldHealth - dam, 0, shieldHealthMax);
        shieldBar.fillAmount = shieldHealth / shieldHealthMax;

        int health = shieldHealth > 0 ? 1 : 0;
        shieldMaterial.SetColor("_Flash_Color", shieldColor);
        shieldMaterial.SetFloat("_FlashOn", health);
        if(shieldHealth <= 0)
        {
            shieldBar.fillAmount = 0;
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

    public void SetHealthBar(float vlaaue)
    {
        healthBar.fillAmount = vlaaue / targetHealth.maxHealth;
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
        healthText.text = $"{Mathf.Round(health)}/{Mathf.Round(targetHealth.maxHealth)}";
    }


    private IEnumerator DamageFlash(Color color)
    {

        // If there's already a flash in progress, stop it
        if (currentFlashRoutine != null)
        {
            StopCoroutine(currentFlashRoutine);
            screenFlash.KillTween();
        }

        flash.color = color;
        isFlashing = true;
        screenFlash.FadeIn();

        yield return new WaitForSeconds(0.2f);

        screenFlash.FadeOut();

        yield return new WaitForSeconds(0.2f);

        isFlashing = false;
        currentFlashRoutine = null;
    }
}