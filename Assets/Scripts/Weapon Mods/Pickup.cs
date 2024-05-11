using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    WeaponMod,
    Health,
    Bonus,
}

public class Pickup : MonoBehaviour
{
    private Renderer pickupRenderer;
    private Collider pickupCollider;

    [ColorUsage(true, true)]
    public Color pickupColor;
    public PickupType pickupType;
    public Light pickupLight;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        pickupRenderer = GetComponent<Renderer>();
        pickupCollider = GetComponent<Collider>();
        pickupLight = GetComponentInChildren<Light>(true);

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
            case PickupType.WeaponMod:
                pickupColor = Color.red;
                break;
            case PickupType.Health:
                pickupColor = Color.green;
                break;
            case PickupType.Bonus:
                pickupColor = Color.cyan;
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
        GameUI.instance.OpenModUI(pickupType);
        if (GameManager.instance == null)
        {
            return;
        }
        CashCollector.Instance.AddArtifact(1);
    }

    private void RemovePickup()
    {
        pickupRenderer.enabled = false;
        pickupCollider.enabled = false;
        pickupLight.enabled = false;
    }
}
