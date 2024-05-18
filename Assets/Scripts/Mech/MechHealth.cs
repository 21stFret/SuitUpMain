using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class MechHealth : MonoBehaviour
{
    public Image image;
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
    public TargetHealth targetHealth;
    private float hitTime;
    private bool hit;

    private void Start()
    {
        image.material.SetColor("_Color", Color.white);
        image.material.SetFloat("_HologramDistortionOffset", 0.2f);
        SetEmmisveHeatlh();
    }

    private void Update()
    {
        CheckAcheivement();
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

        UpdateHealth(targetHealth.health, damage<0);

        // shakes camera
        float damagePercent = Mathf.Clamp(damage / 10, 0.1f, 0.6f);
        impulseSource.GenerateImpulse(damagePercent);

        AudioManager.instance.PlayHurt();
    }

    public void UpdateHealth(float health, bool healed = false)
    {

        if (health <= 0)
        {
            DOTween.Kill(image);
            image.fillAmount = 0;
            MechBattleController.instance.OnDie();
            deathEffect.SetActive(true);
            AudioManager.instance.PlaySFXFromClip(deathClip);
            mainObject.SetActive(false);
            PlayerSavedData.instance._gameStats.totalDeaths++;
            if(PlayerSavedData.instance._gameStats.totalDeaths == 100)
            {
                PlayerAchievements.instance.SetAchievement("DIE_100");
            }

            return;
        }
        if(health>targetHealth.healthMax)
        {
            health = targetHealth.healthMax;
        }


        Color flashcolor = damagefalshColor;
        if(healed)
        {
            flashcolor = healthLightColor;   
        }
        if (updatingUI)
        {
            image.fillAmount = (health / targetHealth.healthMax);
            return;
        }
        updatingUI = true;

        image.color = Color.Lerp(healthLightColor, damageLightColor, 1 - (health / targetHealth.healthMax));
        image.fillAmount = Mathf.Lerp(image.fillAmount, health / targetHealth.healthMax, 0.4f);
        image.DOFillAmount(health / targetHealth.healthMax, 0.4f).OnComplete(()=>updatingUI = false);
        image.DOColor(flashcolor, 0.18f).SetLoops(2, LoopType.Yoyo);
        SetEmmisveHeatlh();

        if(healed)
        {
            return;              
        }
        screenFlash.PlayTween();
        SetShader();

    }

    private void SetShader()
    {
        var material = image.material;
        material.DOColor(damagefalshColor, 0.18f).SetLoops(2, LoopType.Yoyo);
        Tween t = DOTween.To(() => material.GetFloat("_HologramDistortionOffset"), x => material.SetFloat("_HologramDistortionOffset", x), 1f, 0.1f).SetLoops(2, LoopType.Yoyo);
        t.Play();
        image.material = material;
    }

    private void SetEmmisveHeatlh()
    {
        var material = meshRenderer.sharedMaterial;
        material.SetColor("_Emmission", Color.Lerp(healthLightColor, damageLightColor, 1 - (image.fillAmount-0.2f)));
        meshRenderer.material = material;
    }

}
