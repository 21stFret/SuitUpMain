using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public Sprite[] areaIntroSprites;
    public Image areaIntroImage;

    [InspectorButton("ShowAreaIntro")]
    public bool showAreaIntro;

    public void ShowAreaIntro()
    {
        areaIntroImage.gameObject.SetActive(true);
        areaIntroImage.sprite = areaIntroSprites[(int)GameManager.instance.currentAreaType];
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
        material.SetFloat(shader, fadeMinValue);
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
            if (fadeTime >= fadeInTime)
            {
                fadeTime = fadeInTime;
                fadeIn = false;
            }
        }
        else
        {
            fadeTime -= Time.deltaTime;
            if (fadeTime <= 0)
            {
                _enabled = false;
                areaIntroImage.gameObject.SetActive(false);
                material.SetFloat(shader, fadeMinValue);
                return;
            }
        }
        material.SetFloat(shader, Mathf.Lerp(fadeMinValue, fadeMaxValue, fadeTime/fadeInTime));
    }

}
