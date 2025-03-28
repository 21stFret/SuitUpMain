using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TrippyEffect : MonoBehaviour
{
    public Volume volume;
    public bool trippyActive = false;
    public float trippyDuration = 5f;
    public float hueShiftSpeed = 1f;

    private ColorAdjustments colorAdjustments;
    private ChromaticAberration chromaticAberration;
    private MotionBlur motionBlur;

    private void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet<ColorAdjustments>(out var _colorAdjustments);
        colorAdjustments = _colorAdjustments;
        colorAdjustments.hueShift.value = 0;
        volume.profile.TryGet<ChromaticAberration>(out var _chromaticAberration);
        chromaticAberration = _chromaticAberration;
        chromaticAberration.intensity.value = 0;
        volume.profile.TryGet<MotionBlur>(out var _motionBlur);
        motionBlur = _motionBlur;
        motionBlur.intensity.value = 0;
    }

    public void ActivateTrippyEffect()
    {
        if(trippyActive)
        {
            return;
        }
        StartCoroutine(TurnOnTrippyEffect());
    }

    private IEnumerator TurnOnTrippyEffect()
    {
        trippyActive = true;
        float timer = trippyDuration;
        bool pingpong = true;
        motionBlur.intensity.value = 1;
        while (timer > 0)
        {
            if(chromaticAberration.intensity.value< chromaticAberration.intensity.max)
            {
                chromaticAberration.intensity.value += Time.deltaTime;
            }
            if(colorAdjustments.hueShift.value< -170)
            {
                pingpong = true;
            }
            if(colorAdjustments.hueShift.value> 170)
            {
                pingpong = false;
            }
            if(pingpong)
            {
                colorAdjustments.hueShift.value += hueShiftSpeed * Time.deltaTime;
            }
            else
            {
                colorAdjustments.hueShift.value -= hueShiftSpeed * Time.deltaTime;
            }
            timer -= Time.deltaTime;
            yield return null;
        }
        while(chromaticAberration.intensity.value>0)
        {
            chromaticAberration.intensity.value -= Time.deltaTime;
            if(pingpong)
            {
                colorAdjustments.hueShift.value -= hueShiftSpeed * 2 * Time.deltaTime;
                if(colorAdjustments.hueShift.value< 0)
                {
                    colorAdjustments.hueShift.value = 0;
                }
            }
            else
            {
                colorAdjustments.hueShift.value += hueShiftSpeed * 2 * Time.deltaTime;
                if(colorAdjustments.hueShift.value> 0)
                {
                    colorAdjustments.hueShift.value = 0;
                }
            }
            yield return null;
        }
        colorAdjustments.hueShift.value = 0;
        chromaticAberration.intensity.value = 0;
        motionBlur.intensity.value = 0;
        trippyActive = false;
    }


}
