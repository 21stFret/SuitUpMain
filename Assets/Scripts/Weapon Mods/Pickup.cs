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
        pickupRenderer = GetComponent<Renderer>();
        pickupCollider = GetComponent<Collider>();
        pickupLight = GetComponentInChildren<Light>();

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
                pickupColor = Color.blue;
                break;
        }
        pickupCollider.enabled = true;
        pickupRenderer.enabled = true;
        pickupLight.enabled = true;
        pickupRenderer.material.color = pickupColor;
        pickupRenderer.material.SetColor("_EmissionColor", pickupColor * 10);
        pickupLight.color = pickupColor;
    }

    private void PickUp()
    {
        ModUI.instance.OpenModUI(pickupType);
    }

    private void RemovePickup()
    {
        pickupRenderer.enabled = false;
        pickupCollider.enabled = false;
        pickupLight.enabled = false;
    }
}
