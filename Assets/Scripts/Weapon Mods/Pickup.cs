using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pickup : MonoBehaviour
{
    public Renderer pickupRenderer;
    private Collider pickupCollider;

    [ColorUsage(true, true)]
    public Color pickupColor;
    public ModBuildType pickupType;
    public Light pickupLight;

    public RunUpgradeManager runUpgradeManager;
    public ParticleSystem pickupParticles;
    public GameObject pickupModel;

    public ParticleSystem pickupUpgrade;
    public bool upgrade;

    [InspectorButton("SetupPickupT")]
    public bool canpickup;

    private void Start()
    {
        Init(pickupType);
    }

    public void Init(ModBuildType type)
    {
        pickupCollider = GetComponent<Collider>();
        pickupLight = GetComponentInChildren<Light>(true);
        pickupType = type;
        upgrade = false;
        if(pickupType == ModBuildType.UPGRADE)
        {
            upgrade = true;
        }
        pickupModel.SetActive(false);
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

    private void SetupPickupT()
    {
        StartCoroutine(SetupPickup());
    }

    private IEnumerator SetupPickup()
    {
        if (upgrade)
        {
            pickupUpgrade.Play();
        }
        else
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
            pickupModel.transform.localScale = Vector3.zero;
            float scale = 0.35f;
            Vector3 s = new Vector3(scale, scale, scale);
            pickupModel.transform.DOScale(s, 1f);
            pickupModel.SetActive(true);
            yield return new WaitForSeconds(1f);
            pickupLight.enabled = true;
            pickupLight.color = pickupColor;
        }
        pickupCollider.enabled = true;
        pickupRenderer.material.SetColor("_EmissionColor", pickupColor * 5);
        canpickup = true;

    }

    private void PickUp()
    {
        canpickup = false;
        runUpgradeManager.GenerateListOfUpgradesFromAll(pickupType);
        if (GameManager.instance == null)
        {
            return;
        }
        if(upgrade)
        {
            CashCollector.instance.AddArtifact(1);
            pickupUpgrade.Stop();
        }
        pickupParticles.Stop();

    }

    private void RemovePickup()
    {
        pickupCollider.enabled = false;
        pickupLight.enabled = false;
        pickupParticles.Clear();
        pickupModel.SetActive(false);
    }
}
