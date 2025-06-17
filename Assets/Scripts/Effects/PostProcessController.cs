
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PostProcessController : MonoBehaviour
{
    public static PostProcessController instance;
    public Volume globalVolume;
    public Vignette vignette;

    [Header("Nuke Effect")]
    public float nukeDuration = 5f;
    public float nukeIntensity = 1f;
    public CanvasGroup nukeCanvasGroup;
    public bool isNukeActive = false;

    [Header("Flash Vignette")]
    public float flashDuration = 0.5f;
    public bool flashOn = false;
    public float flashIntensity = 0.5f;
    private float flashTimer = 0f;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        globalVolume = GetComponent<Volume>();
        VolumeProfile volumeProfile = globalVolume?.profile;
        volumeProfile.TryGet<Vignette>(out vignette);
    }

    public void NukeEffect()
    {
        isNukeActive = true;
        nukeCanvasGroup.alpha = 1f;
        vignette.intensity.Override(0.5f);
        StartCoroutine(NukeCoroutine());
        ScreenShakeUtility.Instance.ShakeScreen(1.4f);
    }

    private IEnumerator NukeCoroutine()
    {
        float elapsedTime = 0f;
        float startIntensity = nukeIntensity;

        while (elapsedTime < nukeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / nukeDuration;
            nukeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            vignette.intensity.Override(Mathf.Lerp(startIntensity, 0f, t));
            yield return null;
        }

        nukeCanvasGroup.alpha = 0f;
        isNukeActive = false;
    }

    public void FlashVignette(float duration)
    {
        flashDuration = duration;
        flashOn = true;
        vignette.intensity.Override(flashIntensity);
    }

    void Update()
    {
        if (flashOn)
        {
            flashTimer += Time.deltaTime;
            float t = 1- flashTimer / flashDuration;
            vignette.intensity.Override(Mathf.Lerp(0f, flashIntensity, t));
            if (flashTimer >= flashDuration)
            {
                flashOn = false;
                flashTimer = 0f;
                vignette.intensity.Override(0f);
            }
        }
    }
}
