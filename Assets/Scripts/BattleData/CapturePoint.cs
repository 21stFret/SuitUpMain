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
        GameUI.instance.objectiveUI.UpdateObjective("Upload Progress " + (fillAmount*100).ToString("0")+"%");
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
        isCaptured = true;
        _enabled = false;
        captureProgress = 0;
        ball.SetActive(false);
        BattleManager.instance.ObjectiveComplete();

        StartCoroutine(GameUI.instance.objectiveUI.ObjectiveComplete());
        online.Stop();
        inProgress.Stop();
    }
}
