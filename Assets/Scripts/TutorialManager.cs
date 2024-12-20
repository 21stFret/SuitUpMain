using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using FORGE3D;

public class TutorialManager : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public MYCharacterController myCharacterController;
    public WeaponController manualWeaponController;
    public DroneController droneController;
    public TutorialUI tutorialUI;
    public PlayerInput playerInput;
    public MechLoader mechLoader;
    public ConnectWeaponHolderToManager weaponHolder;
    public GameObject crawlers1;
    public GameObject crawlers2;
    public GameObject DroneRepair;
    public GameObject DroneAirStrike;

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
    private bool hasUsedAttack;
    private bool hasselected1;
    private bool hasselected2;

    private bool canCheckProgress = false;

    private Crawler[] crawlersArray1;
    private Crawler[] crawlersArray2;

    void Start()
    {
        Invoke("InitScene", 0.1f);
        InitializeComponents();
        StartCoroutine(RunTutorial());
    }

    private void InitScene()
    {
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
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
                    yield return StartCoroutine(RunAdvancedCombat());
                    break;
                case TutorialStage.OffensiveSystems:
                    yield return StartCoroutine(RunOffensiveSystems());
                    break;
                case TutorialStage.SupportSystems:
                    yield return StartCoroutine(RunSupportSystems());
                    break;
            }
            yield return null;
        }

        yield return StartCoroutine(tutorialUI.EndTutorial());
    }

    private IEnumerator RunBasicMovement()
    {
        tutorialUI.ShowInstructions("Master basic movement.");
        tutorialUI.UpdateInputInstructions(
            new string[] { "move", "aim", "dash" },
            new string[] { "To Move", "To Aim", "To Dash"}
        );
        canCheckProgress = true;

        while (!hasMoved || !hasAimed || !hasDashed)
        {
            yield return null;
        }
        canCheckProgress = false;
        yield return tutorialUI.StartCoroutine(tutorialUI.PrintText("Basic Movement Complete."));
        tutorialUI.HideAllInputUIs();
        yield return new WaitForSeconds(1f);
        currentStage = TutorialStage.Combat;
    }

    private IEnumerator RunAdvancedCombat()
    {
        tutorialUI.ShowInstructions("Use basic combat techniques.");
        tutorialUI.UpdateInputInstructions(
            new string[] { "firePrimary", "fireSecondary", "pulse" },
            new string[] { "To Fire Primary", "To Fire Secondary", "To Pulse" }
        );
        EnableSecondaryWeaponAndPulse();
        canCheckProgress = true;
        crawlers1.SetActive(true);
        while (!hasFiredPrimary || !hasFiredSecondary || !hasUsedPulse || !hasKilledAll1)
        {
            yield return null;
        }
        canCheckProgress = false;
        yield return tutorialUI.StartCoroutine(tutorialUI.PrintText("Basic Combat Complete."));
        tutorialUI.HideAllInputUIs();
        yield return new WaitForSeconds(1f);
        currentStage = TutorialStage.OffensiveSystems;
    }

    private IEnumerator RunSupportSystems()
    {
        tutorialUI.ShowInstructions("Utilize support systems.");
        tutorialUI.UpdateInputInstructions(
            new string[] { "openDrone", "select" ,"repair" },
            new string[] { "To Open Drone Menu", "To Select Ability", "Collect box To Repair" }
        );
        EnableDroneAndRepair();
        canCheckProgress = true;
        while (!hasOpenedDroneMenu2 || !hasselected2 || !hasUsedRepair)
        {
            yield return null;
        }
        canCheckProgress = false;
        yield return tutorialUI.StartCoroutine(tutorialUI.PrintText("Support Systems Complete."));
        tutorialUI.HideAllInputUIs();
        currentStage = TutorialStage.Complete;
    }

    private IEnumerator RunOffensiveSystems()
    {
        tutorialUI.ShowInstructions("Utilize offensive systems.");
        tutorialUI.UpdateInputInstructions(
            new string[] { "openDrone", "select", "bomb" },
            new string[] { "To Open Drone Menu", "To Select Ability", "To Repair" }
        );
        EnableAirStrike();
        canCheckProgress = true;
        while (!hasUsedAttack || !hasselected1 || !hasOpenedDroneMenu || !hasKilledAll2)
        {
            yield return null;
        }
        canCheckProgress = false;
        yield return tutorialUI.StartCoroutine(tutorialUI.PrintText("Offensive Systems Complete."));
        tutorialUI.HideAllInputUIs();
        currentStage = TutorialStage.SupportSystems;
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
                if (!hasselected1 && droneController.timesUsed>0)
                {
                    hasselected1 = true;
                    tutorialUI.SetControlGreen(1);
                }
                if (!hasUsedAttack && droneController.airstikes>0)
                {
                    hasUsedAttack = true;
                    tutorialUI.SetControlGreen(2);
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
                }
                break;
            case TutorialStage.SupportSystems:
                if (!hasOpenedDroneMenu2 && droneController.airdropMenu.activeInHierarchy)
                {
                    hasOpenedDroneMenu2 = true;
                    tutorialUI.SetControlGreen(0);
                }
                if (!hasselected2 && droneController.timesUsed > 1)
                {
                    hasselected2 = true;
                    tutorialUI.SetControlGreen(1);
                }
                if (!hasUsedRepair && tutorialUI.mechHealth.targetHealth.health > 70)
                {
                    hasUsedRepair = true;
                    tutorialUI.SetControlGreen(2);
                }
                break;
        }
    }

    private void EnableMovement()
    {
        myCharacterController.ToggleCanMove(true);
        manualWeaponController.enabled = true;
        manualWeaponController.Init();
        playerInput.ActivateInput();
    }

    private void EnableSecondaryWeaponAndPulse()
    {
        mechLoader.loadMainWeapon = true;
        mechLoader.loadAltWeapon = true;
        mechLoader.Init();
        tutorialUI.altWeapon.SetActive(true);
        tutorialUI.altWeaponImage.SetActive(true);
        tutorialUI.pulseShockwave.enabled = true;
        tutorialUI.pulseBar.SetActive(true);
    }

    private void EnableAirStrike()
    {
        DroneRepair.SetActive(false);
        DroneAirStrike.SetActive(true);
        droneController.enabled = true;
        droneController.FullyChargeDrone();
        tutorialUI.airDrop.SetActive(true);
        crawlers2.SetActive(true);
        crawlers2.transform.SetParent(null);
    }

    private void EnableDroneAndRepair()
    {
        DroneRepair.SetActive(true);
        DroneAirStrike.SetActive(false);
        droneController.enabled = true;
        droneController.FullyChargeDrone();
        tutorialUI.airDrop.SetActive(true);
        tutorialUI.mechHealth.TakeDamage(10);
    }

    public void SkipTutorial()
    {
        GameUI.instance.pauseMenu.ResumeGame();
        PlayerSavedData.instance.UpdateFirstLoad(false);
        PlayerSavedData.instance.SavePlayerData();
        sceneLoader.LoadScene(2);
    }
}