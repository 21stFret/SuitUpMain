using UnityEngine;

public class DirectionalDaylight : MonoBehaviour
{
    [Header("Light State")]
    public bool isDay = true;
    public bool isEvening = false;

    [Header("Rotation Settings")]
    public float rotationMinX = 30f;
    public float rotationMaxX = 80f;
    
    [Header("Day Settings")]
    public float dayIntensity = 1.0f;
    public float dayTempMin = 5500f;
    public float dayTempMax = 6500f;
    [Range(0.3f, 1f)]
    public float dayHeightSlider = 0.7f; // Slider to control the height of the sun during the day

    [Header("Evening Settings")]
    public float eveningIntensity = 0.7f;
    public float eveningTempMin = 3000f; 
    public float eveningTempMax = 4000f;
    
    [Header("Night Settings")]
    public float nightIntensity = 0.1f;
    public float nightTempMin = 1500f;
    public float nightTempMax = 2500f;

    private Light directionalLight;
    private Vector3 targetRotation;

    [InspectorButton("ApplyLightSettings")]
    public bool applySettings;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        directionalLight = GetComponent<Light>();
        ApplyLightSettings();
    }

    public void SetTimeOfDay(bool day, bool evening = false)
    {
        isDay = day;
        isEvening = evening && day; // Evening is only valid during day
        
        ApplyLightSettings();
    }

    public void ApplyLightSettings()
    {
        if (directionalLight == null)
            directionalLight = GetComponent<Light>();

        float randomY = Random.Range(0f, 360f); // Random Y rotation for variety
        
        isDay = Random.Range(0f, 1f) > 0.3f; // Randomly decide if it's day or night
        isEvening = Random.Range(0f, 1f) > 0.5f; // Randomly decide if it's evening

        if (isDay)
        {
            if (isEvening)
            {
                // Evening settings - mid rotation, high temp, mid intensity
                float midRotationX = Mathf.Lerp(rotationMinX, rotationMaxX * 0.6f, 0.5f);
                targetRotation = new Vector3(midRotationX, randomY, 0);
                
                directionalLight.intensity = eveningIntensity;
                directionalLight.colorTemperature = Random.Range(eveningTempMin, eveningTempMax);
            }
            else
            {
                // Day settings - top 30% rotation, mid temp, high intensity
                float highRotationX = Mathf.Lerp(rotationMaxX * dayHeightSlider, rotationMaxX, Random.value);
                targetRotation = new Vector3(highRotationX, randomY, 0);
                
                directionalLight.intensity = dayIntensity;
                directionalLight.colorTemperature = Random.Range(dayTempMin, dayTempMax);
            }
        }
        else
        {
            // Night settings - any rotation, low temp, low intensity
            float anyRotationX = Random.Range(rotationMinX, rotationMaxX);
            targetRotation = new Vector3(anyRotationX, randomY, 0);
            
            directionalLight.intensity = nightIntensity;
            directionalLight.colorTemperature = Random.Range(nightTempMin, nightTempMax);
        }
        
        // Apply rotation
        transform.rotation = Quaternion.Euler(targetRotation);
    }

    // Legacy method renamed for backwards compatibility
    public void CreateRandomRotation()
    {
        ApplyLightSettings();
    }
}