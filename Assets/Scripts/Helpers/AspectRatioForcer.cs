using UnityEngine;

public class AspectRatioEnforcer : MonoBehaviour
{
    [SerializeField] private float targetAspectRatio = 16f / 9f; // or 16f/10f
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        UpdateCameraRect();
        //Screen.SetResolution(1920, 1080, true);
    }

    void UpdateCameraRect()
    {
        float currentAspectRatio = (float)Screen.width / Screen.height;
        float scaleHeight = currentAspectRatio / targetAspectRatio;

        if (scaleHeight < 1f)
        {
            Rect rect = mainCamera.rect;
            rect.width = 1f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1f - scaleHeight) / 2f;
            mainCamera.rect = rect;
        }
        else
        {
            float scaleWidth = 1f / scaleHeight;
            Rect rect = mainCamera.rect;
            rect.width = scaleWidth;
            rect.height = 1f;
            rect.x = (1f - scaleWidth) / 2f;
            rect.y = 0;
            mainCamera.rect = rect;
        }
    }
}