using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.UI.Extensions;
public class ResolutionManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    
    [Header("Confirmation Popup")]
    [SerializeField] private GameObject confirmationPopup;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private int pendingResolutionIndex;
    private bool pendingFullscreenValue;
    private float confirmationTimer;
    private const float REVERT_TIME = 15f; // Time in seconds before settings auto-revert
    
    private Resolution currentResolution;
    private bool currentFullscreen;

    public bool FirstMenuLoad;

    public TMP_Dropdown dropdown;
    private UIScrollToSelection scrollHandler;

    void Awake()
    {
        // Load preferences early
        LoadPrefs();
        
        // Make sure the popup is hidden at start
        if (confirmationPopup != null)
            confirmationPopup.SetActive(false);
    }
    
    void LoadPrefs()
    {
        // Load saved resolution settings
        int width = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        int height = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        if (width == 0 || height == 0)
        {
            width = 1920;
            height = 1080;
        }

        Screen.SetResolution(width, height, fullscreen);
    }
    
    void Start()
    {
        if(FirstMenuLoad)
        {
            return;
        }
        
        // Store initial settings
        currentResolution = new Resolution 
        { 
            width = Screen.width, 
            height = Screen.height 
        };
        currentFullscreen = Screen.fullScreen;
        
        // Setup confirmation buttons
        if (confirmButton != null)
            confirmButton.onClick.AddListener(ConfirmSettings);
        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelSettings);
        
        // Start a coroutine to initialize UI with a delay
        StartCoroutine(InitializeUIWithDelay());

        // Get or add the UIScrollToSelection component
        scrollHandler = dropdown.GetComponentInChildren<UIScrollToSelection>(true);
        if (scrollHandler == null)
        {
            ScrollRect scrollRect = dropdown.template.GetComponent<ScrollRect>();
            scrollHandler = scrollRect.gameObject.AddComponent<UIScrollToSelection>();
            scrollHandler.InitializeDropdown(scrollRect);
        }

                // Set scroll settings
        scrollHandler.scrollSpeed = 15f; // Faster for dropdowns
        scrollHandler.YPadding = 5f;  
    }
    
    IEnumerator InitializeUIWithDelay()
    {
        // Wait for two frames to ensure UI is ready
        yield return null;
        yield return null;
        
        // Initialize resolution data
        InitializeResolutionData();
        
        // Wait another frame before setting up the dropdown
        yield return null;
        
        // Set up the UI components
        SetupResolutionDropdown();
        
        // Setup fullscreen toggle (do this after resolution dropdown is set up)
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
        }
        
        // Force one more UI update
        Canvas.ForceUpdateCanvases();
        
        // Output debug info
        //Debug.Log($"Resolution manager initialized with {filteredResolutions.Count} resolutions");
        //Debug.Log($"Current resolution: {Screen.width}x{Screen.height}, Fullscreen: {Screen.fullScreen}");
    }
    
    void InitializeResolutionData()
    {
        // Common resolutions (16:9 and 16:10 aspect ratios)
        List<Resolution> commonResolutions = new List<Resolution>
        {
            new Resolution { width = 1920, height = 1080 }, // FHD
            new Resolution { width = 1600, height = 900 },  // HD+
            new Resolution { width = 1366, height = 768 },  // HD
            new Resolution { width = 1280, height = 720 },  // 720p
            new Resolution { width = 2560, height = 1440 }, // 2K
            new Resolution { width = 3840, height = 2160 }  // 4K
        };

        // Get the native resolution
        Resolution nativeResolution = new Resolution 
        { 
            width = Display.main.systemWidth,
            height = Display.main.systemHeight 
        };

        // Add native resolution if it's not already in the list
        if (!commonResolutions.Any(r => r.width == nativeResolution.width && r.height == nativeResolution.height))
        {
            commonResolutions.Add(nativeResolution);
        }

        // Sort by total pixels (descending)
        filteredResolutions = commonResolutions
            .OrderByDescending(res => res.width * res.height)
            .ToList();

        // Filter out resolutions higher than the native resolution
        filteredResolutions = filteredResolutions
            .Where(res => res.width <= nativeResolution.width && res.height <= nativeResolution.height)
            .ToList();

        //Debug.Log($"Initialized with {filteredResolutions.Count} common resolutions including native {nativeResolution.width}x{nativeResolution.height}");
    }
    
    void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null)
        {
            Debug.LogError("Resolution dropdown is null!");
            return;
        }
        
        // Clear dropdown first
        resolutionDropdown.ClearOptions();
        
        // Remove any existing listeners to prevent duplicates
        resolutionDropdown.onValueChanged.RemoveAllListeners();
        
        // Create a list of string options instead of using TMPro OptionData directly
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string option = $"{filteredResolutions[i].width} x {filteredResolutions[i].height}";
            options.Add(option);
            
            if (filteredResolutions[i].width == Screen.width && 
                filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        // Add the string options to the dropdown
        resolutionDropdown.AddOptions(options);
        
        // Force UI update
        Canvas.ForceUpdateCanvases();
        
        // Set the current value
        if (currentResolutionIndex < options.Count)
        {
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
        
        // Add the value changed listener
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        
        //Debug.Log($"Dropdown setup complete. Selected index: {currentResolutionIndex}");
    }
    
    void OnResolutionChanged(int index)
    {
        pendingResolutionIndex = index;
        //Debug.Log($"Resolution changed to index: {index}");
    }
    
    void OnFullscreenToggled(bool isFullscreen)
    {
        pendingFullscreenValue = isFullscreen;
        //Debug.Log($"Fullscreen toggled: {isFullscreen}");
    }
    
    public void ShowConfirmationPopup()
    {
        if (pendingResolutionIndex >= 0 && pendingResolutionIndex < filteredResolutions.Count)
        {
            confirmationPopup.SetActive(true);
            confirmationTimer = REVERT_TIME;
            
            // Apply pending changes temporarily
            Resolution newResolution = filteredResolutions[pendingResolutionIndex];
            Screen.SetResolution(newResolution.width, newResolution.height, pendingFullscreenValue);
            
            Debug.Log($"Applying temporary resolution: {newResolution.width}x{newResolution.height}, Fullscreen: {pendingFullscreenValue}");
        }
        else
        {
            Debug.LogError($"Invalid resolution index: {pendingResolutionIndex}");
        }
    }
    
    void Update()
    {
        if (FirstMenuLoad)
        {
            return;
        }
        
        // Handle confirmation timeout
        if (confirmationPopup.activeSelf)
        {
            confirmationTimer -= Time.deltaTime;
            if (confirmationTimer <= 0)
            {
                CancelSettings();
            }
        }
    }
    
    void ConfirmSettings()
    {
        if (pendingResolutionIndex >= 0 && pendingResolutionIndex < filteredResolutions.Count)
        {
            // Save current settings
            currentResolution = filteredResolutions[pendingResolutionIndex];
            currentFullscreen = pendingFullscreenValue;
            
            confirmationPopup.SetActive(false);

            // Save settings to PlayerPrefs
            PlayerPrefs.SetInt("ResolutionWidth", currentResolution.width);
            PlayerPrefs.SetInt("ResolutionHeight", currentResolution.height);
            PlayerPrefs.SetInt("Fullscreen", currentFullscreen ? 1 : 0);
            PlayerPrefs.Save();
            
            Debug.Log($"Settings confirmed and saved: {currentResolution.width}x{currentResolution.height}, Fullscreen: {currentFullscreen}");
        }
    }
    
    void CancelSettings()
    {
        // Revert to previous settings
        Screen.SetResolution(currentResolution.width, currentResolution.height, currentFullscreen);
        
        // Reset UI to match current settings
        int index = filteredResolutions.FindIndex(x => 
            x.width == currentResolution.width && x.height == currentResolution.height);
            
        if (index >= 0 && index < filteredResolutions.Count)
        {
            resolutionDropdown.value = index;
            resolutionDropdown.RefreshShownValue();
        }
        
        fullscreenToggle.isOn = currentFullscreen;
        
        confirmationPopup.SetActive(false);
        
        Debug.Log($"Settings reverted to: {currentResolution.width}x{currentResolution.height}, Fullscreen: {currentFullscreen}");
    }
}