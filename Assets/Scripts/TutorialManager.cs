using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using FORGE3D;

public class TutorialManager : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public MYCharacterController myCharacterController;
    public WeaponController manualWeaponController;
    public DroneControllerUI droneController;
    public TutorialUI tutorialUI;
    public PlayerInput playerInput;
    public MechLoader mechLoader;
    public ConnectWeaponHolderToManager weaponHolder;
    public GameObject crawlers1;
    public GameObject crawlers2;
    public TestPlayerData testPlayerData;

    public static TutorialManager instance;

    public Button skipButton;

    private enum TutorialStage
    {
        BasicMovement,
        Combat,
        OffensiveSystems,
        SupportSystems,
        Complete
    }

    private TutorialStage currentStage = TutorialStage.BasicMovement;

    private bool hasMoved;
    private bool hasAimed;
    private bool hasDashed;
    private bool hasFiredPrimary;
    private bool hasFiredSecondary;
    private bool hasUsedPulse;
    private bool hasKilledAll1;
    private bool hasKilledAll2;
    private bool hasOpenedDroneMenu;
    private bool hasOpenedDroneMenu2;
    private bool hasUsedRepair;
    private bool hasselected1;
    private bool hasselected2;

    private bool canCheckProgress = false;

    private Crawler[] crawlersArray1;
    private Crawler[] crawlersArray2;

    public bool combatTextCheck;
    public bool droneTextCheck;

    void Start()
    {
        instance = this;
        Invoke("InitScene", 0.1f);
        InitializeComponents();
        StartCoroutine(RunTutorial());
        combatTextCheck = false;
        droneTextCheck = false;
    }

    private void InitScene()
    {
        if (sceneLoader == null)
        {
            testPlayerData.InittestData();
        }
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
        AudioManager.instance.Init();
        AudioManager.instance.PlayBGMusic(2);
        DroneAbilityManager.instance.Init();
    }

    void Update()
    {
        CheckTutorialProgress();
    }

    private void InitializeComponents()
    {
        manualWeaponController.enabled = false;
        myCharacterController.ToggleCanMove(false);
        playerInput.SwitchCurrentActionMap("Gameplay");
        droneController.enabled = false;
        sceneLoader = SceneLoader.instance;
        crawlersArray1 = crawlers1.GetComponentsInChildren<Crawler>();
        crawlersArray2 = crawlers2.GetComponentsInChildren<Crawler>();
    }

    private IEnumerator RunTutorial()
    {
        EnableMovement();
        manualWeaponController.Init();
        yield return StartCoroutine(tutorialUI.InitializeMech());

        while (currentStage != TutorialStage.Complete)
        {
            switch (currentStage)
            {
                case TutorialStage.BasicMovement:
                    yield return StartCoroutine(RunBasicMovement());
                    break;
                case TutorialStage.Combat:
                    yield return tutorialUI.StartCoroutine(tutorialUI.StartCombatTraining());
                    while (!combatTextCheck)
                    {
                        yield return null;
                    }
                    yield return StartCoroutine(RunAdvancedCombat());
                    break;
                case TutorialStage.OffensiveSystems:
                    yield return StartCoroutine(RunOffensiveSystems());
                    break;
                case TutorialStage.SupportSystems:
                    yield return tutorialUI.StartCoroutine(tutorialUI.StartDroneTraining());
                    while (!droneTextCheck)
                    {
                        yield return null;
                    }
                    yield return StartCoroutine(RunSupportSystems());
                    break;
            }
            yield return null;
        }

        yield return StartCoroutine(tutorialUI.EndTutorial());
    }

    private IEnumerator RunBasicMovement()
    {
        yield return tutorialUI.currentTextCoroutine = tutorialUI.StartCoroutine(tutorialUI.PrintText("Master basic movement controls to continue."));
        tutorialUI.UpdateInputInstructions(
            new string[] { "move", "aim", "dash" },
            new string[] { "Use <image>button</image> to Move.", "Use <image>button</image> to Aim.", "Press <image>button</image> to Dash" }
        );
        canCheckProgress = true;

        while (!hasMoved || !hasAimed || !hasDashed)
        {
            yield return null;
        }
        canCheckProgress = false;
        yield return tutorialUI.currentTextCoroutine = StartCoroutine(tutorialUI.PrintText("Well Done!\nBasic movement complete!"));
        yield return tutorialUI.currentTextCoroutine = StartCoroutine(tutorialUI.PrintText("...noob.exe not needed...", true));
        tutorialUI.HideAllInputUIs();
        yield return new WaitForSeconds(1f);
        currentStage = TutorialStage.Combat;
    }

    private IEnumerator RunAdvancedCombat()
    {
        yield return tutorialUI.currentTextCoroutine = tutorialUI.StartCoroutine(tutorialUI.PrintText("Utilize basic combat controls and exterminate all enemies to continue."));
        tutorialUI.UpdateInputInstructions(
            new string[] { "firePrimary", "fireSecondary", "pulse" },
            new string[] { "Press <image>button</image> to Fire Assault Weapon", "Press <image>button</image> to Fire Tech Weapon", "Press <image>button</image> to Pulse" }
        );
        EnableSecondaryWeaponAndPulse();
        canCheckProgress = true;
        crawlers1.SetActive(true);
        while (!hasFiredPrimary || !hasFiredSecondary || !hasUsedPulse || !hasKilledAll1)
        {
            yield return null;
        }
        canCheckProgress = false;
        yield return tutorialUI.currentTextCoroutine = tutorialUI.StartCoroutine(tutorialUI.PrintText("Crawlers eradicated! Congratulations on passing Basic Combat."));
        yield return tutorialUI.currentTextCoroutine = tutorialUI.StartCoroutine(tutorialUI.PrintText("....this one might have potential...", true));
        tutorialUI.HideAllInputUIs();
        yield return new WaitForSeconds(1f);
        currentStage = TutorialStage.OffensiveSystems;
    }

    private IEnumerator RunOffensiveSystems()
    {
        yield return tutorialUI.currentTextCoroutine = tutorialUI.StartCoroutine(tutorialUI.PrintText("Enabling D.R.O.N.E system.\n'Deliver Repairs Or Neutralize Enemies'."));
        yield return tutorialUI.currentTextCoroutine = tutorialUI.StartCoroutine(tutorialUI.PrintText("Call in an Air Strike!"));
        tutorialUI.UpdateInputInstructions(
            new string[] { "openDrone", "select", "bomb" },
            new string[] { "Press <image>button</image> to open the Drone Menu", "Use <image>button</image> to input the Sequence", "Kill all Crawlers!" }
        );
        EnableAirStrike();
        canCheckProgress = true;
        while (!hasselected1 || !hasOpenedDroneMenu || !hasKilledAll2)
        {
            yield return null;
        }
        canCheckProgress = false;
        yield return tutorialUI.currentTextCoroutine = tutorialUI.StartCoroutine(tutorialUI.PrintText("Each charge of the drone bar will increase the abilites effect!)"));
        yield return tutorialUI.currentTextCoroutine = tutorialUI.StartCoroutine(tutorialUI.PrintText("Offensive systems complete."));
        tutorialUI.HideAllInputUIs();
        currentStage = TutorialStage.SupportSystems;
    }

    private IEnumerator RunSupportSystems()
    {
        yield return tutorialUI.currentTextCoroutine = tutorialUI.StartCoroutine(tutorialUI.PrintText("Call in a Repair!"));
        tutorialUI.UpdateInputInstructions(
            new string[] { "openDrone", "select" ,"repair" },
            new string[] { "Press <image>button</image> to open the Drone Menu", "Press <image>button</image> to input the Sequence", "Collect drop box to Repair" }
        );
        EnableDroneAndRepair();
        canCheckProgress = true;
        while (!hasOpenedDroneMenu2 || !hasselected2 || !hasUsedRepair)
        {
            yield return null;
        }
        canCheckProgress = false;
        yield return tutorialUI.currentTextCoroutine = tutorialUI.StartCoroutine(tutorialUI.PrintText("Support systems complete."));
        tutorialUI.HideAllInputUIs();
        currentStage = TutorialStage.Complete;
    }

    private void CheckTutorialProgress()
    {
        if (currentStage == TutorialStage.Complete) return;
        if(!canCheckProgress) return;
        switch (currentStage)
        {
            case TutorialStage.BasicMovement:
                if (!hasMoved && myCharacterController._moveInputVector.magnitude > 0)
                {
                    hasMoved = true;
                    tutorialUI.SetControlGreen(0);
                }
                if (!hasAimed && manualWeaponController.isAiming)
                {
                    hasAimed = true;
                    tutorialUI.SetControlGreen(1);
                }
                if (!hasDashed && myCharacterController.isDodging)
                {
                    hasDashed = true;
                    tutorialUI.SetControlGreen(2);
                }
                break;
            case TutorialStage.Combat:
                if (!hasFiredPrimary && manualWeaponController.mainWeaponEquiped.isFiring)
                {
                    hasFiredPrimary = true;
                    tutorialUI.SetControlGreen(0);
                }
                if (!hasFiredSecondary && manualWeaponController.altWeaponEquiped.isFiring)
                {
                    hasFiredSecondary = true;
                    tutorialUI.SetControlGreen(1);
                }
                if (!hasUsedPulse && !tutorialUI.pulseShockwave.canUsePulseWave)
                {
                    hasUsedPulse = true;
                    tutorialUI.SetControlGreen(2);
                }
                int killed = 0;
                foreach (Crawler crawler in crawlersArray1)
                {
                    if (crawler.dead)
                    {
                        killed++;
                    }
                }
                if (killed == crawlersArray1.Length)
                {
                    hasKilledAll1 = true;
                }
                break;
            case TutorialStage.OffensiveSystems:
                if (!hasOpenedDroneMenu && droneController.airdropMenu.activeInHierarchy)
                {
                    hasOpenedDroneMenu = true;
                    tutorialUI.SetControlGreen(0);
                }
                if (!hasselected1 && droneController.airstikes>0)
                {
                    hasselected1 = true;
                    tutorialUI.SetControlGreen(1);
                }
                int killed2 = 0;
                foreach (Crawler crawler in crawlersArray2)
                {
                    if (crawler.dead)
                    {
                        killed2++;
                    }
                }
                if (killed2 == crawlersArray2.Length)
                {
                    hasKilledAll2 = true;
                    tutorialUI.SetControlGreen(2);
                }
                break;
            case TutorialStage.SupportSystems:
                if (!hasOpenedDroneMenu2 && droneController.airdropMenu.activeInHierarchy)
                {
                    hasOpenedDroneMenu2 = true;
                    tutorialUI.SetControlGreen(0);
                }
                if (!hasselected2 && droneController.crates >0)
                {
                    hasselected2 = true;
                    tutorialUI.SetControlGreen(1);
                }
                if (!hasUsedRepair && tutorialUI.mechHealth.targetHealth.health > 90)
                {
                    hasUsedRepair = true;
                    tutorialUI.SetControlGreen(2);
                }
                break;
        }
    }

    public void PauseTutorial()
    {
        Time.timeScale = 0;
        DisableMovement();
    }

    public void ResumeTutorial()
    {
        Time.timeScale = 1;
        EnableMovement();
    }

    private void EnableMovement()
    {
        myCharacterController.ToggleCanMove(true);
        manualWeaponController.enabled = true;
    }

    private void DisableMovement()
    {
        manualWeaponController.enabled = false;
        myCharacterController.ToggleCanMove(false);
    }

    private void EnableSecondaryWeaponAndPulse()
    {
        mechLoader.loadMainWeapon = true;
        mechLoader.loadAltWeapon = true;
        mechLoader.Init();
        tutorialUI.altWeapon.SetActive(true);
        tutorialUI.pulseShockwave.enabled = true;
        tutorialUI.pulseBar.SetActive(true);
    }

    private void EnableAirStrike()
    {
        tutorialUI.mechDroneObject.SetActive(true);
        droneController.enabled = true;
        droneController.airDropTimer.charges = 1;
        droneController.FullyChargeDrone();
        crawlers2.SetActive(true);
        crawlers2.transform.SetParent(null);
    }

    private void EnableDroneAndRepair()
    {
        droneController.enabled = true;
        droneController.airDropTimer.charges = 1;
        droneController.FullyChargeDrone();
    }

    public void OpenSkipMenu()
    {
        PauseTutorial();
        var eventSystem = FindObjectOfType<EventSystem>();
        eventSystem.SetSelectedGameObject(skipButton.gameObject);
    }

    public void SkipTutorial()
    {
        Time.timeScale = 1;
        PlayerSavedData.instance.UpdateFirstLoad(false);
        PlayerSavedData.instance.SavePlayerData();
        sceneLoader.LoadScene(2);
    }
}