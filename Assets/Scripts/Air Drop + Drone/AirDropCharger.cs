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
    public GameObject[] chargesIcons;
    public int charges;
    public bool charged;
    public TMP_Text airDropText;
    public AudioSource audioSource;
    public AudioClip airDropSound;

    void Start()
    {
        ActivateButton(false);
        for (int i = 0; i < chargesIcons.Length; i++)
        {
            chargesIcons[i].SetActive(false);
        }
    }

    void Update()
    {
        ChargeOverTime();

        float percentage = DroneCharge / DroneMaxCharge;

        cover.fillAmount = percentage;
        if (charges > 0)
        {
            ActivateButton(true);
        }
    }

    public void SetChargeRate(float rate)
    {
        chargeRate = rate;
    }

    public void ChargeDrone(float value)
    {
        DroneCharge += value;
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
        if (charges == 3)
        {
            return;
        }
        ChargeDrone(chargeRate * Time.deltaTime);
        if (DroneCharge >= DroneMaxCharge)
        {
            DroneCharge = 0;
            charges++;
            airDropText.text = "Drone Ready";
            airDropText.enabled = true;
            SetBars();
        }

    }

    public void ActivateButton(bool value)
    {
        charged = value;
        airDropText.enabled = value;
        if (value)
        {
            audioSource.PlayOneShot(airDropSound);
        }
    }

    public void UseCharge(int amount)
    {
        if (charges > 0)
        {
            charges -= amount;
            SetBars();
        }
        if (charges == 0)
        {
            ActivateButton(false);
        }
        else
        {
            airDropText.text = "Drone Ready";
            airDropText.enabled = true;
        }
    }

    private void SetBars()
    {
        for (int i = 0; i < chargesIcons.Length; i++)
        {
            chargesIcons[i].SetActive(false);
        }
        for (int i = 0; i < charges; i++)
        {
            chargesIcons[i].SetActive(true);
        }
    }

    public void ResetAirDrop()
    {
        ActivateButton(false);
    }
}
