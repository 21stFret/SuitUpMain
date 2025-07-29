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
    public DroneAbilityManager droneAbilityManager;

    public CreditsController creditsController;

    [SerializeField]
    private bool wonGame;
    private BattleManager battleManager;
    private bool triggeredAreaIntro;
    public AreaIntro areaIntro;

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
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mwData, PlayerSavedData.instance._awData);
        mechLoadOut.Init();
        InitializeStats();
        AudioManager.instance.Init();
        AudioManager.instance.PlayBattleMusic();
        runUpgradeManager.LoadData();
        wonGame = false;
        droneAbilityManager.Init();

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
        { StatType.Assault_Damage, BattleMech.instance.weaponController.mainWeaponEquiped.damage },
        { StatType.Speed, BattleMech.instance.myCharacterController.Speed },
        { StatType.Fire_Rate, BattleMech.instance.weaponController.mainWeaponEquiped.fireRate },
        { StatType.Tech_Damage, BattleMech.instance.weaponController.altWeaponEquiped.damage},
        { StatType.Fuel_Tank, BattleMech.instance.weaponFuelManager.weaponFuelMax},
        { StatType.Dash_Cooldown, BattleMech.instance.myCharacterController.dashCooldown},
        { StatType.Pulse_Range, BattleMech.instance.pulseShockwave.range},
        { StatType.Charge_Rate, BattleMech.instance.droneController.airDropTimer.chargeRate},
        { StatType.Seconds, 0 },
        { StatType.Heals, 0 },
        { StatType.Stun_Time, 0 },
        { StatType.Range, 0 },
        { StatType.Spread, 0 },
        { StatType.Extra_Shot, 0 },
        { StatType.Pierce, 0 },
        { StatType.Bounce, 0 },
        { StatType.Force, 0 },
        { StatType.Split, 0 },
        { StatType.Chains, 0 },
        { StatType.Reload_Time, 0 },
        { StatType.Shield_Health, 0 },
        { StatType.Unique, 0 },
        { StatType.Assault, 0 },
        { StatType.Tech, 0 },
        { StatType.Invincible, 0 }
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
        if (!triggeredAreaIntro)
        {
            areaIntro.ShowAreaIntro();
            triggeredAreaIntro = true;
        }
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
        triggeredAreaIntro = false;
        battleManager.armyGen.SetCurrentAreaType(currentAreaType);
        SpawnPortalsToNextRoom();
    }

    public void SpawnPortalsToNextRoom()
    {
        voidPortalManager.transform.position = battleManager.crawlerSpawner.spawnPoints[0].position;
        if (battleManager.currentBattleIndex == 0)
        {
            voidPortalManager.transform.position = voidPortalManager.voidPortalLocation.position;
        }
        battleManager.crawlerSpawner.waveText.text = "Head through the Portal!";

        if (battleManager.currentBattleIndex > battleManager.Battles.Count - 1)
        {
            voidPortalManager.StartVoidEffect(true);
            voidPortalManager.transform.position = BattleMech.instance.transform.position;
            battleManager.ResetOnNewArea();
            return;
        }
        if (battleManager.currentBattleIndex % 2 == 1)
        {
            voidPortalManager.StartVoidEffect(false);
            return;
        }
        voidPortalManager.StartAllEffects();
    }

    public void EndGame(bool won)
    {
        AudioManager.instance.PlayBGMusic(4);
        gameActive = false;
        CrawlerSpawner.instance.EndBattle();
        CrawlerSpawner.instance.KillAllCrawlers();
        if(SetupGame.instance == null)
        {
            return;
        }
        if(PlayerProgressManager.instance != null)
        {
            PlayerProgressManager.instance.EndGamePlayerProgress(won, (int)SetupGame.instance.diffiulty);
        }
        if(PlayerSavedData.instance != null)
        {
            if(won)
            {
                PlayerSavedData.instance.topDif++;
                if(PlayerSavedData.instance.topDif > 3)
                {
                    PlayerSavedData.instance.topDif = 3;
                }
            }
            PlayerSavedData.instance.SavePlayerData();
        }
        wonGame = won;
        StartCoroutine(DelayedEndGame(won));
    }

    private IEnumerator DelayedEndGame(bool won)
    {
        yield return new WaitForSeconds(1);
        gameUI.ShowEndGamePanel(won);
        SwapPlayerInput("UI");
    }

    public void EndGameCall(bool win)
    {
        Time.timeScale = 1;
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
        Time.timeScale = 1;
        if(wonGame)
        {
            if (!PlayerSavedData.instance.tyPanel)
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