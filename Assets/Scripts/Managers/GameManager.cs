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
    public int currentRoomIndex;
    public bool endlessMode;
    public bool gameActive;

    public ModBuildType nextBuildtoLoad;
    public AreaType currentAreaType;
    public StatMultiplierManager statMultiplierManager;
    public RunUpgradeManager runUpgradeManager;

    public bool playOnAwake = false;
    public TestPlayerData TESTplayerData;

    [InspectorButton("SpawnPortalsToNextRoom")]
    public bool spawnPortal;

    private MYCharacterController _myCharacterController;

    public CreditsController creditsController;

    [SerializeField]
    private bool wonGame;
    private BattleManager battleManager;

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
        battleManager = BattleManager.instance;
        _myCharacterController = BattleMech.instance.myCharacterController;
        _myCharacterController.ToggleCanMove(true);
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
        mechLoadOut.Init();
        InitializeStats();
        AudioManager.instance.Init();
        AudioManager.instance.PlayBattleMusic();
        runUpgradeManager.LoadData();
        wonGame = false;

        if (!playOnAwake) return;

        runUpgradeManager.weaponModManager.RemoveCurrentMods();
        nextBuildtoLoad = (ModBuildType)Random.Range(0, 4);
        currentRoomIndex = -1;
        gameActive = true;
        battleManager.currentBattleIndex = 0;
        battleManager.crawlerSpawner.Init();
        LoadNextRoom(0);
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

    public void SwapPlayerInput(string inputMap)
    {
        playerInput.SwitchCurrentActionMap(inputMap);
    }

    public void LoadNextRoom(float delay = 2)
    {
        StartCoroutine(DelayedLoadNextRoom(delay));
    }

    public IEnumerator DelayedLoadNextRoom(float delay)
    {
        gameUI.gameUIFade.FadeOut();
        yield return new WaitForSeconds(delay);
        AudioManager.instance.PlayBattleMusic();
        CashCollector.instance.DestroyParts();
        battleManager.roomDrop.gameObject.SetActive(false);
        battleManager.SetBattleType();
        areaManager.LoadRoom(currentAreaType);
        battleManager.UpdateCrawlerSpawner();
        playerInput.transform.position = Vector3.zero;
        yield return new WaitForSeconds(delay/2);
        voidPortalManager.StopFirstPersonEffect();
        _myCharacterController.ToggleCanMove(true);
        gameUI.gameUIFade.FadeIn();
        gameUI.objectiveUI.UpdateObjective(battleManager.objectiveMessage);
        gameActive = true;
    }

    public IEnumerator LoadVoidRoom()
    {
        gameUI.gameUIFade.FadeOut();
        yield return new WaitForSeconds(2);
        areaManager.LoadVoidArea();
        voidAreaManager.InitVoidArea();
        playerInput.transform.position = Vector3.zero;
        yield return new WaitForSeconds(1);
        voidPortalManager.StopFirstPersonEffect();
        _myCharacterController.ToggleCanMove(true);
        yield return new WaitForSeconds(1);
        gameUI.gameUIFade.FadeIn();

    }

    public void CompleteVoidRoom()
    {
        //TODO: Add Void Room Completion interaction
        currentAreaType++;
        battleManager.armyGen.SetCurrentAreaType(currentAreaType);
        SpawnPortalsToNextRoom(true);
    }

    public void SpawnPortalsToNextRoom(bool voidRoom = false)
    {
        voidPortalManager.transform.position = playerInput.transform.position;
        battleManager.crawlerSpawner.waveText.text = "Head through the Portal!";

        if (battleManager.currentBattleIndex > battleManager.Battles.Count - 1)
        {
            voidPortalManager.StartVoidEffect(true);
            battleManager.ResetOnNewArea();
            return;
        }
        if (battleManager.currentBattleIndex > battleManager.Battles.Count - 2)
        {
            voidPortalManager.StartVoidEffect(false);
        }
        else
        {
            voidPortalManager.StartAllEffects();
        }
    }



    public void EndGame(bool won)
    {
        AudioManager.instance.PlayBGMusic(4);
        gameActive = false;
        CrawlerSpawner.instance.EndBattle();
        if(PlayerProgressManager.instance != null && SetupGame.instance != null)
        {
            PlayerProgressManager.instance.EndGamePlayerProgress(won, (int)SetupGame.instance.diffiulty);
        }
        if(PlayerSavedData.instance != null && SetupGame.instance != null)
        {
            PlayerSavedData.instance.highestDifficulty = won?  (int)SetupGame.instance.diffiulty+1: PlayerSavedData.instance.highestDifficulty;
            PlayerSavedData.instance.SavePlayerData();
        }
        wonGame = won;
        StartCoroutine(DelayedEndGame(won));
    }

    private IEnumerator DelayedEndGame(bool won)
    {
        yield return new WaitForSeconds(2);
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

    public void LoadMainMenu()
    {
        CrawlerSpawner.instance.EndBattle();
        gameActive = false;
        altWeaponController.ClearWeaponInputs();
        Time.timeScale = 1;
        if(wonGame)
        {
            if (!PlayerSavedData.instance.hasSeenThankYouPanel)
            {
                ShowCredits();
                return;
            }
        }

        SceneLoader.instance.LoadScene(2);
    }

    public void ShowCredits()
    {
        creditsController.gameObject.SetActive(true);
    }
}