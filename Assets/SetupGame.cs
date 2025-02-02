using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy,
    Mid,
    Hard,
    Insane
}

public class SetupGame : MonoBehaviour
{
    public static SetupGame instance;
    public GameManager gameManager;
    public bool endlessMode;
    public bool inGame;
    public Difficulty diffiulty;
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
        BattleManager battleManager = BattleManager.instance;
        battleManager.Battles.Clear();
        switch (diffiulty)
        {
            case Difficulty.Easy:
                battleManager.Battles.AddRange(roomwavesEasy);
                break;
            case Difficulty.Mid:
                battleManager.Battles.AddRange(roomwavesMid);
                break;
            case Difficulty.Hard:
                battleManager.Battles.AddRange(roomwavesHard);
                break;
        }
        battleManager.dificultyMultiplier = diffiulty == Difficulty.Easy ? 1f : diffiulty == Difficulty.Mid ? 1.5f : 2f;
        gameManager.DelayedStart();
    }
}
