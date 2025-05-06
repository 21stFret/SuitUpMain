using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PortalEffect : MonoBehaviour
{
    public DoTweenScale doTweenScale;
    public DoTweenFade doTweenFade;
    public ParticleSystem _particleSystem;
    public ParticleSystem _particleSystem2;
    public FORGE3D.F3DWarpJumpTunnel[] f3DWarpJumpTunnel;
    public float portalEffectDuration = 1f;
    public bool infinite;
    public bool isActive;
    public Light _light;

    [InspectorButton("StartEffect")]
    public bool startEffect;

    [InspectorButton("StopEffect")]
    public bool stopEffect;

    public TMP_Text chipText;
    public GameObject Chip;
    public GameObject pins;
    public DoTweenFade label;

    public void DealyedEffect(float delay)
    {         
        StartCoroutine(StartEffectWithDelay(delay));
    }

    public void DealyedStopEffect(float delay)
    {
        StartCoroutine(StopEffectWithDelay(delay));
    }

    public IEnumerator StartEffectWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartEffect();
    }

    public IEnumerator StopEffectWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopEffect();
    }

    public void StartEffect()
    {
        isActive = true;
        if(_light != null)
        {
            _light.enabled = true;
        }
        for(int i = 0; i < f3DWarpJumpTunnel.Length; i++)
        {
            f3DWarpJumpTunnel[i].FadeDelay = portalEffectDuration;
            f3DWarpJumpTunnel[i].infinite = infinite;
            f3DWarpJumpTunnel[i].OnSpawned();
        }
        doTweenScale.StartTween();
        _particleSystem.Play();
        _particleSystem2.Play();
        if(!infinite)
        {
            StartCoroutine(DealyedStopEffect());
        }
    }

    public void StartFirstPersonEffect()
    {
        for (int i = 0; i < f3DWarpJumpTunnel.Length; i++)
        {
            f3DWarpJumpTunnel[i].FadeDelay = portalEffectDuration;
            f3DWarpJumpTunnel[i].infinite = infinite;
            f3DWarpJumpTunnel[i].OnSpawned();

        }
        doTweenFade.FadeIn();
        _particleSystem.Play();
        _particleSystem2.Play();
    }

    public void StopFirstPersonEffect()
    {
        for (int i = 0; i < f3DWarpJumpTunnel.Length; i++)
        {
            f3DWarpJumpTunnel[i].ToggleGrow(false);
        }
        doTweenFade.FadeOut();
        _particleSystem.Stop();
        _particleSystem2.Stop();
    }

    public void StopEffect()
    {
        isActive = false;
        if (_light != null)
        {
            _light.enabled = false;
        }
        for (int i = 0; i < f3DWarpJumpTunnel.Length; i++)
        {
            f3DWarpJumpTunnel[i].ToggleGrow(false);
        }
        doTweenScale.ReverseTween();
        _particleSystem.Stop();
        _particleSystem2.Stop();
        HideChipAndText();
    }


    private IEnumerator DealyedStopEffect()
    {
        yield return new WaitForSeconds(portalEffectDuration);
        if(isActive)
        {
            StopEffect();
        }
    }

    public void SetChipandText(Color pickupColor, ModBuildType pickupType)
    {
        if (Chip == null) return;
        if (chipText == null) return;
        Chip.transform.localScale = Vector3.zero;
        Chip.SetActive(false);
        chipText.color = pickupColor;
        string text = pickupType.ToString().ToLower();
        chipText.text = text + " chip";
        pins.GetComponent<Renderer>().material.SetColor("_EmissionColor", pickupColor * 5);
        StartCoroutine(ShowChipAndText());
    }

    private IEnumerator ShowChipAndText()
    {
        yield return new WaitForSeconds(1f);
        Chip.transform.DOScale(1, 1f);
        Chip.SetActive(true);
        label.FadeIn();
    }

    public void HideChipAndText()
    {
        if(Chip == null) return;
        Chip.SetActive(false);
        label.FadeOut();
    }


}
