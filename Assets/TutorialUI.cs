using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    private TutorialManager _tutorialManager;
    public TMP_Text loadUpText;
    public GameObject loadUpPanel;
    public TMP_Text loadUpWText;
    public GameObject loadUpWPanel;
    public TMP_Text ControlText;
    public GameObject ControlPanel;
    public MechHealth mechHealth;
    public GameObject mechHealthObject;
    public GameObject mechFuelObject;
    public WeaponFuelManager weaponFuelManager;
    public MechLoadOut mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;
    public GameObject mainWeapon;
    public GameObject mainWeaponImage;
    public GameObject altWeapon;
    public GameObject altWeaponImage;
    public float textSpeed;
    public GameObject enemies;
    public GameObject enemies2;
    public Sprite[] Controls;
    public Sprite[] ControlsPC;
    public Image controlImage;
    private int controlIndex = 0;
    public GameObject dodgeBoots, dodgeBoots2;
    public GameObject pulseBar;
    public PulseShockwave pulseShockwave;
    public GameObject airDrop;

    private void Awake()
    {
        _tutorialManager = GetComponent<TutorialManager>();
    }

    void Start()
    {
        mechHealth.SetEmmisveHeatlh(0);
        mechHealth.SetEmmisiveStrength(0);
        Invoke("DelayedStart", 0.5f);
    }

    private void Update()
    {
        SwapControls();
    }

    public void DelayedStart()
    {
        altWeapon.SetActive(false);
        StartCoroutine(PrintText("Initialzing Mech...", loadUpText));
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
        mechLoadOut.Init();
        AudioManager.instance.PlayMusic(1);
        StartCoroutine(LoadUI());
        mechHealth.SetEmmisiveStrength(0.5f);
        pulseShockwave.enabled = false;
    }

    IEnumerator LoadUI()
    {
        loadUpPanel.SetActive(true);
        weaponFuelManager.constantUse = true;
        weaponFuelManager._enabled = false;
        yield return new WaitForSeconds(2f);
        mechHealth.image.fillAmount = 0;
        mechHealthObject.SetActive(true);
        mechHealth.enabled = true;
        StartCoroutine(PrintText("Loading Mech Health...", loadUpText));
        mechHealth.SetEmmisiveStrength(2);
        mechHealth.UpdateHealth(100, true);
        yield return new WaitForSeconds(3f);
        mechFuelObject.SetActive(true);
        StartCoroutine(PrintText("Loading Weapon Fuel...", loadUpText));
        weaponFuelManager.weaponFuel = 0;
        weaponFuelManager._enabled = true;
        weaponFuelManager.weaponRechargeRate = 35;
        yield return new WaitForSeconds(3f);
        weaponFuelManager.weaponRechargeRate = 15;
        StartCoroutine(PrintText("Loading Complete! \nScanning environment...", loadUpText));
        yield return new WaitForSeconds(4f);
        loadUpPanel.SetActive(false);
        StartCoroutine(LoadWeaponsUI());
    }

    IEnumerator LoadWeaponsUI()
    {
        enemies.SetActive(true);
        loadUpWPanel.SetActive(true);
        StartCoroutine(PrintText("Enemies have been detected. \nRunning battle simulation.", loadUpWText));
        yield return new WaitForSeconds(5f);
        mainWeapon.SetActive(true);
        mainWeaponImage.SetActive(true);
        StartCoroutine(PrintText("Primary weapon enabled. \nWill auto fire when in range.", loadUpWText));
        yield return new WaitForSeconds(4f);
        loadUpWPanel.SetActive(false);
        ShowControlPanel(0);
        _tutorialManager.runTest = true;
    }

    public IEnumerator LoadWeaponsUI2()
    {
        loadUpPanel.SetActive(true);
        StartCoroutine(PrintText("Targets eliminated.", loadUpText));
        yield return new WaitForSeconds(3f);
        loadUpPanel.SetActive(false);
        altWeapon.SetActive(true);
        altWeaponImage.SetActive(true);
        loadUpWPanel.SetActive(true);
        StartCoroutine(PrintText("Secondary weapon enabled. \nWill consume fuel on use.", loadUpWText));
        yield return new WaitForSeconds(5f);
        loadUpWPanel.SetActive(false);
        enemies2.SetActive(true);
        ShowControlPanel(2);
        _tutorialManager.shootTest = true;
        _tutorialManager.test2 = true;
    }

    public IEnumerator AdvancedMovement()
    {
        loadUpPanel.SetActive(true);
        StartCoroutine(PrintText("Targets eliminated.", loadUpText));
        yield return new WaitForSeconds(3f);
        loadUpPanel.SetActive(false);
        loadUpWPanel.SetActive(true);
        StartCoroutine(PrintText("Dodge and Pulse enabled.", loadUpWText));
        pulseShockwave.enabled = true;
        pulseBar.SetActive(true);
        dodgeBoots.SetActive(true);
        dodgeBoots2.SetActive(true);
        yield return new WaitForSeconds(5f);
        loadUpWPanel.SetActive(false);
        ShowControlPanel(3);
        _tutorialManager.dodgeTest = true;
        _tutorialManager.myCharacterController.candodge = true;
    }

    public IEnumerator DroneControls()
    {
        yield return new WaitForSeconds(1f);
        loadUpWPanel.SetActive(true);
        StartCoroutine(PrintText("Establishing Link to Drone", loadUpWText));
        airDrop.SetActive(true);
        _tutorialManager.droneController.enabled = true;
        yield return new WaitForSeconds(4f);
        StartCoroutine(PrintText("Drone can be used for \npowerful attacks and assits.", loadUpWText));
        yield return new WaitForSeconds(5f);
        StartCoroutine(PrintText("Mech taken damage! \nCall in a repair now!.", loadUpWText));
        mechHealth.TakeDamage(50);
        yield return new WaitForSeconds(3f);
        loadUpWPanel.SetActive(false);
        _tutorialManager.droneTest = true;
        ShowControlPanel(5);
    }

    public IEnumerator EndTutorial()
    {
        loadUpPanel.SetActive(true);
        StartCoroutine(PrintText("Systems check complete. \n Ready for live combat!", loadUpText));
        yield return new WaitForSeconds(5f);
        _tutorialManager.sceneLoader.LoadScene(1);
    }

    public void ShowControlPanel(int index)
    {
        controlImage.color = Color.white;
        ControlPanel.SetActive(true);
        controlIndex = index;
        controlImage.sprite = Controls[controlIndex];
        controlImage.gameObject.SetActive(true);
        switch (index)
        {
            case 0:
                ControlText.text = "to move."; 
                break;
            case 1:
                ControlText.text = "to aim.";
                break;
            case 2: 
                ControlText.text = "to fire.";
                break;
            case 3:
                ControlText.text = "to dodge.";
                break;
            case 4:
                ControlText.text = "to pulse.";
                break;
            case 5:
                ControlText.text = "to open menu.";
                break;
            case 6:
                ControlText.text = "to select.";
                break;
        }
    }

    public void SetControlGreen()
    {
        controlImage.color = Color.green;
    }

    private IEnumerator PrintText(string text, TMP_Text tmp)
    {
        tmp.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            tmp.text += text[i];
            yield return new WaitForSeconds(textSpeed);
        }
    }

    public void SwapControls()
    {
        bool PC = InputTracker.instance.usingMouse;
        controlImage.sprite = !PC ? Controls[controlIndex] : ControlsPC[controlIndex];
    }
}
