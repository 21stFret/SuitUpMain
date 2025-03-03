using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class CreditsController : MonoBehaviour
{
    [Header("Scrolling Settings")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 0.1f;
    
    [Header("Skip Button Settings")]
    [SerializeField] private GameObject skipButton;
    [SerializeField] private float skipButtonDelay = 3f;
    [SerializeField] private float skipHoldDuration = 1.5f;
    [SerializeField] private string nextSceneName = "MainMenu";

    private bool isScrolling = true;
    private float skipHoldTimer = 0f;
    private bool canSkip = false;
    public DoTweenFade doTweenFade;
    public Image fillBar;

    private bool pressed;

    public PlayerInput playerInput;

    public bool loadScene;

    private void OnEnable()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerInput.SwitchCurrentActionMap("Credits");
        isScrolling = false;
        pressed = false;
        // Ensure scroll position starts at top
        scrollRect.verticalNormalizedPosition = 1f;
        
        // Hide skip button initially
        if (skipButton != null)
        {
            skipButton.SetActive(false);
            StartCoroutine(ShowCreditsDelayed());
            StartCoroutine(ShowSkipButtonDelayed());
        }
    }

    private void OnDisable()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

    private void Update()
    {
        // Handle automatic scrolling
        if (isScrolling)
        {
            float newPosition = scrollRect.verticalNormalizedPosition - (scrollSpeed * Time.deltaTime);
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newPosition);

            // Check if we've reached the bottom
            if (scrollRect.verticalNormalizedPosition <= 0f)
            {
                isScrolling = false;
                Invoke("LoadNextScene", 10f);
            }
        }

        if (pressed)
        {
            skipHoldTimer += Time.deltaTime;
            fillBar.fillAmount = skipHoldTimer / skipHoldDuration;
            if (skipHoldTimer >= skipHoldDuration)
            {
                canSkip = false;
                isScrolling = false;
                if (loadScene)
                {
                    LoadNextScene();
                }
                else
                {
                    doTweenFade.FadeOut();
                    this.gameObject.SetActive(false);
                }
            }
        }
    }

    public IEnumerator ShowCreditsDelayed()
    {
        doTweenFade.FadeIn();
        yield return new WaitForSeconds(2);
        isScrolling = true;
    }

    public void SkipButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (canSkip)
            {
                pressed = true;
            }
        }
        if (context.canceled)
        {
            skipHoldTimer = 0f;
            fillBar.fillAmount = 0f;
            pressed = false;
        }
    }

    private IEnumerator ShowSkipButtonDelayed()
    {
        yield return new WaitForSeconds(skipButtonDelay);
        skipButton.SetActive(true);
        canSkip = true;
    }

    private void LoadNextScene()
    {
        SceneLoader.instance.LoadScene(2);
    }
}
