using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingRock : MonoBehaviour
{
    private Rigidbody _rb;
    public float force;
    public float torque;
    public float maxForce;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.AddForce(Random.insideUnitSphere * force, ForceMode.Impulse);
        _rb.AddTorque(Random.insideUnitSphere * torque, ForceMode.Impulse);
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, Vector3.zero) > 100)
        {
            Vector3 direction = Vector3.zero - transform.position;
            _rb.AddForce(direction.normalized * force, ForceMode.Impulse);
            _rb.AddTorque(Random.insideUnitSphere * torque, ForceMode.Impulse);
        } 


        if (_rb.velocity.magnitude > maxForce)
        {
            _rb.velocity = _rb.velocity.normalized * maxForce;
        }
    }

}
