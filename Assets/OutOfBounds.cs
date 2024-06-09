using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OutOfBounds : MonoBehaviour
{
    public MechHealth mechHealth;
    public Transform mech;
    public Transform mapCentre;
    public float distanceThreshold = 10f;
    public float warningTimer = 5f;
    private float currentTimer = 0f;
    private bool isPlayerOutOfRange = false;
    public TMP_Text warningText;
    public GameObject warningOverlay;
    public DoTweenFade doTweenFade;
    public bool _enabled;

    private void Update()
    {
        if (!_enabled)
        {
            return;
        }

        float distance = Vector3.Distance(mech.position, mapCentre.position);

        if (distance > distanceThreshold)
        {
            if (!isPlayerOutOfRange)
            {
                StartWarningTimer();
            }
        }
        else
        {
            if (isPlayerOutOfRange)
            {
                StopWarningTimer();
            }
        }

        if (isPlayerOutOfRange)
        {
            UpdateWarningTimer();
        }
    }

    private void StartWarningTimer()
    {
        warningOverlay.SetActive(true);
        isPlayerOutOfRange = true;
        currentTimer = warningTimer;
        doTweenFade.PlayTween();
    }

    private void StopWarningTimer()
    {
        warningOverlay.SetActive(false);
        isPlayerOutOfRange = false;
        currentTimer = 0f;
        doTweenFade.KillTween();
    }

    private void UpdateWarningTimer()
    {
        if (currentTimer == 0f)
        {
            mechHealth.TakeDamage(1000);
            _enabled = false;
            warningOverlay.SetActive(false);
            return;
        }
        currentTimer -= Time.deltaTime;
        if(currentTimer < 0f)
        {
            currentTimer = 0f;
        }
        string timeleft = Mathf.Ceil(currentTimer).ToString();
        warningText.text = "Leaving the battlefield. Return in " + timeleft + "s or fail.";

    }
}
