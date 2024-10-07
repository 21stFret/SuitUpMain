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
    public ManualWeaponController altWeaponController;
    public AreaManager areaManager;
    public VoidPortalManager voidPortalManager;
    public RoomPortal RoomPortal;
    public int currentRoomIndex;
    public bool endlessMode;
    public bool gameActive;
    public GameObject dayLight;
    public GameObject nightLight;
    public ModBuildType nextBuildtoLoad;
    public AreaType currentAreaType;

    public bool playOnAwake = false;

    [InspectorButton("SpawnPortalsToNextRoom")]
    public bool spawnPortal;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        if (SetupGame.instance != null) SetupGame.instance.LinkGameManager(this);
        else Invoke("DelayedStart", 0.1f);
    }

    public void DelayedStart()
    {

        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
        mechLoadOut.Init();


        if (!playOnAwake) return;

        nextBuildtoLoad = (ModBuildType)Random.Range(0, 4);
        currentRoomIndex = -1;
        areaManager.LoadRoom(currentAreaType);
        gameActive = true;
        BattleManager.instance.SetBattleType();
        BattleManager.instance.currentBattleIndex = 0;
        BattleManager.instance.UpdateCrawlerSpawner();
        AudioManager.instance.PlayMusic(1);
        gameUI.objectiveUI.UpdateObjective(BattleManager.instance.objectiveMessage);
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
        SceneLoader.instance.LoadScene(1);
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
        yield return new WaitForSeconds(1);
        gameUI.gameUIFade.FadeIn();
        gameUI.objectiveUI.UpdateObjective(BattleManager.instance.objectiveMessage);
    }

    private void DayNightCycle(bool night = false)
    {
        bool dayTime = Random.Range(0, 100) < 50;
        dayLight.SetActive(dayTime);
        nightLight.SetActive(!dayTime);
        if (night)
        {
            dayLight.SetActive(false);
            nightLight.SetActive(true);
        }
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

    public IEnumerator LoadVoidRoom()
    {
        RoomPortal.portalEffect.StopEffect();
        yield return new WaitForSeconds(2);
        areaManager.LoadVoidArea();
        DayNightCycle(true);
        playerInput.transform.position = Vector3.zero;
        yield return new WaitForSeconds(1);
        RoomPortal.visualPortalEffect.StopFirstPersonEffect();
        yield return new WaitForSeconds(1);
        gameUI.gameUIFade.FadeIn();
        currentAreaType++;
        SpawnPortalsToNextRoom(true);
    }

    public void EndGame(bool won)
    {
        AudioManager.instance.PlayMusic(3);
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