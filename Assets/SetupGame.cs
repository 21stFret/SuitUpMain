using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupGame : MonoBehaviour
{
    public static SetupGame instance;
    public GameManager gameManager;
    public bool endlessMode;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void ToggleEndlessMode()
    {
        endlessMode = !endlessMode;
    }

    public void LinkGameManager(GameManager _gameManager)
    {
        gameManager = _gameManager;
        gameManager.endlessMode = endlessMode;
        gameManager.DelayedStart();
    }
}
