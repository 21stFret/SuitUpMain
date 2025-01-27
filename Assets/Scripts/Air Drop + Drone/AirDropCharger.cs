using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class AirDropCharger : MonoBehaviour
{
    public float DroneMaxCharge;
    public float DroneCharge;
    public float chargeRate;
    public Image cover;
    public bool charged;
    public TMP_Text airDropText;
    public AudioSource audioSource;
    public AudioClip airDropSound;

    void Start()
    {
        ActivateButton(false);
    }

    void Update()
    {
        ChargeOverTime();

        if(charged)
        {
            return;
        }

        float percentage = DroneCharge / DroneMaxCharge;

        cover.fillAmount = percentage;
        if (DroneCharge >= DroneMaxCharge)
        {
            ActivateButton(true);
        }
    }

    public void SetChargeRate(float rate)
    {
        chargeRate = rate;
    }

    private void ChargeOverTime()
    {
        if (GameManager.instance != null)
        {
            if (!GameManager.instance.gameActive)
            {
                if (charged)
                {
                    airDropText.enabled = true;
                    airDropText.text = "Drone On Standby";
                }
                return;
            }
            else
            {
                airDropText.text = "Drone Ready";
            }
        }
        if (charged)
        {
            return;
        }
        DroneCharge += chargeRate * Time.deltaTime;
    }

    public void ActivateButton(bool value)
    {
        charged = value;
        DroneCharge = value ? DroneMaxCharge:0;
        cover.fillAmount = value ? 1 : 0;
        airDropText.enabled = value;
        if (value)
        {
            audioSource.PlayOneShot(airDropSound);
        }
    }

    public void ResetAirDrop()
    {
        ActivateButton(false);
    }
}
