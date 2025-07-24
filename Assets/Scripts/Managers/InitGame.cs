using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitGame : MonoBehaviour
{
    public bool demoBuild;  

    private void Start()
    {
        DelayedStart();
    }

    private void DelayedStart()
    {
        PlayerSavedData.instance.LoadPlayerData();
        PlayerSavedData.instance.demo = demoBuild;
        Time.timeScale = 1;
        AudioManager.instance.Init();
    }
}
