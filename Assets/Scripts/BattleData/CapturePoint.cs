using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour
{   
    private bool _enabled;
    public ParticleSystem online;
    public ParticleSystem inProgress;
    public float captureTime;
    public float captureProgress;
    public float captureSpeed;
    public bool isCaptured;
    public bool playerInArea;
    public GameObject ball;

    public void Init()
    {
        isCaptured = false;
        captureProgress = 0;
        gameObject.SetActive(true);
        _enabled = true;
        online.Play();
        ball.SetActive(true);
    }

    public void Update()
    {
        if (!_enabled)
        {
            if (inProgress.isPlaying)
            {
                inProgress.Stop();
            }
            return;
        }
        if(playerInArea)
        {
            CapturePointProgress();
            if (!inProgress.isPlaying)
            {
                inProgress.Play();
            }
        }
        else
        {
            if(inProgress.isPlaying)
            {
                inProgress.Stop();
            }
        }
    }

    public void CapturePointProgress()
    {
        captureProgress += Time.deltaTime * captureSpeed;
        if (captureProgress >= captureTime)
        {
            Capture();
            return;
        }
        var fillAmount = captureProgress / captureTime;
        GameUI.instance.objectiveUI.UpdateBar(fillAmount);
        GameUI.instance.objectiveUI.UpdateUpload( (fillAmount*100).ToString("0")+"%");
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInArea = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInArea = false;
        }
    }

    public void Capture()
    {
        Debug.Log("Capture Point Captured");
        online.Stop();
        inProgress.Stop();
        isCaptured = true;
        _enabled = false;
        captureProgress = 0;
        ball.SetActive(false);
        CrawlerSpawner.instance.KillAllCrawlers();
        BattleManager.instance.ObjectiveComplete();
        // for finishing off the fight
        /*
        if(CrawlerSpawner.instance.activeCrawlerCount==0)
        {
            BattleManager.instance.ObjectiveComplete();
            GameUI.instance.StartCoroutine(GameUI.instance.objectiveUI.ObjectiveComplete());
        }
        else 
        {
            GameUI.instance.objectiveUI.UpdateObjective("Finish them off!");
            BattleManager.instance._usingBattleType = BattleType.Exterminate;
            CrawlerSpawner.instance.EndBattle();
        }
        */
    }
}
