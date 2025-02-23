using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

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
        LoadPrefs();
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
        
        // Get all available resolutions
        resolutions = Screen.resolutions;
        
        // Filter to unique resolutions
        filteredResolutions = resolutions
            .Select(res => new Resolution { width = res.width, height = res.height })
            .Distinct()
            .OrderByDescending(res => res.width * res.height)
            .ToList();
            
        // Setup resolution dropdown
        SetupResolutionDropdown();
        
        // Setup fullscreen toggle
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
        
        // Setup confirmation buttons
        confirmButton.onClick.AddListener(ConfirmSettings);
        cancelButton.onClick.AddListener(CancelSettings);
        
        // Hide confirmation popup initially
        confirmationPopup.SetActive(false);
    }
    
    void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        int currentResolutionIndex = 0;
        
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string option = $"{filteredResolutions[i].width} x {filteredResolutions[i].height}";
            options.Add(new TMP_Dropdown.OptionData(option));
            
            if (filteredResolutions[i].width == Screen.width && 
                filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }
    
    void OnResolutionChanged(int index)
    {
        pendingResolutionIndex = index;
    }
    
    void OnFullscreenToggled(bool isFullscreen)
    {
        pendingFullscreenValue = isFullscreen;
    }
    
    public void ShowConfirmationPopup()
    {
        confirmationPopup.SetActive(true);
        confirmationTimer = REVERT_TIME;
        
        // Apply pending changes temporarily
        Resolution newResolution = filteredResolutions[pendingResolutionIndex];
        Screen.SetResolution(newResolution.width, newResolution.height, pendingFullscreenValue);
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
        // Save current settings
        currentResolution = filteredResolutions[pendingResolutionIndex];
        currentFullscreen = pendingFullscreenValue;
        
        confirmationPopup.SetActive(false);

        // Save settings to PlayerPrefs
        PlayerPrefs.SetInt("ResolutionWidth", currentResolution.width);
        PlayerPrefs.SetInt("ResolutionHeight", currentResolution.height);
        PlayerPrefs.SetInt("Fullscreen", currentFullscreen ? 1 : 0);
    }
    
    void CancelSettings()
    {
        // Revert to previous settings
        Screen.SetResolution(currentResolution.width, currentResolution.height, currentFullscreen);
        
        // Reset UI to match current settings
        resolutionDropdown.value = filteredResolutions.FindIndex(x => 
            x.width == currentResolution.width && x.height == currentResolution.height);
        fullscreenToggle.isOn = currentFullscreen;
        
        confirmationPopup.SetActive(false);
    }
}