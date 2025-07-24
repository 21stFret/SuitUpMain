using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeCircuitboardManager : MonoBehaviour
{
    public static UpgradeCircuitboardManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public RunUpgradeManager runUpgradeManager;
    public List<ChipSlotUI> slots = new List<ChipSlotUI>();
    public ComboLock[] comboLocks;
    [HideInInspector]
    public RunMod currentRunMod = null;
    public ChipSlotUI currentChipSlot;
    public GameObject firstSelectedChipSlot;
    public PauseModUI pauseModUI;
    public InputActionReference ineteractSlotAction;
    public ModEntry _modIcon;
    public Button CloseMenuButton;
    public bool readOnly;
    public GameObject statInfoPanel;
    public List<StatInfoUI> statInfoUI;
    public GameObject rootNodeHighlight;
    private int poweredNodes;
    [Header("Circuit Tutorial")]
    public GameObject circuitTutPanel;
    public GameObject circuitTutPopUp;
    public TMP_Text circuitTutText;
    private bool isCircuitTutorialActive = false;
    private int part;
    public Button NextButton;
    public Button PreviousButton;
    public List<GameObject> tutorialParts;
    public Vector3 offset;
    private EventSystem eventSystem;

    private void Start()
    {
        Init();
        eventSystem = EventSystem.current;
    }

    public void Init()
    {
        runUpgradeManager = GameManager.instance.runUpgradeManager;
        poweredNodes = 0;
        foreach (var slot in slots)
        {
            slot.runUpgradeManager = runUpgradeManager;
            slot.upgradeCircuitboardManager = this;
            slot.currentRunMod = null;
        }
        ineteractSlotAction.action.Enable();
        ineteractSlotAction.action.performed += InteractWithSlot;
        CloseMenuButton.gameObject.SetActive(!readOnly);
        CloseMenuButton.enabled = true;
        if (currentRunMod != null)
        {
            CloseMenuButton.enabled = false;
        }
    }

    public void PowerNodesUnlockedCheck(bool value)
    {
        if (value)
        {
            poweredNodes++;
        }
        else
        {
            poweredNodes--;
        }

        if (poweredNodes < 0)
        {
            poweredNodes = 0;
            return; // Prevent negative powered nodes
        }
        if(PlayerAchievements.instance == null)
        {
            Debug.LogWarning("PlayerAchievements instance is null. Cannot set achievements.");
            print("PoweredNodes = " + poweredNodes);
            return; // Prevent null reference if PlayerAchievements is not initialized
        }
        if (poweredNodes == 1)
        {
            PlayerAchievements.instance.SetAchievement("MOD_1");
        }
        else if (poweredNodes == 3)
        {
            PlayerAchievements.instance.SetAchievement("MOD_3");
        }
        if (poweredNodes >= 4)
        {
            //runUpgradeManager.UnlockCircuitBoard();
            PlayerAchievements.instance.SetAchievement("MOD_4");
        }
    }

    public void InteractWithSlot(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (currentChipSlot == null)
        {
            //Debug.LogError("No chip slot selected. Please select a chip slot before interacting.");
            return;
        }
        currentChipSlot.InteractWithSlot();
    }

    public void SelectChipSlot(ChipSlotUI chipSlot)
    {
        currentChipSlot = chipSlot;
    }

    public void OnOpenCircuitBoard()
    {
        if (PlayerSavedData.instance != null)
        {
            if (PlayerSavedData.instance.circuitTut)
            {
                circuitTutPanel.SetActive(false);
            }
            else
            {
                PreviousButton.onClick.AddListener(PreviousPart);
                part = 0;
                StartCoroutine(ShowCircuitTutorial());
            }
        }
    }

    public void OpenTutorialButton()
    {
        StartCoroutine(ShowCircuitTutorial());
    }

    public void OnCloseCircuitBoard()
    {
        if (currentChipSlot != null)
        {
            currentChipSlot.chipSlotHighlight.SetActive(false);
        }
        if (_modIcon != null)
        {
            _modIcon.OnHoverExit();
        }
        currentChipSlot = null;
        _modIcon = null;
        CloseMenuButton.enabled = true;
        StopAllCoroutines();
        circuitTutPanel.SetActive(false);
        isCircuitTutorialActive = false;
        statInfoPanel.SetActive(false);
        rootNodeHighlight.SetActive(false);
        BattleMech.instance.myCharacterController.ToggleCanMove(true);
    }

    public void ShowCurrentStats()
    {
        statInfoPanel.SetActive(true);
        rootNodeHighlight.SetActive(true);
        var allCurrentMultipliers = runUpgradeManager.GetAllCurrentMultipliers();

        foreach (var statInfo in statInfoUI)
        {
            statInfo.gameObject.SetActive(false);
        }

        for (int i = 0; i < allCurrentMultipliers.Count && i < statInfoUI.Count; i++)
        {
            StatInfoUI statInfo = statInfoUI[i];
            if (statInfo == null)
            {
                Debug.LogError("StatInfoUI is null for index: " + i);
                continue;
            }

            float statValue = allCurrentMultipliers[(StatType)i];

            // Skip if value is 0, NaN, or infinity
            if (statValue == 0 || float.IsNaN(statValue) || float.IsInfinity(statValue))
            {
                continue;
            }

            statInfo.gameObject.SetActive(true);
            statInfo.UpdateStatValue(statValue * 100); // Assuming you want to show percentage
            statInfo.UpdateStatName(((StatType)i).ToString().Replace("_", " "));
        }
    }

    public void HideCurrentStats()
    {
        statInfoPanel.SetActive(false);
        rootNodeHighlight.SetActive(false);
    }

    public IEnumerator ShowCircuitTutorial()
    {
        if (isCircuitTutorialActive)
        {
            yield break; // Prevent multiple activations
        }
        part = 0; // Reset part for the tutorial
        CloseMenuButton.gameObject.SetActive(false);
        isCircuitTutorialActive = true;
        circuitTutPanel.SetActive(true);
        while (circuitTutPanel.activeSelf)
        {
            switch (part)
            {
                case 0:
                    circuitTutText.text = "Welcome to the Circuit Board Upgrade System! Here, you can enhance your run with various mods.";
                    NextButton.GetComponentInChildren<TMP_Text>().text = "Next";
                    NextButton.onClick.RemoveAllListeners();
                    NextButton.onClick.AddListener(NextPart);   
                    PreviousButton.gameObject.SetActive(false);
                    circuitTutPopUp.SetActive(true);
                    circuitTutPopUp.transform.localPosition = tutorialParts[part].transform.localPosition + offset;
                    eventSystem.SetSelectedGameObject(NextButton.gameObject);
                    break;
                case 1:
                    circuitTutText.text = "You can click on these slots to equip your upgrade chips.";
                    PreviousButton.gameObject.SetActive(true);
                    circuitTutPopUp.transform.localPosition = tutorialParts[part].transform.localPosition + offset;
                    break;
                case 2:
                    circuitTutText.text = "Each Power Node has 4 unlock combinations. Combine the right chips to unlock them!";
                    circuitTutPopUp.transform.localPosition = tutorialParts[part].transform.localPosition + offset;
                    break;
                case 3:
                    circuitTutText.text = "The golden corner nodes can double your mod effects! They require any 2 upgrade chips to power them!";
                    circuitTutPopUp.transform.localPosition = tutorialParts[part].transform.localPosition + offset;
                    break;
                case 4:
                    circuitTutText.text = "These smaller slots do not effect your mech but can be used for storage or sorting!";
                    NextButton.GetComponentInChildren<TMP_Text>().text = "Next";
                    circuitTutPopUp.transform.localPosition = tutorialParts[part].transform.localPosition + offset;
                    NextButton.onClick.RemoveAllListeners();
                    NextButton.onClick.AddListener(NextPart);
                    break;
                case 5:
                    circuitTutText.text = "You can check your stats at any time from the Root Node. Good Luck!";
                    NextButton.GetComponentInChildren<TMP_Text>().text = "Finish";
                    NextButton.onClick.RemoveAllListeners();
                    NextButton.onClick.AddListener(EndTutorial);
                    circuitTutPopUp.transform.localPosition = tutorialParts[part].transform.localPosition + offset;
                    break;
                default:
                    circuitTutPanel.SetActive(false);
                    isCircuitTutorialActive = false;
                    yield break;
            }
            yield return null;
        }
    }

    public void NextPart()
    {
        part++;
        if (part >= tutorialParts.Count)
        {
            part = 0; // Loop back to the start
        }
    }
    public void PreviousPart()
    {
        part--;
        if (part < 0)
        {
            part = tutorialParts.Count - 1; // Loop back to the end
            eventSystem.SetSelectedGameObject(NextButton.gameObject);
        }
        
    }

    public void EndTutorial()
    {
        circuitTutPanel.SetActive(false);
        isCircuitTutorialActive = false;
        CloseMenuButton.gameObject.SetActive(true);
        part = 0; // Reset part for future tutorials
        PlayerSavedData.instance.circuitTut = true;
        PlayerSavedData.instance.SavePlayerData();
        eventSystem.SetSelectedGameObject(firstSelectedChipSlot);
    }
}

