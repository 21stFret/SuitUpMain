using UnityEngine;
using System.Collections.Generic;

public class RunwayLightsController : MonoBehaviour
{
    [System.Serializable]
    public class LightPair
    {
        public Light leftLight;
        public Light rightLight;
    }

    [Header("Light Configuration")]
    [SerializeField] private List<LightPair> lightPairs = new List<LightPair>();
    [SerializeField] private float timeBetweenPairs = 0.5f;
    [SerializeField] private float intensityWhenOn = 1.0f;
    
    [Header("Animation Settings")]
    [SerializeField] private bool loopAnimation = true;
    [SerializeField] private float delayBetweenLoops = 2.0f;
    
    private float timer = 0f;
    private int currentPairIndex = 0;
    private bool isAnimating = false;
    private bool isWaitingForLoop = false;

    private void Start()
    {
        // Initialize all lights to off
        foreach (var pair in lightPairs)
        {
            if (pair.leftLight != null) pair.leftLight.intensity = 0f;
            if (pair.rightLight != null) pair.rightLight.intensity = 0f;
        }

        StartLightSequence();
    }

    private void Update()
    {
        if (!isAnimating) return;

        timer += Time.deltaTime;

        if (isWaitingForLoop)
        {
            if (timer >= delayBetweenLoops)
            {
                ResetAnimation();
            }
            return;
        }

        // Check if it's time to update the lights
        if (timer >= timeBetweenPairs)
        {
            // Turn off previous pair
            if (currentPairIndex > 0)
            {
                TurnOffLightPair(currentPairIndex - 1);
            }

            // Turn on current pair
            if (currentPairIndex < lightPairs.Count)
            {
                TurnOnLightPair(currentPairIndex);
                currentPairIndex++;
            }
            else if (loopAnimation)
            {
                // Prepare for next loop
                isWaitingForLoop = true;
                timer = 0f;
                return;
            }
            else
            {
                // Stop animation if we're not looping
                isAnimating = false;
                return;
            }

            timer = 0f;
        }
    }

    private void TurnOnLightPair(int index)
    {
        if (index >= 0 && index < lightPairs.Count)
        {
            var pair = lightPairs[index];
            /*
            if (pair.leftLight != null) pair.leftLight.gameObject.SetActive(true);
            if (pair.rightLight != null) pair.rightLight.gameObject.SetActive(true);
            */
            if (pair.leftLight != null) pair.leftLight.intensity = intensityWhenOn;
            if (pair.rightLight != null) pair.rightLight.intensity = intensityWhenOn;
        }
    }

    private void TurnOffLightPair(int index)
    {
        if (index >= 0 && index < lightPairs.Count)
        {
            var pair = lightPairs[index];
            /*
            if (pair.leftLight != null) pair.leftLight.gameObject.SetActive(false);
            if (pair.rightLight != null) pair.rightLight.gameObject.SetActive(false);
            */
            if (pair.leftLight != null) pair.leftLight.intensity = 0f;
            if (pair.rightLight != null) pair.rightLight.intensity = 0f;
        }
    }

    public void StartLightSequence()
    {
        ResetAnimation();
        isAnimating = true;
    }

    public void StopLightSequence()
    {
        isAnimating = false;
        // Turn off all lights
        for (int i = 0; i < lightPairs.Count; i++)
        {
            TurnOffLightPair(i);
        }
    }

    private void ResetAnimation()
    {
        currentPairIndex = 0;
        timer = timeBetweenPairs;
        isWaitingForLoop = false;
        
        // Ensure all lights are off
        for (int i = 0; i < lightPairs.Count; i++)
        {
            //TurnOffLightPair(i);
        }
    }
}
