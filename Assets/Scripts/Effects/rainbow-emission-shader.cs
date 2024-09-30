using UnityEngine;

public class SmoothRainbowEmission : MonoBehaviour
{
    public float speed = 0.5f;
    public float saturation = 1f;
    public float brightness = 1f;
    public int colorSteps = 7; // Number of colors in the rainbow

    private Material material;
    private float[] hues;
    private float transition = 0f;
    private int currentColorIndex = 0;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        material = renderer.material;

        // Initialize hues for rainbow colors
        hues = new float[colorSteps];
        for (int i = 0; i < colorSteps; i++)
        {
            hues[i] = (float)i / colorSteps;
        }
    }

    void Update()
    {
        transition += speed * Time.deltaTime;

        if (transition >= 1f)
        {
            transition -= 1f;
            currentColorIndex = (currentColorIndex + 1) % colorSteps;
        }

        int nextColorIndex = (currentColorIndex + 1) % colorSteps;
        Color currentColor = Color.HSVToRGB(hues[currentColorIndex], saturation, brightness);
        Color nextColor = Color.HSVToRGB(hues[nextColorIndex], saturation, brightness);

        Color lerpedColor = Color.Lerp(currentColor, nextColor, transition);
        material.SetColor("_EmissionColor", lerpedColor);
    }
}