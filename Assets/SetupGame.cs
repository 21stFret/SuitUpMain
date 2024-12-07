using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupGame : MonoBehaviour
{
    public static SetupGame instance;
    public GameManager gameManager;
    public bool endlessMode;
    public bool inGame;
    public int diffiulty;
    public List<Battle> roomwavesEasy = new List<Battle>();
    public List<Battle> roomwavesMid = new List<Battle>();
    public List<Battle> roomwavesHard = new List<Battle>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
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
        gameManager.playOnAwake = true;

        BattleManager.instance.Battles.Clear();
        switch (diffiulty)
        {
            case 0:
                BattleManager.instance.Battles = roomwavesEasy;
                break;
            case 1:
                BattleManager.instance.Battles = roomwavesMid;
                break;
            case 2:
                BattleManager.instance.Battles = roomwavesHard;
                break;

        }

        gameManager.DelayedStart();
    }
}
