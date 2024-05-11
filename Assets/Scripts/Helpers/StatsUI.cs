using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class StatsUI : MonoBehaviour
{
    public TMP_Text Artifacts;
    public TMP_Text _waves;
    public TMP_Text _cash;
    private bool removingCash = false;
    private int cashedCash;
    private int cashedArt;
    private float cashLerp = 0;

    public void RemoveCash(int cash)
    {
        cashedCash = PlayerSavedData.instance._Cash;
        PlayerSavedData.instance.UpdatePlayerCash(-cash);
        removingCash = true;
    }
    public void RemoveArtifact(int cash)
    {
        PlayerSavedData.instance.UpdatePlayerArtifact(-cash);
        UpdateArtifact(PlayerSavedData.instance._Artifact);
    }

    public void UpdateCash(int cash)
    {
        _cash.text = "$" + cash.ToString();
    }
    
    public void UpdateArtifact(int cash)
    {
        Artifacts.text = cash.ToString();
    }

    public void Update()
    {
        if (removingCash)
        {
            _cash.color = Color.red;
            cashLerp += Time.deltaTime * 2;
            _cash.text = "$" + Mathf.Lerp(cashedCash, PlayerSavedData.instance._Cash, cashLerp).ToString("0");
            if(cashLerp >= 1)
            {
                _cash.color = Color.white;
                cashLerp = 0;
                removingCash = false;
            }
        }
    }
}
