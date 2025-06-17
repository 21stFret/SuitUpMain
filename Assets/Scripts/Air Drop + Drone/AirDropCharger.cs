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
    private bool buttonActive;
    private float localMaxCharge;

    void Start()
    {
        ActivateButton(false);
        SetBars();
        airDropText.enabled = false;
        localMaxCharge = DroneMaxCharge;
    }

    void Update()
    {
        ChargeOverTime();

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
                    airDropText.text = "STANDBY";
                }
                return;
            }
            else
            {
                airDropText.text = "READY";
            }
        }
        if (charges == 3)
        {
            cover.fillAmount = 1f;
            airDropText.text = "FULL";
            return;
        }
        ChargeDrone(chargeRate * Time.deltaTime);

        float percentage = DroneCharge / localMaxCharge;
        cover.fillAmount = percentage;

        if (DroneCharge >= localMaxCharge)
        {
            DroneCharge = 0;
            charges++;
            airDropText.text = "READY";
            airDropText.enabled = true;
            localMaxCharge = DroneMaxCharge + (DroneMaxCharge * charges * 0.5f); // Increase max charge by 50% each time
            SetBars();
        }

    }

    public void ActivateButton(bool value)
    {
        if (buttonActive == value)
        {
            return;
        }
        buttonActive = value;
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
            airDropText.text = "READY";
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
