using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour
{
    private bool _enabled;
    public ParticleSystem online;
    public ParticleSystem inProgress;
    public float captureTime;
    private float _captureTime;
    public float captureProgress;
    public float captureSpeed;
    public bool isCaptured;
    public bool playerInArea;
    public GameObject ball;

    public void Init()
    {
        isCaptured = false;
        captureProgress = 0;
        playerInArea = false;
        gameObject.SetActive(true);
        _enabled = true;
        online.Play();
        ball.SetActive(true);
        _captureTime = captureTime + (0.25f * BattleManager.instance.dificultyMultiplier);
        GameUI.instance.objectiveUI.UpdateUpload("0%");
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
        if (playerInArea)
        {
            CapturePointProgress();
            if (!inProgress.isPlaying)
            {
                inProgress.Play();
            }
        }
        else
        {
            if (inProgress.isPlaying)
            {
                inProgress.Stop();
            }
        }
    }

    public void CapturePointProgress()
    {
        captureProgress += Time.deltaTime * captureSpeed;
        if (captureProgress >= _captureTime)
        {
            Capture();
            return;
        }
        var fillAmount = captureProgress / _captureTime;
        GameUI.instance.objectiveUI.UpdateBar(fillAmount);
        GameUI.instance.objectiveUI.UpdateUpload((fillAmount * 100).ToString("0") + "%");
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
        CrawlerSpawner.instance.KillAllCrawlers();
        BattleManager.instance.ObjectiveComplete();
        GameManager.instance.areaManager.missileLauncher.missilePayload = MissilePayload.FatMan;
        GameManager.instance.areaManager.missileLauncher.SpawnExplosion(Vector3.zero);
        PostProcessController.instance.NukeEffect();
    }
    
    public void ProcessCapture()
    {
        online.Stop();
        inProgress.Stop();
        isCaptured = true;
        _enabled = false;
        captureProgress = 0;
        ball.SetActive(false);
    }
}
