using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    public GameObject mainObject;
    public TargetHealth targetHealth;

    private void Start()
    {
        image.material.SetColor("_Color", Color.white);
        image.material.SetFloat("_HologramDistortionOffset", 0.2f);
        SetEmmisveHeatlh();
    }

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    public void TakeDamage(float damage)
    {
        if (damage < 0)
        {
            if (targetHealth.health > targetHealth.healthMax)
            {
                // could overheal
                targetHealth.health = targetHealth.healthMax;
            }
            UpdateHealth(targetHealth.health, true);
            return;
        }

        targetHealth.health -= damage;

        UpdateHealth(targetHealth.health);

        // shakes camera
        float damagePercent = Mathf.Clamp(damage / 10, 0.1f, 0.6f);
        impulseSource.GenerateImpulse(damagePercent);

        AudioManager.instance.PlayHurt();
    }

    public void UpdateHealth(float health, bool healed = false)
    {

        if (health <= 0)
        {
            image.fillAmount = 0;
            MechBattleController.instance.OnDie();
            return;
        }


        Color flashcolor = damagefalshColor;
        if(healed)
        {
            flashcolor = healthLightColor;   
        }
        if (updatingUI)
        {
            image.fillAmount = (health / 100);
            return;
        }
        updatingUI = true;

        image.color = Color.Lerp(healthLightColor, damageLightColor, 1 - (health / 100));
        image.DOFillAmount(health / 100, 0.4f).OnComplete(()=>updatingUI = false);
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
