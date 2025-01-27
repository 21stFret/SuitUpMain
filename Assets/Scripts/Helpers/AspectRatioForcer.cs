using UnityEngine;

public class AspectRatioEnforcer : MonoBehaviour
{
    public Vector2 screenSize = new Vector2(1920, 1080);
    public Vector2 aspectRatio  = new Vector2(16, 9);
    [SerializeField] private float targetAspectRatio; // or 16f/10f
    private Camera mainCamera;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        UpdateCameraRect();
        targetAspectRatio = aspectRatio.x / aspectRatio.y;
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