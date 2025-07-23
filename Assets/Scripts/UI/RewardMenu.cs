using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RewardMenu : MonoBehaviour
{
    public TMP_Text cashReward;
    public TMP_Text expReward;
    public TMP_Text artifactReward;
    public TMP_Text playTimer;
    public TMP_Text _multiplier;

    public void SetRewards(int cash, int artifact, float playTime, float multiplier)
    {
        cashReward.text = cash.ToString();
        artifactReward.text = artifact.ToString();
        _multiplier.text = "Reward Multiplier : x" + multiplier.ToString("F1");
        int mins = Mathf.FloorToInt(playTime/60);
        string runTime = "Run Time ";

        if(mins > 0)
        {
            for(int i = 0; i < mins; i++)
            {
                playTime -= 60;
            }
            runTime += mins.ToString("F0") + "m ";
        }
        playTimer.text = runTime + playTime.ToString("F0")+"s";
    }
}
