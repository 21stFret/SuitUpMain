using System.Collections;
using UnityEngine;

public class EnhancedFlickerController : MonoBehaviour
{
    [System.Serializable]
    public class MaterialSettings
    {
        public bool enableMaterialFlicker = true;
        public Material lightsOFF;
        public Material lightsON;
        public int targetSubMeshIndex = 2;
    }

    [System.Serializable]
    public class LightSettings
    {
        public bool enableLightFlicker = true;
        public Light[] lightsToFlicker;
        [Range(0f, 1f)]
        public float intensityMultiplierWhenOff = 0f;
    }

    [System.Serializable]
    public class SoundSettings
    {
        public bool enableSoundEffects = true;
        public AudioClip[] flickerSounds;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.5f, 2f)]
        public float randomPitchRange = 1.1f;
    }

    [Header("Timing Settings")]
    public float minTime = 0.1f;
    public float maxTime = 0.5f;

    [Header("Material Settings")]
    public MaterialSettings materialSettings;

    [Header("Light Settings")]
    public LightSettings lightSettings;

    [Header("Sound Settings")]
    public SoundSettings soundSettings;

    private MeshRenderer meshRenderer;
    private AudioSource audioSource;
    private bool isOn = false;
    private float[] originalIntensities;

    void Start()
    {
        // Get or add required components
        if (materialSettings.enableMaterialFlicker)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                Debug.LogWarning("MeshRenderer not found but material flickering is enabled!");
            }
        }

        if (soundSettings.enableSoundEffects)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1f; // Make sound fully 3D
            }
        }

        // Validate and store original light intensities
        if (lightSettings.enableLightFlicker)
        {
            if (lightSettings.lightsToFlicker == null || lightSettings.lightsToFlicker.Length == 0)
            {
                Debug.LogWarning("No lights assigned but light flickering is enabled!");
            }
            else
            {
                // Store original intensities
                originalIntensities = new float[lightSettings.lightsToFlicker.Length];
                for (int i = 0; i < lightSettings.lightsToFlicker.Length; i++)
                {
                    if (lightSettings.lightsToFlicker[i] != null)
                    {
                        originalIntensities[i] = lightSettings.lightsToFlicker[i].intensity;
                    }
                }
            }
        }

        // Start flickering
        StartCoroutine(FlickerRoutine());
    }

    private void PlayFlickerSound()
    {
        if (!soundSettings.enableSoundEffects ||
            soundSettings.flickerSounds == null ||
            soundSettings.flickerSounds.Length == 0
            // || audioSource.isPlaying)
            )
        {
            return;
        }

        // Pick a random sound from the array
        AudioClip randomClip = soundSettings.flickerSounds[Random.Range(0, soundSettings.flickerSounds.Length)];
        if (randomClip != null)
        {
            audioSource.clip = randomClip;
            audioSource.volume = soundSettings.volume;
            audioSource.pitch = Random.Range(1f / soundSettings.randomPitchRange, soundSettings.randomPitchRange);
            audioSource.Play();
        }
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Toggle state
            isOn = !isOn;

            // Update materials if enabled
            if (materialSettings.enableMaterialFlicker && meshRenderer != null)
            {
                Material[] materials = meshRenderer.sharedMaterials;
                materials[materialSettings.targetSubMeshIndex] = isOn ?
                    materialSettings.lightsON :
                    materialSettings.lightsOFF;
                meshRenderer.sharedMaterials = materials;
            }

            // Update lights if enabled
            if (lightSettings.enableLightFlicker && lightSettings.lightsToFlicker != null)
            {
                for (int i = 0; i < lightSettings.lightsToFlicker.Length; i++)
                {
                    Light light = lightSettings.lightsToFlicker[i];
                    if (light != null)
                    {
                        float originalIntensity = originalIntensities[i];
                        light.intensity = isOn ?
                            originalIntensity :
                            Mathf.Lerp(0f, originalIntensity, lightSettings.intensityMultiplierWhenOff);
                    }
                }
            }

            // Play sound effect
            if(isOn)
            {
                PlayFlickerSound();
            }


            // Wait for next cycle
            yield return new WaitForSeconds(Random.Range(minTime, maxTime));
        }
    }

    // Optional: Public method to force a specific state
    public void SetFlickerState(bool on)
    {
        isOn = on;

        if (materialSettings.enableMaterialFlicker && meshRenderer != null)
        {
            Material[] materials = meshRenderer.sharedMaterials;
            materials[materialSettings.targetSubMeshIndex] = on ?
                materialSettings.lightsON :
                materialSettings.lightsOFF;
            meshRenderer.sharedMaterials = materials;
        }

        if (lightSettings.enableLightFlicker && lightSettings.lightsToFlicker != null)
        {
            for (int i = 0; i < lightSettings.lightsToFlicker.Length; i++)
            {
                Light light = lightSettings.lightsToFlicker[i];
                if (light != null)
                {
                    float originalIntensity = originalIntensities[i];
                    light.intensity = on ?
                        originalIntensity :
                        Mathf.Lerp(0f, originalIntensity, lightSettings.intensityMultiplierWhenOff);
                }
            }
        }

        PlayFlickerSound();
    }
}