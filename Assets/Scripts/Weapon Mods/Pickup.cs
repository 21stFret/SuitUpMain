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
    public ParticleSystem pickupParticles;

    [InspectorButton("SetupPickup")]
    public bool canpickup;

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
        StartCoroutine(SetupPickup());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!canpickup) return;

        if (other.CompareTag("Player"))
        {
            PickUp();
            RemovePickup();
        }
    }

    private IEnumerator SetupPickup()
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
        }
        pickupParticles.Play();
        yield return new WaitForSeconds(1f);
        pickupCollider.enabled = true;
        pickupRenderer.enabled = true;
        pickupLight.enabled = true;
        pickupRenderer.material.SetColor("_EmissionColor", pickupColor * 5);
        pickupLight.color = pickupColor;
        canpickup = true;
    }

    private void PickUp()
    {
        canpickup = false;
        runUpgradeManager.GenerateListOfUpgrades(pickupType);
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
        pickupParticles.Stop();
    }
}
