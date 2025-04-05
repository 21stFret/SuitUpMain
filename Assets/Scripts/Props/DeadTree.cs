using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class DeadTree : Crate
{
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Boss")
        {
            Die();
        }
    }

    public override void RefreshProp()
    {
        Init();
    }
}
