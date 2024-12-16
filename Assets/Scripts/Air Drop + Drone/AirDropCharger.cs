using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AirDropCharger : MonoBehaviour
{
    public float DroneMaxCharge;
    public float DroneCharge;
    public float chargeRate;
    public Image cover;
    public bool charged;
    public TMP_Text airDropText;

    void Start()
    {
        ActivateButton(false);
    }

    void Update()
    {
        if(!GameManager.instance.gameActive)
        {
            return;
        }

        if(charged)
        {
            return;
        }
        DroneCharge += chargeRate * Time.deltaTime;

        float percentage = DroneCharge / DroneMaxCharge;

        cover.fillAmount = percentage;
        if (DroneCharge >= DroneMaxCharge)
        {
            DroneCharge = DroneMaxCharge;
            ActivateButton(true);
        }
    }

    private void ActivateButton(bool value)
    {
        charged = value;
        airDropText.enabled = value;
    }

    public void ResetAirDrop()
    {
        ActivateButton(false);
    }
}
