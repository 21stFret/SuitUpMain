using UnityEngine;

public class DirectionalDaylight : MonoBehaviour
{
    [Header("Sun Settings")]
    public float dayDuration = 24f;
    public float startTime = 0.25f;
    public float dawnTime = 0.25f;  // 6 AM
    public float duskTime = 0.75f;  // 6 PM

    [Header("Colors")]
    [SerializeField]
    private Gradient skyGradient;
    public Color dawnColor = new Color(1f, 0.6f, 0.4f);
    public Color dayColor = Color.white;
    public Color duskColor = new Color(1f, 0.5f, 0.3f);
    public Color nightColor = new Color(0.1f, 0.1f, 0.3f);

    [Header("Intensity")]
    public float dayIntensity = 1f;
    public float nightIntensity = 0.1f;

    private Light directionalLight;
    private float currentTime;
    public Vector3 nightRotation;
    public Vector3 dawnRotation;
    public Vector3 duskRotation;
    public Vector3 middayRotation;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        skyGradient = null;
        directionalLight = GetComponent<Light>();
        currentTime = startTime;
        SetupGradient();
    }

    void Update()
    {
        currentTime += Time.deltaTime / dayDuration;
        if (currentTime >= 1f)
            currentTime -= 1f;

        UpdateSunRotation();
        UpdateLightSettings(currentTime);
    }

    void UpdateSunRotation()
    {
        if (currentTime < dawnTime || currentTime > duskTime)
        {
            // Keep light pointing down during night
            transform.rotation = Quaternion.Euler(nightRotation);
            return;
        }

        // Calculate daytime rotation (dawn to dusk)
        float dayProgress = (currentTime - dawnTime) / (duskTime - dawnTime);
        float middayTime = (dawnTime + duskTime) / 2f;

        if (currentTime < middayTime)
        {
            float hlafdayProgress = 1f - Mathf.Abs(dayProgress - 0.5f) * 2f;
            Vector3 sunAngle = Vector3.Lerp(dawnRotation, middayRotation, hlafdayProgress);
            transform.rotation = Quaternion.Euler(sunAngle);
        }
        else
        {
            float hlafdayProgress =  Mathf.Abs(dayProgress - 0.5f) * 2f;
            Vector3 sunAngle = Vector3.Lerp(middayRotation, duskRotation, hlafdayProgress);
            transform.rotation = Quaternion.Euler(sunAngle);
        }

    }

    void SetupGradient()
    {
        if (skyGradient == null)
        {
            GradientColorKey[] colorKeys = new GradientColorKey[5];
            colorKeys[0] = new GradientColorKey(nightColor, 0f);
            colorKeys[1] = new GradientColorKey(dawnColor, dawnTime);
            colorKeys[2] = new GradientColorKey(dayColor, 0.5f);
            colorKeys[3] = new GradientColorKey(duskColor, duskTime);
            colorKeys[4] = new GradientColorKey(nightColor, 1f);

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(1f, 1f);

            skyGradient = new Gradient();
            skyGradient.SetKeys(colorKeys, alphaKeys);
        }
    }

    void UpdateLightSettings(float time)
    {
        directionalLight.color = skyGradient.Evaluate(time);

        float intensityMultiplier = 1f;
        if (time < dawnTime || time > duskTime)
        {
            intensityMultiplier = nightIntensity;
        }
        else
        {
            float dayProgress = (time - dawnTime) / (duskTime - dawnTime);

            if (dayProgress < 0.5f) // Morning
            {
                intensityMultiplier = Mathf.Lerp(nightIntensity, dayIntensity, dayProgress * 2f);
            }
            else // Afternoon
            {
                intensityMultiplier = Mathf.Lerp(dayIntensity, nightIntensity, (dayProgress - 0.5f) * 2f);
            }
        }

        directionalLight.intensity = intensityMultiplier;
    }
}