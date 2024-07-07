using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pickup : MonoBehaviour
{
    private Renderer pickupRenderer;
    private Collider pickupCollider;

    [ColorUsage(true, true)]
    public Color pickupColor;
    public ModBuildType pickupType;
    public Light pickupLight;

    public RunUpgradeManager runUpgradeManager;

    [InspectorButton("SetupPickup")]
    public bool ResetPickup;

    private void Start()
    {
        Init((ModBuildType)Random.Range(0,4));
    }

    public void Init(ModBuildType type)
    {
        pickupRenderer = GetComponent<Renderer>();
        pickupCollider = GetComponent<Collider>();
        pickupLight = GetComponentInChildren<Light>(true);
        pickupType = type;
        SetupPickup();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PickUp();
            RemovePickup();
        }
    }

    private void SetupPickup()
    {
        switch (pickupType)
        {
            case ModBuildType.ASSAULT:
                pickupColor = Color.red;
                break;
            case ModBuildType.TANK:
                pickupColor = Color.green;
                break;
            case ModBuildType.TECH:
                pickupColor = Color.cyan;
                break;
            case ModBuildType.AGILITY:
                pickupColor = Color.yellow;
                break;
            case ModBuildType.CURRENCY:
                pickupColor = Color.white;
                break;
        }
        pickupCollider.enabled = true;
        pickupRenderer.enabled = true;
        pickupLight.enabled = true;
        pickupRenderer.material.SetColor("_EmissionColor", pickupColor * 5);
        pickupLight.color = pickupColor;
    }

    private void PickUp()
    {
        runUpgradeManager.GenerateListOfUpgrades(pickupType);
        //GameUI.instance.OpenModUI(pickupType);
        if (GameManager.instance == null)
        {
            return;
        }
        CashCollector.instance.AddArtifact(1);
    }

    private void RemovePickup()
    {
        pickupRenderer.enabled = false;
        pickupCollider.enabled = false;
        pickupLight.enabled = false;
    }
}
