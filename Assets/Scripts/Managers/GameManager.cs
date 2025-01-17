using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameUI gameUI;
    public PlayerInput playerInput;
    public MechLoader mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;
    public WeaponController altWeaponController;
    public AreaManager areaManager;
    public VoidPortalManager voidPortalManager;
    public VoidAreaManager voidAreaManager;
    public RoomPortal RoomPortal;
    public int currentRoomIndex;
    public bool endlessMode;
    public bool gameActive;
    public GameObject dayLight;
    public GameObject nightLight;
    public ModBuildType nextBuildtoLoad;
    public AreaType currentAreaType;
    public StatMultiplierManager statMultiplierManager;
    public RunUpgradeManager runUpgradeManager;

    public bool playOnAwake = false;
    public TestPlayerData TESTplayerData;

    [InspectorButton("SpawnPortalsToNextRoom")]
    public bool spawnPortal;

    private MYCharacterController _myCharacterController;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        if (SetupGame.instance != null) SetupGame.instance.LinkGameManager(this);
        else
        {
            Debug.Log("No Setup Game Instance Found using test data.");
            TESTplayerData.InittestData();
            Invoke("DelayedStart", 0.1f);
        }
    }

    public void DelayedStart()
    {

        _myCharacterController = BattleMech.instance.myCharacterController;
        _myCharacterController.ToggleCanMove(true);
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
        mechLoadOut.Init();
        InitializeStats();
        AudioManager.instance.Init();
        AudioManager.instance.PlayBattleMusic();

        if (!playOnAwake) return;

        nextBuildtoLoad = (ModBuildType)Random.Range(0, 4);
        currentRoomIndex = -1;
        areaManager.LoadRoom(currentAreaType);
        gameActive = true;
        BattleManager.instance.SetBattleType();
        BattleManager.instance.currentBattleIndex = 0;
        BattleManager.instance.crawlerSpawner.Init();
        BattleManager.instance.UpdateCrawlerSpawner();
        gameUI.objectiveUI.UpdateObjective(BattleManager.instance.objectiveMessage);
        runUpgradeManager.LoadData();
    }

    public void InitializeStats()
    {
        Dictionary<StatType, float> baseStats = new Dictionary<StatType, float>
        {
        { StatType.Health, BattleMech.instance.targetHealth.maxHealth },
        { StatType.Assault_Damage, BattleMech.instance.weaponController.mainWeaponEquiped.damage},
        { StatType.Speed, BattleMech.instance.myCharacterController.Speed },
        { StatType.Fire_Rate, BattleMech.instance.weaponController.mainWeaponEquiped.fireRate },
        { StatType.Tech_Damage, BattleMech.instance.weaponController.altWeaponEquiped.damage},
        { StatType.Fuel_Tank, BattleMech.instance.weaponFuelManager.weaponFuelMax},
        { StatType.Dash_Cooldown, BattleMech.instance.myCharacterController.dashCooldown},
        { StatType.Pulse_Range, BattleMech.instance.pulseShockwave.range},
        { StatType.Charge_Rate, BattleMech.instance.droneController.airDropTimer.chargeRate},
        };

        statMultiplierManager.LoadBaseValues(baseStats);
    }

    private IEnumerator ShowControls()
    {
        yield return new WaitForSeconds(1);
        gameUI.pauseMenu.PauseGame();
        gameUI.pauseMenu.menu.SetActive(false);
        gameUI.pauseMenu.controlsMenu.SetActive(true);
        gameUI.eventSystem.SetSelectedGameObject(gameUI.pauseMenu.controlsSelectedButton);
        gameUI.pauseMenu.SwapControlsMenu();
    }

    public void SwapPlayerInput(string inputMap)
    {
        playerInput.SwitchCurrentActionMap(inputMap);
    }

    public void LoadMainMenu()
    {
        altWeaponController.ClearWeaponInputs();
        SceneLoader.instance.LoadScene(2);
    }

    public void LoadNextRoom()
    {
        StartCoroutine(DelayedLoadNextRoom());
    }

    public IEnumerator DelayedLoadNextRoom()
    {
        gameUI.gameUIFade.FadeOut();
        yield return new WaitForSeconds(2);
        CashCollector.instance.DestroyParts();
        areaManager.LoadRoom(currentAreaType);
        BattleManager.instance.SetBattleType();
        BattleManager.instance.UpdateCrawlerSpawner();
        DayNightCycle();
        playerInput.transform.position = Vector3.zero;
        BattleManager.instance.roomDrop.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        RoomPortal.visualPortalEffect.StopFirstPersonEffect();
        _myCharacterController.ToggleCanMove(true);
        gameUI.gameUIFade.FadeIn();
        gameUI.objectiveUI.UpdateObjective(BattleManager.instance.objectiveMessage);
        gameActive = true;
        AudioManager.instance.PlayBattleMusic();

    }

    public IEnumerator LoadVoidRoom()
    {
        gameUI.gameUIFade.FadeOut();
        RoomPortal.portalEffect.StopEffect();
        yield return new WaitForSeconds(2);
        areaManager.LoadVoidArea();
        voidAreaManager.InitVoidArea();
        DayNightCycle(true);
        playerInput.transform.position = Vector3.zero;
        yield return new WaitForSeconds(1);
        RoomPortal.visualPortalEffect.StopFirstPersonEffect();
        yield return new WaitForSeconds(1);
        _myCharacterController.ToggleCanMove(true);
        gameUI.gameUIFade.FadeIn();

    }

    public void CompleteVoidRoom()
    {
        //TODO: Add Void Room Completion interaction
        currentAreaType++;
        SpawnPortalsToNextRoom(true);
    }

    private void DayNightCycle(bool night = false)
    {

        dayLight.SetActive(!night);
        nightLight.SetActive(night);
        if (IsDarkRoom())
        {
            dayLight.SetActive(false);
            nightLight.SetActive(true);
        }
    }

    private bool IsDarkRoom()
    {
        BattleType currenType = BattleManager.instance.currentBattle.battleType;
        return currentAreaType == AreaType.Void || currenType == BattleType.Survive || currenType == BattleType.Boss;
    }


    public void SpawnPortalsToNextRoom(bool voidRoom = false)
    {
        voidPortalManager.transform.position = voidRoom ? voidPortalManager.voidPortalLocation.position : playerInput.transform.position;

        if (BattleManager.instance.currentBattleIndex > BattleManager.instance.Battles.Count - 1)
        {
            voidPortalManager.StartVoidEffect();
            BattleManager.instance.ResetOnNewArea();
        }
        else
        {
            voidPortalManager.StartEffect();
        }
        BattleManager.instance.crawlerSpawner.waveText.text = "Head through the Portal!";
    }



    public void EndGame(bool won)
    {
        AudioManager.instance.PlayMusic(4);
        gameActive = false;
        CrawlerSpawner.instance.EndBattle();
        PlayerProgressManager.instance.EndGamePlayerProgress(won);
        gameUI.ShowEndGamePanel(won);
        SwapPlayerInput("UI");
    }

    public void EndGameCall(bool win)
    {
        StartCoroutine(EndGameDelay(win));
    }

    private IEnumerator EndGameDelay(bool win)
    {
        yield return new WaitForSeconds(1f);
        EndGame(win);
    }
}