[Serializable]
public class ComboLock
{
    public List<ModBuildType> buildLocks;
    public bool isLocked;

    public void ResetLock()
    {
        isLocked = true;
    }

    public bool TryUnlock(List<ModBuildType> modTypes)
    {
        // Check if both lists have the same count first
        if (buildLocks.Count != modTypes.Count)
            return false;
        
        // Create sorted copies and compare
        var sortedRequired = new List<ModBuildType>(buildLocks);
        var sortedProvided = new List<ModBuildType>(modTypes);
        
        sortedRequired.Sort();
        sortedProvided.Sort();
        
        // Compare sorted lists element by element
        for (int i = 0; i < sortedRequired.Count; i++)
        {
            if (sortedRequired[i] != sortedProvided[i])
                return false;
        }
        
        isLocked = false;
        return true;
    }

    public void CreateRandomLock()
    {
        int randomModType = UnityEngine.Random.Range(0, 4);
        buildLocks.Clear();
        for (int i = 0; i < 4; i++)
        {
            buildLocks.Add((ModBuildType)randomModType);
            randomModType = (randomModType + UnityEngine.Random.Range(0, 4)) % 4; // Ensure variety in mod types
        }
        ResetLock();
        isLocked = true;
    }
    public List<ModBuildType> GetRequiredCombination()
    {
        return buildLocks; // Or whatever your field is called
    }


}
