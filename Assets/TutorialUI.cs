using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public TMP_Text instructionText;
    public GameObject instructionPanel;
    public MechHealth mechHealth;
    public GameObject mechHealthObject;
    public GameObject mechFuelObject;
    public WeaponFuelManager weaponFuelManager;
    public MechLoader mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;
    public GameObject mainWeapon;
    public GameObject mainWeaponImage;
    public GameObject altWeapon;
    public GameObject altWeaponImage;
    public GameObject pulseBar;
    public PulseShockwave pulseShockwave;
    public GameObject airDrop;
    public float textSpeed = 0.05f;

    public TutorialInputUI[] inputUIs; // Array of TutorialInputUI components
    public GameObject[] inputTicks;
    public GameObject inputPanel; // Panel containing input UIs
    public Sprite[] controlSprites; // Array of control sprites (keyboard, gamepad, etc.)

    private Dictionary<string, int> controlIndexMap; // Map control names to sprite indices

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
        yield return StartCoroutine(PrintText("Initializing Mech..."));
        mechHealthObject.SetActive(true);
        mechHealth.enabled = true;
        mechHealth.targetHealth.health = mechHealth.targetHealth.maxHealth;
        mechFuelObject.SetActive(true);
        weaponFuelManager._enabled = true;
        weaponFuelManager.weaponRechargeRate = 35;
        yield return new WaitForSeconds(3f);
        yield return StartCoroutine(PrintText("Loading Complete! Scanning environment..."));
        yield return new WaitForSeconds(1f);
        weaponFuelManager.weaponRechargeRate = 15;
        instructionPanel.SetActive(false);
        //yield return StartCoroutine(EnableMainWeapon());
    }

    public IEnumerator StartCombatTraining()
    {
        mainWeapon.SetActive(true);
        mainWeaponImage.SetActive(true);
        yield return StartCoroutine(PrintText("Weapons enabled. Prepare for combat training."));
        yield return new WaitForSeconds(1f);
        instructionPanel.SetActive(false);
    }

    public void ShowInstructions(string instructions)
    {
        StartCoroutine(PrintText(instructions));
    }

    public void UpdateInputInstructions(string[] controls, string[] instructions)
    {
        HideAllInputUIs();

        Sprite[] controlSprites = InputTracker.instance.usingMouse? controlSprites;

        for (int i = 0; i < controls.Length && i < inputUIs.Length; i++)
        {
            inputPanel.SetActive(true);
            inputUIs[i].gameObject.SetActive(true);
            inputUIs[i].inputImage.sprite = controlSprites[controlIndexMap[controls[i]]];
            inputUIs[i].inputText.text = instructions[i];
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

    public IEnumerator EndTutorial()
    {
        PlayerSavedData.instance.UpdateFirstLoad(false);
        PlayerSavedData.instance.SavePlayerData();
        yield return StartCoroutine(PrintText("Systems check complete. Ready for live combat!"));
        yield return new WaitForSeconds(1f);
        SceneLoader.instance.LoadScene(2);
    }

    public IEnumerator PrintText(string text)
    {
        instructionPanel.SetActive(true);
        instructionText.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            instructionText.text += text[i];
            yield return new WaitForSeconds(textSpeed);
        }
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

    public void SwapControlsToPC(bool isPC)
    {
        // Implement logic to swap between PC and console sprites
        // This might involve having separate sprite arrays for PC and console
        // and updating the controlSprites reference based on isPC
    }
}