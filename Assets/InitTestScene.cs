using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitTestScene : MonoBehaviour
{
    public MechLoader mechLoadOut;
    public ConnectWeaponHolderToManager weaponHolder;
    public CrawlerSpawner crawlerSpawner;
    public bool spawnersActive = true;
    public float delay = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        Invoke("DelayedStart", delay);
    }

    public void DelayedStart()
    {
        BattleMech.instance.myCharacterController.ToggleCanMove(true);
        weaponHolder.SetupWeaponsManager();
        WeaponsManager.instance.LoadWeaponsData(PlayerSavedData.instance._mwData, PlayerSavedData.instance._awData);
        mechLoadOut.Init();
        crawlerSpawner.Init();
        crawlerSpawner.isActive = spawnersActive;
        AudioManager.instance.Init();
        AudioManager.instance.PlayBGMusic(1);
    }
}
