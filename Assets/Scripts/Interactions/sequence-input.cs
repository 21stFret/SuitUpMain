using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class SequenceInputController : MonoBehaviour
{
    [Header("Sequence Settings")]
    [SerializeField] private int sequenceLength = 5;
    [SerializeField] private float timeLimit = 10f;

    [Header("UI References")]
    [SerializeField] private Image[] sequenceDisplayImages;
    [SerializeField] private Image timerFillImage;
    [SerializeField] private GameObject sequencePanel;

    [Header("Arrow Settings")]
    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private Sprite completeSprite;
    [Tooltip("Rotation offset if your arrow sprite isn't pointing up by default")]
    [SerializeField] private float baseRotationOffset = 0f;

    [Header("Input Settings")]
    [SerializeField] private InputActionAsset inputActions;
    private InputAction upAction;
    private InputAction downAction;
    private InputAction leftAction;
    private InputAction rightAction;

    // Enumeration for possible directions
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private Direction[] currentSequence;
    private int currentInputIndex;
    private float remainingTime;
    private bool isSequenceActive;
    private Dictionary<Direction, float> directionRotations;

    public delegate void SequenceEvent();
    public event SequenceEvent OnSequenceComplete;
    public event SequenceEvent OnSequenceFailed;

    public string ActionMapName = "Gameplay";

    public void RemoveAllListeners()
    {
        OnSequenceComplete = null;
        OnSequenceFailed = null;
    }

    private void Awake()
    {
        // Setup Input Actions
        var gameplayMap = inputActions.FindActionMap(ActionMapName);
        upAction = gameplayMap.FindAction("Up");
        downAction = gameplayMap.FindAction("Down");
        leftAction = gameplayMap.FindAction("Left");
        rightAction = gameplayMap.FindAction("Right");

        // Subscribe to input events
        upAction.performed += ctx => OnDirectionPressed(Direction.Up);
        downAction.performed += ctx => OnDirectionPressed(Direction.Down);
        leftAction.performed += ctx => OnDirectionPressed(Direction.Left);
        rightAction.performed += ctx => OnDirectionPressed(Direction.Right);
    }

    private void OnEnable()
    {
        // Enable input actions
        upAction?.Enable();
        downAction?.Enable();
        leftAction?.Enable();
        rightAction?.Enable();
    }

    private void OnDisable()
    {
        // Disable input actions
        upAction?.Disable();
        downAction?.Disable();
        leftAction?.Disable();
        rightAction?.Disable();
    }

    private void Start()
    {
        InitializeDirectionMappings();
        //sequencePanel.SetActive(false);

        if(ActionMapName == "UI")
        {
            LoadSetSequence();
        }

    }

    private void InitializeDirectionMappings()
    {
        // Define rotation angles for each direction
        directionRotations = new Dictionary<Direction, float>
        {
            { Direction.Up, 0f },
            { Direction.Left, 90f },
            { Direction.Down, 180f },
            { Direction.Right, 270f }
        };
    }

    public void StartNewSequence()
    {
        InitializeDirectionMappings();
        GenerateRandomSequence();
        DisplaySequence();
        StartCoroutine(SequenceTimer());
        isSequenceActive = true;
        currentInputIndex = 0;
    }

    private void GenerateRandomSequence()
    {
        currentSequence = new Direction[sequenceLength];
        for (int i = 0; i < sequenceLength; i++)
        {
            currentSequence[i] = (Direction)Random.Range(0, 4);
        }
    }

    public void LoadSetSequence()
    {
        sequenceLength = 4;
        currentSequence = new Direction[sequenceLength];
        for (int i = 0; i < sequenceLength; i++)
        {
            switch (i)
            {
                case 0:
                    currentSequence[i] = Direction.Up;
                    break;
                case 1:
                    currentSequence[i] = Direction.Down;
                    break;
                case 2:
                    currentSequence[i] = Direction.Left;
                    break;
                case 3:
                    currentSequence[i] = Direction.Right;
                    break;
            }
        }
        isSequenceActive = true;
        currentInputIndex = 0;
    }

    private void DisplaySequence()
    {
        sequencePanel.SetActive(true);
        for (int i = 0; i < sequenceLength; i++)
        {
            if (i < sequenceDisplayImages.Length)
            {
                sequenceDisplayImages[i].sprite = arrowSprite;
                sequenceDisplayImages[i].rectTransform.localRotation = Quaternion.Euler(0, 0, directionRotations[currentSequence[i]] + baseRotationOffset);
                sequenceDisplayImages[i].color = Color.white;
            }
        }
    }

    private void OnDirectionPressed(Direction inputDirection)
    {
        if (!isSequenceActive) return;

        CheckInput(inputDirection);
    }

    private void CheckInput(Direction inputDirection)
    {
        if (currentSequence[currentInputIndex] == inputDirection)
        {
            if (sequenceDisplayImages.Length > 0)
            {
                sequenceDisplayImages[currentInputIndex].sprite = completeSprite; 
            }
            // Correct input
            currentInputIndex++;
            AudioManager.instance.PlaySFX(SFX.Select);
            if (currentInputIndex >= sequenceLength)
            {
                SequenceSuccess();
            }


        }
        else
        {
            // Wrong input
            SequenceFailed();
        }
    }

    private void SequenceSuccess()
    {
        isSequenceActive = false;
        OnSequenceComplete?.Invoke();
        AudioManager.instance.PlaySFX(SFX.Confirm);
    }

    private void SequenceFailed()
    {
        isSequenceActive = false;
        OnSequenceFailed?.Invoke();

        // Visual feedback for failure
        for (int i = currentInputIndex; i < sequenceLength; i++)
        {
            sequenceDisplayImages[i].color = Color.red;
        }

        StartCoroutine(RefreshSequence(1f));
    }

    private IEnumerator SequenceTimer()
    {
        remainingTime = timeLimit;

        while (remainingTime > 0 && isSequenceActive)
        {
            remainingTime -= Time.deltaTime;
            if (timerFillImage != null)
            {
                timerFillImage.fillAmount = remainingTime / timeLimit;
            }
            yield return null;
        }

        if (isSequenceActive)
        {
            SequenceFailed();
        }
    }

    private IEnumerator RefreshSequence(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNewSequence();
    }

    // Public getter for testing and debugging
    public Direction[] GetCurrentSequence()
    {
        return currentSequence;
    }
}