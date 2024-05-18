using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CrateType
{
    Gun,
    Flamer,
    Armour
}

public class AirDropCrate : MonoBehaviour
{
    public CrateType crateType;
    public ParticleSystem _particleSystem;
    public bool active;
    private MeshRenderer _meshRenderer;
    private BoxCollider _collider;
    public Rigidbody rb;
    public GameObject _object;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }

    public void Init()
    {
        active = true;
        _meshRenderer.enabled = active;
        _collider.enabled = !active;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        _object.SetActive(true);
    }

    private void DeActive()
    {
        active = !active;
        _meshRenderer.enabled = active;
        _collider.enabled = active;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        _object.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        print("box hit " + other.gameObject.name);
        if (other.gameObject.CompareTag("Player"))
        {
            if(!active)
            {
                return;
            }
            EnableCrate();
            _particleSystem.Play();
            DeActive();
        }
    }

    public void Launch()
    {
        _collider.enabled = true;
        rb.useGravity = true;
        rb.AddExplosionForce(100, transform.position, 10);
        rb.transform.SetParent(null);
    }

    private void EnableCrate()
    {
        switch (crateType)
        {
            case CrateType.Gun:
                break;
            case CrateType.Flamer:
                break;
            case CrateType.Armour:
                MechBattleController.instance.RepairArmour();
                break;
            default:
                break;
        }
    }
}
