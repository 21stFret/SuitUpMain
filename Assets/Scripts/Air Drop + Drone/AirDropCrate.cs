using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirDropCrate : MonoBehaviour
{
    public DroneType crateType;
    public ParticleSystem _particleSystem;
    public bool active;
    public MeshRenderer _meshRenderer;
    public MeshRenderer _boxLightRenderer;
    Material _boxLightMaterial;
    private BoxCollider _collider;
    public Rigidbody rb;
    public GameObject _object;

    public Light _light;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }

    public void Init()
    {
        _boxLightMaterial = _boxLightRenderer.sharedMaterial;
        Color color = new Color(0, 0, 0, 0);
        if(crateType == DroneType.Repair)
        {
            color = Color.green;
        }
        else
        {
            color = Color.cyan;
        }
        _boxLightMaterial.SetColor("_EmissionColor", color);
        _light.color = color;

        active = true;
        _meshRenderer.enabled = active;
        _collider.enabled = !active;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        _object.SetActive(true);
    }

    private void DeActive()
    {
        active = false;
        _meshRenderer.enabled = active;
        _collider.enabled = active;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        _object.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("air drop crate hit " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!active)
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
            case DroneType.Repair:
                BattleMech.instance.RepairArmour(20);
                break;
            default:
                break;
        }
    }
}
