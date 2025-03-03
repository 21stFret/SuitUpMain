using UnityEngine;
using UnityEngine.UI;

public class BaseManager : MonoBehaviour
{
    public static BaseManager instance; 
    public MechLoader mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;
    public StatsUI statsUI;
    public SceneLoader sceneLoader;
    public TestPlayerData playerData;
    public DirectionalDaylight daylight;
    public LoadOutPanel loadOutPanel;
    public GameObject thankYouPanel;
    public GameObject ThankyouButton;

    public Image globalBackButton;
    public Sprite button, key;

    private void Awake()
    {
        if (instance == null) instance = this;
        sceneLoader = SceneLoader.instance;
    }

    public void Start()
    {
        InitBaseArea();
    }

    public void InitBaseArea()
    {
        InitializeBaseSystem();
        if (ShouldShowThankYouPanel())
        {
            ShowThankYouPanel();
            return;
        }
    }

    private void InitializeBaseSystem()
    {
        BattleMech.instance.myCharacterController.ToggleCanMove(true);
        BattleMech.instance.playerInput.SwitchCurrentActionMap("UI");
        BattleMech.instance.playerInput.SwitchCurrentActionMap("Gameplay");
        weaponHolder.SetupWeaponsManager();
        if(PlayerSavedData.instance._mainWeaponData == null)
        {
            playerData.InittestData();
        }
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mainWeaponData, PlayerSavedData.instance._altWeaponData);
        mechLoadOut.Init();
        AudioManager.instance.Init();
        AudioManager.instance.PlayBGMusic(3);
        statsUI.UpdateCash(PlayerSavedData.instance._Cash);
        statsUI.UpdateArtifact(PlayerSavedData.instance._Artifact);
        daylight.startTime = Random.Range(0, 1f);
        globalBackButton.sprite = InputTracker.instance.usingMouse ? key : button;

        Debug.Log("Base Initialized");
    }

    private bool ShouldShowThankYouPanel()
    {
        return PlayerSavedData.instance.HasCompletedEasyMode() && 
               !PlayerSavedData.instance.hasSeenThankYouPanel;
    }

    private void ShowThankYouPanel()
    {
        BattleMech.instance.myCharacterController.ToggleCanMove(false);
        BattleMech.instance.playerInput.SwitchCurrentActionMap("UI");
        thankYouPanel.SetActive(true);
        InputTracker.instance.eventSystem.SetSelectedGameObject(ThankyouButton);
    }

    public void CloseThankYouPanel()
    {
        thankYouPanel.SetActive(false);
        PlayerSavedData.instance.hasSeenThankYouPanel = true;
        PlayerSavedData.instance.SavePlayerData();
        InitializeBaseSystem();
    }

    public void OpenDiscordLink()
    {
        Application.OpenURL("https://discord.gg/tRwM2pdGyQ");
    }

    public void StartGame(int value)
    {
        if(loadOutPanel.playLocked) 
        {
            AudioManager.instance.PlayButtonSFX((int)SFX.Error);
            return;
        }
        AudioManager.instance.PlayButtonSFX((int)SFX.Confirm);
        Time.timeScale = 1;
        sceneLoader.LoadScene(value, true);
    }
}
