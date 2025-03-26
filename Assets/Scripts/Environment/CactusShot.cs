using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusShot : MonoBehaviour
{
    public float speed = 10.0f;
    public float damage = 2.0f;
    public float lifeTime = 2.0f;

    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<CactusShot>() != null)
        {
            return;
        }

        var target = collision.gameObject.GetComponent<TargetHealth>();
        if(target != null)
        {
            target.TakeDamage(damage);
        }
        //Destroy(gameObject);
    }
}
