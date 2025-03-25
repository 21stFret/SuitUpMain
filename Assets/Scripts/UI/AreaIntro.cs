using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaIntro : MonoBehaviour
{
    public int shader;
    public Material material;

    public float fadeInTime = 1.0f;
    private float fadeTime = 0.0f;
    public float fadeMinValue = 5.0f;
    public float fadeMaxValue = -5.0f;
    public bool fadeIn = true;
    private bool _enabled;

    [InspectorButton("ShowAreaIntro")]
    public bool showAreaIntro;

    public void ShowAreaIntro()
    {
        fadeIn = true;
        fadeTime = 0.0f;
        _enabled = true;
    }

    void Start()
    {
        fadeIn = true;
        _enabled = false;
        fadeTime = 0.0f;
        shader = Shader.PropertyToID("_DirectionalGlowFadeFade");
    }

    void Update()
    {
        if (!_enabled)
        {
            return;
        }
        if (fadeIn)
        {
            fadeTime += Time.deltaTime;
            if (fadeInTime <= fadeTime)
            {
                fadeIn = false;
            }
        }
        else{
            fadeTime -= Time.deltaTime;
            if (fadeInTime <= fadeTime)
            {
                enabled = false;
            }
        }
        material.SetFloat(shader, Mathf.Lerp(fadeMinValue, fadeMaxValue, fadeTime/fadeInTime));
    }

}
