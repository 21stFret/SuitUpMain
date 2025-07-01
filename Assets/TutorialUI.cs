using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public TMP_Text instructionText;
    public GameObject instructionPanel;
    public MechHealth mechHealth;
    public GameObject mechHealthObject;
    public GameObject mechFuelObject;
    public GameObject mechDroneObject;
    public WeaponFuelManager weaponFuelManager;
    public MechLoader mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;
    public GameObject mainWeapon;
    public GameObject altWeapon;
    public GameObject pulseBar;
    public PulseShockwave pulseShockwave;
    public float textSpeed = 0.05f;

    public TutorialTextWithImage[] inputUIs; // Array of TutorialInputUI components
    public GameObject[] inputTicks;
    public GameObject inputPanel; // Panel containing input UIs
    public Sprite[] controlSpritesPC;
    public Sprite[] controlSpritesGamePad; // Array of control sprites (keyboard, gamepad, etc.)

    private Dictionary<string, int> controlIndexMap; // Map control names to sprite indices
    private string[] curControls;

    private float _fontsize;

    void Awake()
    {
        InitializeControlMap();
    }

    private void InitializeControlMap()
    {
        controlIndexMap = new Dictionary<string, int>
        {
            {"move", 0},
            {"aim", 1},
            {"dash", 2},
            {"firePrimary", 3},
            {"fireSecondary", 4},
            {"pulse", 5},
            {"openDrone", 6},
            {"select", 7},
            {"bomb", 8},
            {"repair", 9},
        };
    }

    public IEnumerator InitializeMech()
    {
        _fontsize = instructionText.fontSize;
        mechFuelObject.SetActive(false);
        mechHealthObject.SetActive(false);
        yield return StartCoroutine(PrintText("New user detected, running initialization protocols..."));
        StartCoroutine(PrintText("Health and Fuel systems loading..."));
        yield return new WaitForSeconds(1f);
        mechHealthObject.SetActive(true);
        mechHealth.targetHealth.Init(null, mechHealth);
        mechHealth.targetHealth.health = 1;
        mechHealth.UpdateHealthUI(0);
        mechHealth.SetHealthBar(0);
        yield return new WaitForSeconds(0.5f);
        mechHealth.Heal(100);
        mechFuelObject.SetActive(true);
        weaponFuelManager._enabled = true;
        weaponFuelManager.weaponRechargeRate = 35;
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(PrintText("Systems check complete! Preparing training environment..."));
        yield return new WaitForSeconds(1f);
        weaponFuelManager.weaponRechargeRate = 15;
        instructionPanel.SetActive(false);
    }

    public IEnumerator StartCombatTraining()
    {
        mainWeapon.SetActive(true);
        currentTextCoroutine = StartCoroutine(PrintText("Weapons have now been enabled. Please get ready for combat training."));
        yield return currentTextCoroutine;
        yield return new WaitForSeconds(1f);
        instructionPanel.SetActive(false);
        TutorialManager.instance.combatTextCheck = true;
    }

    public IEnumerator StartDroneTraining()
    {
        currentTextCoroutine = StartCoroutine(PrintText("Well Done! Now simulating pain!"));
        yield return currentTextCoroutine;
        mechHealth.TakeDamage(20);
        instructionPanel.SetActive(false);
        TutorialManager.instance.droneTextCheck = true;
    }

    public IEnumerator EndTutorial()
    {
        PlayerSavedData.instance.UpdateFirstLoad(false);
        PlayerSavedData.instance.SavePlayerData();
        currentTextCoroutine = StartCoroutine(PrintText("All systems check complete. Ready for live combat!"));
        yield return currentTextCoroutine;
        currentTextCoroutine = StartCoroutine(PrintText("...Interesting...\n...record this time line...", true));
        yield return currentTextCoroutine;
        yield return new WaitForSeconds(1f);
        TutorialManager.instance.SkipTutorial();
    }

    public void UpdateInputInstructions(string[] controls, string[] instructions)
    {
        HideAllInputUIs();

        curControls = controls;
         
        Sprite[] controlSprites = InputTracker.instance.usingMouse? controlSpritesPC : controlSpritesGamePad;

        for (int i = 0; i < controls.Length && i < inputUIs.Length; i++)
        {
            inputPanel.SetActive(true);
            inputUIs[i].gameObject.SetActive(true);
            inputUIs[i].SetTutorialText(instructions[i], controlSprites[controlIndexMap[controls[i]]]);
        }
    }

    public void UpdateInputImages()
    {
        if(curControls == null)
        {
            return;
        }
        Sprite[] controlSprites = InputTracker.instance.usingMouse ? controlSpritesPC : controlSpritesGamePad;
        for (int i = 0; i < inputUIs.Length; i++)
        {
            inputUIs[i].UpdateImage(controlSprites[controlIndexMap[curControls[i]]]);
        }
    }

    public void HideAllInputUIs()
    {
        inputPanel.SetActive(false);
        foreach (var inputTicks in inputTicks)
        {
            inputTicks.gameObject.SetActive(false);
        }
        foreach (var inputUI in inputUIs)
        {
            inputUI.gameObject.SetActive(false);
        }
    }

    public Coroutine currentTextCoroutine;

    public IEnumerator PrintText(string text, bool small = false)
    {
        if (currentTextCoroutine != null)
        {
            StopCoroutine(currentTextCoroutine);
        }

        instructionText.fontSize = small ? _fontsize * 0.75f : _fontsize;
        instructionText.fontStyle = small ? FontStyles.Italic : FontStyles.Normal;
        instructionPanel.SetActive(true);
        instructionText.text = "";

        for (int i = 0; i < text.Length; i++)
        {
            instructionText.text += text[i];
            yield return new WaitForSeconds(textSpeed);
        }
        yield return new WaitForSeconds(1f);
        currentTextCoroutine = null;
    }

    public void SetControlGreen(int control)
    {
        for (int i = 0; i < inputUIs.Length; i++)
        {
            if (!inputTicks[i].gameObject.activeSelf && control == i)
            {
                inputTicks[i].SetActive(true);
                break;
            }
        }
    }
}