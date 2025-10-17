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
    public GameObject pickupModel, upgradeModel;

    public ParticleSystem pickupUpgrade;
    public bool upgrade;

    [InspectorButton("SetupPickupT")]
    public bool canpickup;

    public void Init(ModBuildType type, bool isUpgrade = false)

    {
        pickupCollider = GetComponent<Collider>();
        pickupLight = GetComponentInChildren<Light>(true);
        pickupType = type;
        upgrade = false;
        if (isUpgrade)
        {
            upgrade = true;
            upgradeModel.SetActive(true);
            pickupModel.SetActive(false);
            pickupType = GameManager.instance.nextBuildtoLoad;
        }
        pickupModel.SetActive(false);
        gameObject.SetActive(true);
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
            pickupRenderer.material.SetColor("_EmissionColor", pickupColor * 5);
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
        canpickup = true;
    }

    private void PickUp()
    {
        canpickup = false;
        if (GameManager.instance == null)
        {
            return;
        }
        if (upgrade)
        {
            pickupUpgrade.Stop();
            CashCollector.instance.AddArtifact(1);
            runUpgradeManager.upgradePickup = true;
        }
        runUpgradeManager.GenerateListOfUpgradesFromAll(pickupType);
        pickupParticles.Stop();
        BattleMech.instance.myCharacterController.ToggleCanMove(false);
    }

    private void RemovePickup()
    {
        pickupCollider.enabled = false;
        pickupLight.enabled = false;
        pickupParticles.Clear();
        pickupModel.SetActive(false);
        upgradeModel.SetActive(false);
    }
}
