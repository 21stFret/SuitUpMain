using UnityEngine;
using Cinemachine;

public class ScreenShakeUtility : MonoBehaviour
{
    #region Singleton
    public static ScreenShakeUtility Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float defaultAmplitude = 1f;
    [SerializeField] private float defaultFrequency = 0.2f;
    
    private void Start()
    {
        if (impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
            
            if (impulseSource == null)
            {
                Debug.LogWarning("No CinemachineImpulseSource attached. Adding one.");
                impulseSource = gameObject.AddComponent<CinemachineImpulseSource>();
                impulseSource.m_ImpulseDefinition.m_AmplitudeGain = defaultAmplitude;
                impulseSource.m_ImpulseDefinition.m_FrequencyGain = defaultFrequency;
            }
        }
    }
    
    /// <summary>
    /// Generate a screen shake with intensity from 0-1
    /// </summary>
    public void ShakeScreen(float intensity)
    {
        impulseSource.GenerateImpulse(intensity);
    }
    
    /// <summary>
    /// Generate a screen shake with direction vector and intensity
    /// </summary>
    public void ShakeScreen(Vector3 direction, float intensity)
    {
        impulseSource.GenerateImpulse(direction * intensity);
    }
    
    /// <summary>
    /// Generate a screen shake with the default intensity
    /// </summary>
    public void ShakeScreen()
    {
        impulseSource.GenerateImpulse();
    }

    /// <summary>
    /// Generate a screen shake which calculates shake on distance of camera to explosion
    /// </summary>
    public void ShakeScreenDistance(Vector3 explosionPosition)
    {
        float d = Vector3.Distance(Camera.main.transform.position, explosionPosition);
        float maxDistance = 50f; // Maximum distance for shake effect
        d = Mathf.Clamp(d, 0, maxDistance); // Clamp distance to avoid extreme values
        d = 1 - (d / maxDistance); // Normalize to 0-1 range
        impulseSource.GenerateImpulse(d);
    }
}