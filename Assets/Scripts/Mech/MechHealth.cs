using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using Micosmo.SensorToolkit.Example;
using TMPro;

public class MechHealth : MonoBehaviour
{
    public Image image;
    public TMP_Text value;
    public bool updatingUI;
    [ColorUsage(true, true)]
    public Color healthLightColor;
    [ColorUsage(true, true)]
    public Color damageLightColor;
    public Color damagefalshColor;
    private SkinnedMeshRenderer meshRenderer;
    public DoTweenFade screenFlash;
    public Cinemachine.CinemachineImpulseSource impulseSource;
    public GameObject deathEffect;
    public AudioClip deathClip;
    public GameObject mainObject;
    public GameObject topHlafObject;
    public TargetHealth targetHealth;
    private float hitTime;
    private bool hit;
    public float regenRate =3f;
    private float requestedHealth;
    private float regenTime;
    private Rigidbody rb;
    private MYCharacterController characterController;

    private void Start()
    {
        image.material.SetColor("_Color", Color.white);
        image.material.SetFloat("_HologramDistortionOffset", 0.2f);
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<MYCharacterController>();
    }

    private void Update()
    {
        UpdateHealthBar();
        CheckAcheivement();
    }

    private void UpdateHealthBar()
    {
        if (requestedHealth == targetHealth.health)
        {
            return;
        }
        regenTime += Time.deltaTime;
        requestedHealth = Mathf.Lerp(requestedHealth, targetHealth.health, regenTime/regenRate);
        image.color = Color.Lerp(healthLightColor, damageLightColor, 1 - (requestedHealth / targetHealth.healthMax));
        image.fillAmount = requestedHealth / targetHealth.healthMax;
        SetEmmisveHeatlh(image.fillAmount);
    }

    private void CheckAcheivement()
    {
        if (GameManager.instance == null)
        {
            return;
        }
        if (GameManager.instance.RoomPortal._active)
        {
            return;
        }
        hitTime += Time.deltaTime;
        if (hitTime > 180f)
        {
            if (!hit)
            {
                PlayerAchievements.instance.SetAchievement("DODGE_1");
            }
        }
    }

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public void TakeDamage(float damage)
    {
        hit = true;
        targetHealth.health -= damage;

        if (targetHealth.health > targetHealth.healthMax)
        {
            targetHealth.health = targetHealth.healthMax;
        }

        UpdateHealth(targetHealth.health, damage<0);

        // shakes camera
        float damagePercent = Mathf.Clamp(damage / 10, 0.1f, 0.6f);
        impulseSource.GenerateImpulse(damagePercent);

        AudioManager.instance.PlayHurt();

        if(characterController.isDodging)
        {
            return;
        }
        rb.velocity = Vector3.zero;
    }

    public void UpdateHealth(float health, bool healed = false)
    {
        regenTime = 0;

        if (health <= 0)
        {
            health = 0;
            value.text = health.ToString() + "/" + targetHealth.healthMax;
            DOTween.Kill(image);
            image.fillAmount = 0;
            MechBattleController.instance.OnDie();
            targetHealth.alive = false;
            deathEffect.SetActive(true);
            AudioManager.instance.PlaySFXFromClip(deathClip);
            mainObject.SetActive(false);
            topHlafObject.SetActive(false);
            PlayerSavedData.instance._gameStats.totalDeaths++;
            if(PlayerSavedData.instance._gameStats.totalDeaths == 100)
            {
                PlayerAchievements.instance.SetAchievement("DIE_100");
            }

            return;
        }
        value.text = health.ToString() + "/" + targetHealth.healthMax;
        if (healed)
        {
            return;              
        }
        StartCoroutine(DamageFlash());

    }

    private IEnumerator DamageFlash()
    {
        screenFlash.FadeIn();
        yield return new WaitForSeconds(0.2f);
        screenFlash.FadeOut();
    }

    public void SetEmmisveHeatlh(float amount)
    {
        var material = meshRenderer.sharedMaterial;
        material.SetColor("_Emmission", Color.Lerp(healthLightColor, damageLightColor, 1 - (amount-0.2f)));
        meshRenderer.material = material;
    }

    public void SetEmmisiveStrength(float value)
    {
        var material = meshRenderer.sharedMaterial;
        DOTween.To(() => material.GetFloat("_Emmssive_Strength"), x => material.SetFloat("_Emmssive_Strength", x), value, 2f);
        meshRenderer.material = material;
    }

}
