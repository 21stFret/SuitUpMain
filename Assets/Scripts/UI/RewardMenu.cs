using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RewardMenu : MonoBehaviour
{
    public TMP_Text cashReward;
    public TMP_Text expReward;
    public TMP_Text artifactReward;

    public void SetRewards(int cash, int exp, int artifact)
    {
        cashReward.text = cash.ToString();
        expReward.text = exp.ToString();
        artifactReward.text = artifact.ToString();
    }
}
