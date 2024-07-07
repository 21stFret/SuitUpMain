using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public MYCharacterController myCharacterController;
    public ManualWeaponController manualWeaponController;
    public DroneController droneController;
    public TutorialUI tutorialUI;
    public Crawler[] testCrawlers;
    public Crawler[] testCrawlers2;
    public PlayerInput playerInput;
    public bool test1;
    public bool test2;
    public bool runTest;
    public bool aimTest;
    public bool shootTest;
    public bool shootPTest;
    public bool dodgeTest;
    public bool pulseTest;
    public bool droneTest;
    public bool selectTest;
    public bool repairTest;

    // Start is called before the first frame update
    void Start()
    {
        manualWeaponController.enabled = false;
        myCharacterController.enabled = false;
        playerInput.SwitchCurrentActionMap("Gameplay");
        droneController.enabled = false;
        sceneLoader = SceneLoader.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (test1)
        {
            CheckTest1();
        }
        if (test2)
        {
            CheckTest2();
        }
        if (runTest)
        {
            RunTest();
        }
        if (aimTest)
        {
            AimTest();
        }
        if (shootTest)
        {
            ShootTest();
        }
        if (shootPTest)
        {
            ShootPTest();
        }
        if (dodgeTest)
        {
            DodgeTest();
        }
        if (pulseTest)
        {
            PulseTest();
        }
        if (droneTest)
        {
            DroneTest();
        }
        if (selectTest)
        {
            SelectTest();
        }
        if (repairTest)
        {
            RepairTest();
        }
    }

    public void SkipTutorial()
    {
        GameUI.instance.pauseMenu.ResumeGame();
        PlayerSavedData.instance.UpdateFirstLoad(false);
        PlayerSavedData.instance.SavePlayerData();
        sceneLoader.LoadScene(2);
    }

    public void RepairTest()
    {
        if (tutorialUI.mechHealth.targetHealth.health >70)
        {
            tutorialUI.SetControlGreen();
            repairTest = false;
            StartCoroutine(HidePanel());
            StartCoroutine(tutorialUI.EndTutorial());
        }
    }

    public void SelectTest()
    {
        if (droneController.timesUsed>0)
        {
            tutorialUI.SetControlGreen();
            repairTest = true;
            selectTest = false;
            StartCoroutine(HidePanel());
        }
    }

    public void DroneTest()
    {
        if (droneController.airdropMenu.activeInHierarchy)
        {
            tutorialUI.SetControlGreen();
            droneTest = false;
            StartCoroutine(LoadDronePanel());
        }
    }

    public void PulseTest()
    {
        if (!tutorialUI.pulseShockwave.canUsePulseWave)
        {
            tutorialUI.SetControlGreen();
            StartCoroutine(HidePanel());
            pulseTest = false;
            StartCoroutine(tutorialUI.DroneControls());
        }
    }


    public void DodgeTest()
    {
        if (myCharacterController.isDodging)
        {
            tutorialUI.SetControlGreen();
            dodgeTest = false;
            StartCoroutine(LoadPulsePanel());
        }
    }

    public void ShootTest()
    {
        if (manualWeaponController.equipedWeapon.isFiring)
        {
            tutorialUI.SetControlGreen();
            StartCoroutine(HidePanel());
            shootTest = false;
        }
    }

    public void AimTest()
    {
        if (manualWeaponController.isAiming)
        {
            tutorialUI.SetControlGreen();
            aimTest = false;
            StartCoroutine(HidePanel());
        }
    }

    public void ShootPTest()
    {
        playerInput.ActivateInput();
        myCharacterController.enabled = true;
        if (manualWeaponController.equipedWeaponP.isFiring)
        {
            tutorialUI.SetControlGreen();
            StartCoroutine(HidePanel());
            shootPTest = false;
            StartCoroutine(LoadRunPanel());
        }
    }

    private IEnumerator HidePanel()
    {
        yield return new WaitForSeconds(1f);
        tutorialUI.ControlPanel.SetActive(false);
    }

    public void RunTest()
    {
        if(myCharacterController._moveInputVector.magnitude > 0)
        {
            tutorialUI.SetControlGreen();
            test1 = true;
            runTest = false;
            StartCoroutine(LoadAimPanel());
        }
    }

    private IEnumerator LoadRunPanel()
    {
        yield return new WaitForSeconds(2f);
        tutorialUI.ShowControlPanel(1);
        runTest = true;
    }

    private IEnumerator LoadAimPanel()
    {
        yield return new WaitForSeconds(2f);
        tutorialUI.ShowControlPanel(2);
        aimTest = true;
    }

    private IEnumerator LoadPulsePanel()
    {
        yield return new WaitForSeconds(2f);
        tutorialUI.ShowControlPanel(5);
        pulseTest = true;
    }

    private IEnumerator LoadDronePanel()
    {
        yield return new WaitForSeconds(0.5f);
        tutorialUI.ShowControlPanel(7);
        selectTest = true;
    }

    void CheckTest1()
    {
        if (aimTest || runTest)
        {
            return;
        }
        if (testCrawlers[0].health <= 0 && testCrawlers[1].health <= 0 && testCrawlers[2].health <= 0)
        {
            test1 = false;
            StartCoroutine( tutorialUI.LoadWeaponsUI2());
        }
    }

    void CheckTest2()
    {
        if (shootTest)
        {
            return;
        }
        if (testCrawlers2[0].health <= 0 && testCrawlers2[1].health <= 0 && testCrawlers2[2].health <= 0)
        {
            test2 = false;
            StartCoroutine(tutorialUI.AdvancedMovement());
        }
    }
}
