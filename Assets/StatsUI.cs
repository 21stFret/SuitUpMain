using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    public TMP_Text kills;
    public TMP_Text waves;

    public void UpdateStats(int kills, int waves)
    {
        this.kills.text = kills.ToString();
        this.waves.text = waves.ToString();
    }
}
